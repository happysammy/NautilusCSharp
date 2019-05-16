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
    using Nautilus.Core;
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
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Capitalization easier to read.")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Underscores easier to read.")]
    public class HashedWheelTimerScheduler : SchedulerBase, IDisposable
    {
        private const int WORKER_STATE_INIT = 0;
        private const int WORKER_STATE_STARTED = 1;
        private const int WORKER_STATE_SHUTDOWN = 2;

        private readonly AtomicReference<TaskCompletionSource<IEnumerable<SchedulerRegistration>>> stopped = new AtomicReference<TaskCompletionSource<IEnumerable<SchedulerRegistration>>>();
        private readonly ConcurrentQueue<SchedulerRegistration> registrations = new ConcurrentQueue<SchedulerRegistration>();
        private readonly HashSet<SchedulerRegistration> unprocessedRegistrations = new HashSet<SchedulerRegistration>();
        private readonly HashSet<SchedulerRegistration> rescheduleRegistrations = new HashSet<SchedulerRegistration>();
        private readonly CountdownEvent workerInitialized = new CountdownEvent(1);
        private readonly TimeSpan shutdownTimeout = TimeSpan.FromSeconds(1);
        private readonly Bucket[] wheel;
        private readonly long tickDuration;
        private readonly int mask;

        private Thread? worker;
        private long startTime;
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
            var ticksPerWheel = 512; // Default.
            var tickDurationTimeSpan = TimeSpan.FromMilliseconds(10);
            if (tickDurationTimeSpan.TotalMilliseconds < 10.0d)
            {
                // "minimum supported on Windows is 10ms"
                throw new ArgumentOutOfRangeException(nameof(tickDurationTimeSpan) + "Cannot be less than 10ms.");
            }

            // Normalize ticks per wheel to power of two and create the wheel.
            this.wheel = CreateWheel(ticksPerWheel, log);
            this.mask = this.wheel.Length - 1;

            // Convert tick-duration to ticks.
            this.tickDuration = tickDurationTimeSpan.Ticks;

            // Prevent overflow.
            if (this.tickDuration >= long.MaxValue / this.wheel.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(this.tickDuration),
                    this.tickDuration,
                    $"tickDuration: {this.tickDuration} (expected: 0 < tick-duration in ticks < {long.MaxValue / this.wheel.Length}");
            }
        }

        /// <summary>
        /// Gets the elapsed time since start high resolution.
        /// </summary>
        public override TimeSpan Elapsed => MonotonicClock.Elapsed;

        /// <summary>
        /// Gets the time now.
        /// </summary>
        ///
        protected override DateTimeOffset TimeNow => DateTimeOffset.Now;

        /// <summary>
        /// Starts the <see cref="HashedWheelTimerScheduler"/>.
        /// </summary>
        /// <exception cref="SchedulerException">If the worker state is already shutdown.</exception>
        /// <exception cref="InvalidOperationException">If the worker state is invalid.</exception>
        public void Start()
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
                    throw new SchedulerException("Cannot enqueue after timer shutdown.");
                default:
                    throw new InvalidOperationException($"Worker in invalid state: {this.workerState}.");
            }

            while (this.startTime == 0)
            {
                this.workerInitialized.Wait();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var stoppedTask = this.Stop();
            if (!stoppedTask.Wait(this.shutdownTimeout))
            {
                this.Log.Warning(NautilusService.Scheduling, $"Failed to shutdown scheduler within {this.shutdownTimeout}");
                return;
            }

            // Execute all outstanding work items.
            foreach (var task in stoppedTask.Result)
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
        /// Schedule a message to be sent once.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected override void InternalScheduleSendOnce(
            TimeSpan delay,
            IEndpoint receiver,
            object message,
            IEndpoint sender,
            OptionRef<ICancelable> cancelable)
        {
            this.InternalSchedule(delay, TimeSpan.Zero, new ScheduledSend(receiver, message, sender), cancelable);
        }

        /// <summary>
        /// Schedule a message to be sent repeatedly.
        /// </summary>
        /// <param name="initialDelay">The delay.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected override void InternalScheduleSendRepeatedly(
            TimeSpan initialDelay,
            TimeSpan interval,
            IEndpoint receiver,
            object message,
            IEndpoint sender,
            OptionRef<ICancelable> cancelable)
        {
            this.InternalSchedule(initialDelay, interval, new ScheduledSend(receiver, message, sender), cancelable);
        }

        /// <summary>
        /// Schedule an action to be invoked once.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected override void InternalScheduleOnce(
            TimeSpan delay,
            Action action,
            OptionRef<ICancelable> cancelable)
        {
            this.InternalSchedule(delay, TimeSpan.Zero, new ActionRunnable(action), cancelable);
        }

        /// <summary>
        /// Schedule an action to be invoked repeatedly.
        /// </summary>
        /// <param name="initialDelay">The initial delay.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="action">The action to schedule.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected override void InternalScheduleRepeatedly(
            TimeSpan initialDelay,
            TimeSpan interval,
            Action action,
            OptionRef<ICancelable> cancelable)
        {
            this.InternalSchedule(initialDelay, interval, new ActionRunnable(action), cancelable);
        }

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

        /// <summary>
        /// Scheduler thread entry method.
        /// </summary>
        private void Run()
        {
            // Initialize the clock.
            this.startTime = this.Elapsed.Ticks;
            if (this.startTime == 0)
            {
                // 0 means it's an uninitialized value, so bump to 1 to indicate it's started.
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
                    this.tick++; // it will take 2^64 * 10ms for this to overflow.

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
            this.stopped.Value?.TrySetResult(this.unprocessedRegistrations);
        }

        private void ProcessReschedule()
        {
            foreach (var toReschedule in this.rescheduleRegistrations)
            {
                var nextDeadline = this.Elapsed.Ticks - this.startTime + toReschedule.Offset;
                toReschedule.Deadline = nextDeadline;
                this.PlaceInBucket(toReschedule);
            }

            this.rescheduleRegistrations.Clear();
        }

        private long WaitForNextTick()
        {
            var deadline = this.tickDuration * (this.tick + 1);

            // unchecked avoids trouble with long-running applications.
            unchecked
            {
                for (; ;)
                {
                    var currentTime = this.Elapsed.Ticks - this.startTime;
                    var sleepMs = (deadline - currentTime + TimeSpan.TicksPerMillisecond - 1) / TimeSpan.TicksPerMillisecond;

                    if (sleepMs <= 0)
                    {
                        // No need to sleep.
                        if (currentTime == long.MinValue)
                        {
                            // Wrap-around.
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
                    break;  // All processed.
                }

                if (registration.Cancelled)
                {
                    continue;  // Cancelled before we could process it.
                }

                this.PlaceInBucket(registration);
            }
        }

        private void PlaceInBucket(SchedulerRegistration reg)
        {
            var calculated = reg.Deadline / this.tickDuration;
            reg.RemainingRounds = (calculated - this.tick) / this.wheel.Length;

            var ticks = Math.Max(calculated, this.tick); // Ensure we don't schedule for the past.
            var stopIndex = (int)(ticks & this.mask);

            var bucket = this.wheel[stopIndex];
            bucket.AddRegistration(reg);
        }

        private void InternalSchedule(
            TimeSpan delay,
            TimeSpan interval,
            IRunnable action,
            OptionRef<ICancelable> cancelable)
        {
            this.Start();
            var deadline = this.Elapsed.Ticks + delay.Ticks - this.startTime;
            var offset = interval.Ticks;
            var reg = new SchedulerRegistration(action, cancelable)
            {
                Deadline = deadline,
                Offset = offset,
            };
            this.registrations.Enqueue(reg);
        }

        private Task<IEnumerable<SchedulerRegistration>> Stop()
        {
            var p = new TaskCompletionSource<IEnumerable<SchedulerRegistration>>();
            if (this.stopped.CompareAndSet(null, p) && Interlocked.CompareExchange(ref this.workerState, WORKER_STATE_SHUTDOWN, WORKER_STATE_STARTED) == WORKER_STATE_STARTED)
            {
                // Let remaining work that is already being processed finished. The termination task will complete afterwards.
                return p.Task;
            }

            return Task.FromResult((IEnumerable<SchedulerRegistration>)new List<SchedulerRegistration>());
        }
    }
}
