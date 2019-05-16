// -------------------------------------------------------------------------------------------------
// <copyright file="HashedWheelTimerScheduler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// This <see cref="IScheduler"/> implementation is built using a revolving wheel of buckets
    /// with each bucket belonging to a specific time resolution. As the "clock" of the scheduler ticks it advances
    /// to the next bucket in the circle and processes the items in it, and optionally reschedules recurring
    /// tasks into the future into the next relevant bucket.
    ///
    /// There are `akka.scheduler.ticks-per-wheel` initial buckets (we round up to the nearest power of 2) with 512
    /// being the initial default value. The timings are approximated and are still limited by the ceiling of the operating
    /// system's clock resolution.
    ///
    /// Further reading: http://www.cs.columbia.edu/~nahum/w6998/papers/sosp87-timing-wheels.pdf
    /// Presentation: http://www.cse.wustl.edu/~cdgill/courses/cs6874/TimingWheels.ppt.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1108", Justification = "Justified!")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Justified!")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Justified!")]
    public class HashedWheelTimerScheduler : SchedulerBase, IDisposable
    {
        private const int WORKER_STATE_INIT = 0;
        private const int WORKER_STATE_STARTED = 1;
        private const int WORKER_STATE_SHUTDOWN = 2;

        private readonly int mask;
        private readonly CountdownEvent workerInitialized = new CountdownEvent(1);
        private readonly ConcurrentQueue<SchedulerRegistration> registrations = new ConcurrentQueue<SchedulerRegistration>();
        private readonly Bucket[] wheel;

        private readonly TimeSpan shutdownTimeout;
        private readonly long tickDuration;

        private readonly HashSet<SchedulerRegistration> unprocessedRegistrations = new HashSet<SchedulerRegistration>();
        private readonly HashSet<SchedulerRegistration> rescheduleRegistrations = new HashSet<SchedulerRegistration>();

        private Thread worker;
        private long startTime = 0;
        private long tick;

        /// <summary>
        /// 0 - INIT, 1 - STARTED, 2 - SHUTDOWN.
        /// </summary>
        private volatile int workerState = WORKER_STATE_INIT;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedWheelTimerScheduler"/> class.
        /// </summary>
        /// <param name="log">The logger.</param>
        public HashedWheelTimerScheduler(ILoggingAdapter log)
            : base(log)
        {
            var ticksPerWheel = 10; // SchedulerConfig.GetInt("akka.scheduler.ticks-per-wheel");
            var tickDurationTimeSpan = TimeSpan.FromMilliseconds(10);
            if (tickDurationTimeSpan.TotalMilliseconds < 10.0d)
            {
                // "minimum supported akka.scheduler.tick-duration on Windows is 10ms"
                throw new ArgumentOutOfRangeException(nameof(tickDurationTimeSpan));
            }

            // convert tick-duration to ticks
            this.tickDuration = tickDurationTimeSpan.Ticks;

            // Normalize ticks per wheel to power of two and create the wheel
            this.wheel = CreateWheel(ticksPerWheel, log);
            this.mask = this.wheel.Length - 1;

            // prevent overflow
            if (this.tickDuration >= long.MaxValue / this.wheel.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(this.tickDuration),
                    this.tickDuration,
                    $"akka.scheduler.tick-duration: {this.tickDuration} (expected: 0 < tick-duration in ticks < {long.MaxValue / this.wheel.Length}");
            }

            this.shutdownTimeout = TimeSpan.Zero; // SchedulerConfig.GetTimeSpan("akka.scheduler.shutdown-timeout");
        }

        /// <summary>
        /// Gets the elapsed time since start.
        /// </summary>
        public override TimeSpan Elapsed => MonotonicClock.Elapsed;

        /// <summary>
        /// Gets the elapsed time since start high resolution.
        /// </summary>
        public override TimeSpan ElapsedHighRes => MonotonicClock.ElapsedHighRes;

        /// <summary>
        /// Gets the time now.
        /// </summary>
        ///
        protected override DateTimeOffset TimeNow => DateTimeOffset.Now;

        private static Bucket[] CreateWheel(int ticksPerWheel, ILoggingAdapter log)
        {
            if (ticksPerWheel <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticksPerWheel), ticksPerWheel, "Must be greater than 0.");
            }

            if (ticksPerWheel > 1073741824)
            {
                throw new ArgumentOutOfRangeException(nameof(ticksPerWheel), ticksPerWheel, "Cannot be greater than 2^30.");
            }

            ticksPerWheel = NormalizeTicksPerWheel(ticksPerWheel);
            var wheel = new Bucket[ticksPerWheel];
            for (var i = 0; i < wheel.Length; i++)
            {
                wheel[i] = new Bucket(log);
            }

            return wheel;
        }

        /// <summary>
        /// Normalize a wheel size to the nearest power of 2.
        /// </summary>
        /// <param name="ticksPerWheel">The original input per wheel.</param>
        /// <returns><paramref name="ticksPerWheel"/> normalized to the nearest power of 2.</returns>
        private static int NormalizeTicksPerWheel(int ticksPerWheel)
        {
            var normalizedTicksPerWheel = 1;
            while (normalizedTicksPerWheel < ticksPerWheel)
            {
                normalizedTicksPerWheel <<= 1;
            }

            return normalizedTicksPerWheel;
        }

        private void Start()
        {
            switch (this.workerState)
            {
                case WORKER_STATE_STARTED:
                    break; // Do nothing.
                case WORKER_STATE_INIT:
                {
                    this.worker = new Thread(this.Run) { IsBackground = true };

                    if (Interlocked.CompareExchange(ref this.workerState, WORKER_STATE_STARTED, WORKER_STATE_INIT) == WORKER_STATE_INIT)
                    {
                        this.worker.Start();
                    }

                    break;
                }

                case WORKER_STATE_SHUTDOWN:
                    throw new SchedulerException("cannot enqueue after timer shutdown");
                default:
                    throw new InvalidOperationException($"Worker in invalid state: {this.workerState}");
            }

            while (this.startTime == 0)
            {
                this.workerInitialized.Wait();
            }
        }

        /// <summary>
        /// Scheduler thread entry method.
        /// </summary>
        private void Run()
        {
            // Initialize the clock
            this.startTime = this.ElapsedHighRes.Ticks;
            if (this.startTime == 0)
            {
                // 0 means it's an uninitialized value, so bump to 1 to indicate it's started
                this.startTime = 1;
            }

            this.workerInitialized.Signal();

            do
            {
                var deadline = this.WaitForNextTick();
                if (deadline > 0)
                {
                    var idx = (int)(this.tick & this.mask);
                    var bucket = this.wheel[idx];
                    this.TransferRegistrationsToBuckets();
                    bucket.Execute(deadline);
                    this.tick++; // it will take 2^64 * 10ms for this to overflow

                    bucket.ClearReschedule(this.rescheduleRegistrations);
                    this.ProcessReschedule();
                }
            }
            while (this.workerState == WORKER_STATE_STARTED);

            // Empty all of the buckets
            foreach (var bucket in this.wheel)
            {
                bucket.ClearRegistrations(this.unprocessedRegistrations);
            }

            // Empty tasks that haven't been placed into a bucket yet
            foreach (var reg in this.registrations)
            {
                if (!reg.Cancelled)
                {
                    this.unprocessedRegistrations.Add(reg);
                }
            }

            // Return the list of unprocessedRegistrations and signal that we're finished
            this.stopped.Value.TrySetResult(this.unprocessedRegistrations);
        }

        private void ProcessReschedule()
        {
            foreach (var toReschedule in this.rescheduleRegistrations)
            {
                var nextDeadline = this.ElapsedHighRes.Ticks - this.startTime + toReschedule.Offset;
                toReschedule.Deadline = nextDeadline;
                this.PlaceInBucket(toReschedule);
            }

            this.rescheduleRegistrations.Clear();
        }

        private long WaitForNextTick()
        {
            var deadline = this.tickDuration * (this.tick + 1);
            unchecked // just to avoid trouble with long-running applications
            {
                for (; ;)
                {
                    var currentTime = this.ElapsedHighRes.Ticks - this.startTime;
                    var sleepMs = (deadline - currentTime + TimeSpan.TicksPerMillisecond - 1) / TimeSpan.TicksPerMillisecond;

                    if (sleepMs <= 0) // No need to sleep.
                    {
                        if (currentTime == long.MinValue) // wrap-around
                        {
                            return -long.MaxValue;
                        }

                        return currentTime;
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(sleepMs));
                }
            }
        }

        private void TransferRegistrationsToBuckets()
        {
            // Transfer only max. 100000 registrations per tick to prevent a thread to stale the workerThread when it just
            // adds new timeouts in a loop.
            for (var i = 0; i < 100000; i++)
            {
                if (!this.registrations.TryDequeue(out var registration))
                {
                    // all processed
                    break;
                }

                if (registration.Cancelled)
                {
                    // cancelled before we could process it
                    continue;
                }

                this.PlaceInBucket(registration);
            }
        }

        private void PlaceInBucket(SchedulerRegistration reg)
        {
            var calculated = reg.Deadline / this.tickDuration;
            reg.RemainingRounds = (calculated - this.tick) / this.wheel.Length;

            var ticks = Math.Max(calculated, this.tick); // Ensure we don't schedule for the past
            var stopIndex = (int)(ticks & this.mask);

            var bucket = this.wheel[stopIndex];
            bucket.AddRegistration(reg);
        }




        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="delay">TBD</param>
        /// <param name="receiver">TBD</param>
        /// <param name="message">TBD</param>
        /// <param name="sender">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected override void InternalScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender,
                    ICancelable cancelable)
        {
            InternalSchedule(delay, TimeSpan.Zero, new ScheduledTell(receiver, message, sender), cancelable);
        }

        private void InternalSchedule(TimeSpan delay, TimeSpan interval, IRunnable action, ICancelable cancelable)
        {
            Start();
            var deadline = ElapsedHighRes.Ticks + delay.Ticks - this.startTime;
            var offset = interval.Ticks;
            var reg = new SchedulerRegistration(action, cancelable)
            {
                Deadline = deadline,
                Offset = offset
            };
            this.registrations.Enqueue(reg);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="initialDelay">TBD</param>
        /// <param name="interval">TBD</param>
        /// <param name="receiver">TBD</param>
        /// <param name="message">TBD</param>
        /// <param name="sender">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected override void InternalScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message,
            IEndpoint sender, ICancelable cancelable)
        {
            InternalSchedule(initialDelay, interval, new ScheduledTell(receiver, message, sender), cancelable);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="delay">TBD</param>
        /// <param name="action">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected override void InternalScheduleOnce(TimeSpan delay, Action action, ICancelable cancelable)
        {
            InternalSchedule(delay, TimeSpan.Zero, new ActionRunnable(action), cancelable);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="initialDelay">TBD</param>
        /// <param name="interval">TBD</param>
        /// <param name="action">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected override void InternalScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action, ICancelable cancelable)
        {
            InternalSchedule(initialDelay, interval, new ActionRunnable(action), cancelable);
        }

        private AtomicReference<TaskCompletionSource<IEnumerable<SchedulerRegistration>>> stopped = new AtomicReference<TaskCompletionSource<IEnumerable<SchedulerRegistration>>>();

        private static readonly Task<IEnumerable<SchedulerRegistration>> Completed = Task.FromResult((IEnumerable<SchedulerRegistration>)new List<SchedulerRegistration>());

        private Task<IEnumerable<SchedulerRegistration>> Stop()
        {
            var p = new TaskCompletionSource<IEnumerable<SchedulerRegistration>>();

            if (this.stopped.CompareAndSet(null, p) && Interlocked.CompareExchange(ref this.workerState, WORKER_STATE_SHUTDOWN, WORKER_STATE_STARTED) == WORKER_STATE_STARTED)
            {
                // Let remaining work that is already being processed finished. The termination task will complete afterwards.
                return p.Task;
            }

            return Completed;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var stopped = Stop();
            if (!stopped.Wait(this.shutdownTimeout))
            {
                this.Log.Warning(NautilusService.Scheduling, $"Failed to shutdown scheduler within { this.shutdownTimeout}");
                return;
            }

            // Execute all outstanding work items
            foreach (var task in stopped.Result)
            {
                try
                {
                    task.Action.Run();
                }
                catch (SchedulerException)
                {
                    // ignore, this is from terminated actors
                }
                catch (Exception ex)
                {
                    this.Log.Error(NautilusService.Scheduling, "Exception while executing timer task.", ex);
                }
                finally
                {
                    // free the object from bucket
                    task.Reset();
                }
            }

            this.unprocessedRegistrations.Clear();
        }

        /// <summary>
        /// INTERNAL API.
        /// </summary>
        private sealed class ScheduledTell : IRunnable
        {
            private readonly IEndpoint _receiver;
            private readonly object _message;
            private readonly IEndpoint _sender;

            public ScheduledTell(IEndpoint receiver, object message, IEndpoint sender)
            {
                _receiver = receiver;
                _message = message;
                _sender = sender;
            }

            public void Run()
            {
                _receiver.Send(_message);
            }

            public override string ToString()
            {
                return $"[{_receiver}.Tell({_message}, {_sender})]";
            }
        }

        private class SchedulerRegistration
        {
            /// <summary>
            /// The cancellation handle, if any
            /// </summary>
            public readonly ICancelable Cancellation;

            /// <summary>
            /// The task to be executed
            /// </summary>
            public readonly IRunnable Action;

            /*
             * Linked list is only ever modified from the scheduler thread, so this
             * implementation does not need to be synchronized or implement CAS semantics.
             */
            public SchedulerRegistration Next;
            public SchedulerRegistration Prev;

            public long RemainingRounds;

            /// <summary>
            /// Used to determine the delay for repeatable sends
            /// </summary>
            public long Offset;

            /// <summary>
            /// The deadline for determining when to execute
            /// </summary>
            public long Deadline;

            public SchedulerRegistration(IRunnable action, ICancelable cancellation)
            {
                Action = action;
                Cancellation = cancellation;
            }

            /// <summary>
            /// Determines if this task will need to be re-scheduled according to its <see cref="Offset"/>.
            /// </summary>
            public bool Repeat => Offset > 0;

            /// <summary>
            /// If <c>true</c>, we skip execution of this task.
            /// </summary>
            public bool Cancelled => Cancellation?.IsCancellationRequested ?? false;

            /// <summary>
            /// The <see cref="Bucket"/> to which this registration belongs.
            /// </summary>
            public Bucket Bucket;

            /// <summary>
            /// Resets all of the fields so this registration object can be used again
            /// </summary>
            public void Reset()
            {
                Next = null;
                Prev = null;
                Bucket = null;
                Deadline = 0;
                RemainingRounds = 0;
            }

            public override string ToString()
            {
                return
                    $"ScheduledWork(Deadline={Deadline}, RepeatEvery={Offset}, Cancelled={Cancelled}, Work={Action})";
            }
        }

        private sealed class Bucket
        {
            private readonly ILoggingAdapter _log;

            /*
             * Endpoints of our doubly linked list
             */
            private SchedulerRegistration head;
            private SchedulerRegistration tail;

            private SchedulerRegistration rescheduleHead;
            private SchedulerRegistration rescheduleTail;

            public Bucket(ILoggingAdapter log)
            {
                _log = log;
            }

            /// <summary>
            /// Adds a <see cref="SchedulerRegistration"/> to this bucket.
            /// </summary>
            /// <param name="reg">The scheduled task.</param>
            public void AddRegistration(SchedulerRegistration reg)
            {
                System.Diagnostics.Debug.Assert(reg.Bucket == null);
                reg.Bucket = this;
                if (this.head == null) // first time the bucket has been used
                {
                    this.head = this.tail = reg;
                }
                else
                {
                    this.tail.Next = reg;
                    reg.Prev = this.tail;
                    this.tail = reg;
                }
            }

            /// <summary>
            /// Slot a repeating task into the "reschedule" linked list.
            /// </summary>
            /// <param name="reg">The registration scheduled for repeating</param>
            public void Reschedule(SchedulerRegistration reg)
            {
                if (this.rescheduleHead == null)
                {
                    this.rescheduleHead = this.rescheduleTail = reg;
                }
                else
                {
                    this.rescheduleTail.Next = reg;
                    reg.Prev = this.rescheduleTail;
                    this.rescheduleTail = reg;
                }
            }

            /// <summary>
            /// Empty this bucket
            /// </summary>
            /// <param name="registrations">A set of registrations to populate.</param>
            public void ClearRegistrations(HashSet<SchedulerRegistration> registrations)
            {
                for (;;)
                {
                    var reg = Poll();
                    if (reg == null)
                        return;
                    if (reg.Cancelled)
                        continue;
                    registrations.Add(reg);
                }
            }

            /// <summary>
            /// Reset the reschedule list for this bucket
            /// </summary>
            /// <param name="registrations">A set of registrations to populate.</param>
            public void ClearReschedule(HashSet<SchedulerRegistration> registrations)
            {
                for (;;)
                {
                    var reg = PollReschedule();
                    if (reg == null)
                        return;
                    if (reg.Cancelled)
                        continue;
                    registrations.Add(reg);
                }
            }

            private static readonly Action<object> ExecuteRunnableWithState = r => ((IRunnable)r).Run();

            /// <summary>
            /// Execute all <see cref="SchedulerRegistration"/>s that are due by or after <paramref name="deadline"/>.
            /// </summary>
            /// <param name="deadline">The execution time.</param>
            public void Execute(long deadline)
            {
                var current = this.head;

                // process all registrations
                while (current != null)
                {
                    bool remove = false;
                    if (current.Cancelled) // check for cancellation first
                    {
                        remove = true;
                    }
                    else if (current.RemainingRounds <= 0)
                    {
                        if (current.Deadline <= deadline)
                        {
                            try
                            {
                                // Execute the scheduled work
                                current.Action.Run();
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    _log.Error(NautilusService.Scheduling, $"Error while executing scheduled task {current}", ex);
                                    var nextErrored = current.Next;
                                    this.Remove(current);
                                    current = nextErrored;
                                    continue; // don't reschedule any failed actions
                                }
                                catch
                                {
                                } // suppress any errors thrown during logging
                            }

                            remove = true;
                        }
                        else
                        {
                            // Registration was placed into the wrong bucket. This should never happen.
                            throw new InvalidOperationException(
                                $"SchedulerRegistration.Deadline [{current.Deadline}] > Timer.Deadline [{deadline}]");
                        }
                    }
                    else
                    {
                        current.RemainingRounds--;
                    }

                    var next = current.Next;
                    if (remove)
                    {
                        this.Remove(current);
                    }

                    if (current.Repeat && remove)
                    {
                        this.Reschedule(current);
                    }

                    current = next;
                }
            }

            public void Remove(SchedulerRegistration reg)
            {
                var next = reg.Next;

                // Remove work that's already been completed or cancelled
                // Work that is scheduled to repeat will be handled separately
                if (reg.Prev != null)
                {
                    reg.Prev.Next = next;
                }

                if (reg.Next != null)
                {
                    reg.Next.Prev = reg.Prev;
                }

                if (reg == this.head)
                {
                    // need to adjust the ends
                    if (reg == this.tail)
                    {
                        this.tail = null;
                        this.head = null;
                    }
                    else
                    {
                        this.head = next;
                    }
                }
                else if (reg == this.tail)
                {
                    this.tail = reg.Prev;
                }

                // detach the node from Linked list so it can be GCed
                reg.Reset();
            }

            private SchedulerRegistration Poll()
            {
                var thisHead = this.head;
                if (thisHead == null)
                {
                    return null;
                }

                var next = thisHead.Next;
                if (next == null)
                {
                    this.tail = this.head = null;
                }
                else
                {
                    this.head = next;
                    next.Prev = null;
                }

                thisHead.Reset();
                return thisHead;
            }

            private SchedulerRegistration PollReschedule()
            {
                var thisHead = this.rescheduleHead;
                if (thisHead == null)
                {
                    return null;
                }

                var next = thisHead.Next;
                if (next == null)
                {
                    this.rescheduleTail = this.rescheduleHead = null;
                }
                else
                {
                    this.rescheduleHead = next;
                    next.Prev = null;
                }

                thisHead.Reset();
                return thisHead;
            }
        }
    }
}

