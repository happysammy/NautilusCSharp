// -------------------------------------------------------------------------------------------------
// <copyright file="HashedWheelTimerScheduler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler.Internal;
    using NodaTime;

    /// <summary>
    /// This <see cref="IScheduler"/> implementation is built using a revolving wheel of buckets
    /// with each bucket belonging to a specific time resolution. As the "clock" of the scheduler
    /// ticks it advances to the next bucket in the circle and processes the items in it, and
    /// optionally reschedules recurring tasks into the future into the next relevant bucket.
    ///
    /// There are 512 initial buckets as default (we round up to the nearest power of 2).
    /// The timings are approximated and are still limited by the ceiling of the operating
    /// system's clock resolution.
    ///
    /// Further reading: http://www.cs.columbia.edu/~nahum/w6998/papers/sosp87-timing-wheels.pdf
    /// Presentation: http://www.cse.wustl.edu/~cdgill/courses/cs6874/TimingWheels.ppt.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public sealed class HashedWheelTimerScheduler : Component, IScheduler, IDisposable
    {
        private const int DEFAULT_TICKS_PER_WHEEL = 512;
        private const int TWO_POWER_30 = 1073741824;
        private const int MIN_TICK_DURATION_MS = 10;
        private const int MAX_REGISTRATIONS_PER_TICK = 100000;
        private const int TICKS_PER_MILLISECOND = 10000;
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
        /// <param name="container">The componentry container.</param>
        public HashedWheelTimerScheduler(IComponentryContainer container)
            : base(container)
        {
            this.wheel = CreateWheel(DEFAULT_TICKS_PER_WHEEL, this.Log);
            this.mask = this.wheel.Length - 1;
            this.tickDuration = DurationToTicksWithCheck(Duration.FromMilliseconds(MIN_TICK_DURATION_MS), this.wheel.Length);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var stoppedTask = this.OnStop();
            if (!stoppedTask.Wait(this.shutdownTimeout))
            {
                this.Log.Warning($"Failed to shutdown scheduler within {this.shutdownTimeout}");
                return;
            }

            // Execute all outstanding work items
            foreach (var task in stoppedTask.Result)
            {
                try
                {
                    task.Action.Run();
                }
                catch (Exception ex)
                {
                    this.Log.Error("Exception while executing timer task.", ex);
                }
                finally
                {
                    // Free the object from bucket
                    task.Reset();
                }
            }

            this.unprocessedRegistrations.Clear();
        }

        /// <inheritdoc />
        public void ScheduleOnce(Duration delay, Action action)
        {
            Condition.NotNegativeInt32((int)delay.TotalMilliseconds, nameof(delay.TotalMilliseconds));

            this.InternalSchedule(delay, Duration.Zero, new ActionRunnable(action), null);
        }

        /// <inheritdoc />
        public void ScheduleOnce(ZonedDateTime forTime, Action action)
        {
            Condition.True(forTime.IsGreaterThan(this.TimeNow()), "Given for time > time now.");

            this.InternalSchedule(forTime - this.TimeNow(), Duration.Zero, new ActionRunnable(action), null);
        }

        /// <inheritdoc />
        public void ScheduleRepeatedly(Duration initialDelay, Duration interval, Action action)
        {
            Condition.NotNegativeInt32((int)initialDelay.TotalMilliseconds, nameof(initialDelay.TotalMilliseconds));
            Condition.PositiveInt32((int)interval.TotalMilliseconds, nameof(interval.TotalMilliseconds));

            this.InternalSchedule(initialDelay, interval, new ActionRunnable(action), null);
        }

        /// <inheritdoc />
        public void ScheduleSendOnce(Duration delay, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.NotNegativeInt32((int)delay.TotalMilliseconds, nameof(delay.TotalMilliseconds));

            this.InternalSchedule(delay, Duration.Zero, new ScheduledSend(receiver, message, sender), null);
        }

        /// <inheritdoc />
        public void ScheduleSendOnce(ZonedDateTime forTime, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.True(forTime.IsGreaterThan(this.TimeNow()), "Given for time > time now.");

            this.InternalSchedule(forTime - this.TimeNow(), Duration.Zero, new ScheduledSend(receiver, message, sender), null);
        }

        /// <inheritdoc />
        public void ScheduleSendRepeatedly(Duration initialDelay, Duration interval, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.NotNegativeInt32((int)initialDelay.TotalMilliseconds, nameof(initialDelay.TotalMilliseconds));
            Condition.PositiveInt32((int)interval.TotalMilliseconds, nameof(interval.TotalMilliseconds));

            this.InternalSchedule(initialDelay, interval, new ScheduledSend(receiver, message, sender), null);
        }

        /// <inheritdoc />
        public ICancelable ScheduleOnceCancelable(Duration delay, Action action)
        {
            Condition.NotNegativeInt32((int)delay.TotalMilliseconds, nameof(delay.TotalMilliseconds));

            var cancelable = new Cancelable(this);
            this.InternalSchedule(delay, Duration.Zero, new ActionRunnable(action), cancelable);
            return cancelable;
        }

        /// <inheritdoc />
        public ICancelable ScheduleOnceCancelable(ZonedDateTime forTime, Action action)
        {
            Condition.True(forTime.IsGreaterThan(this.TimeNow()), "Given for time > time now.");

            var cancelable = new Cancelable(this);
            this.InternalSchedule(forTime - this.TimeNow(), Duration.Zero, new ActionRunnable(action), cancelable);
            return cancelable;
        }

        /// <inheritdoc />
        public ICancelable ScheduleRepeatedlyCancelable(Duration initialDelay, Duration interval, Action action)
        {
            Condition.NotNegativeInt32((int)initialDelay.TotalMilliseconds, nameof(initialDelay.TotalMilliseconds));
            Condition.PositiveInt32((int)interval.TotalMilliseconds, nameof(interval.TotalMilliseconds));

            var cancelable = new Cancelable(this);
            this.InternalSchedule(initialDelay, interval, new ActionRunnable(action), cancelable);
            return cancelable;
        }

        /// <inheritdoc />
        public ICancelable ScheduleSendOnceCancelable(Duration delay, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.NotNegativeInt32((int)delay.TotalMilliseconds, nameof(delay.TotalMilliseconds));

            var cancelable = new Cancelable(this);
            this.InternalSchedule(delay, Duration.Zero, new ScheduledSend(receiver, message, sender), cancelable);
            return cancelable;
        }

        /// <inheritdoc />
        public ICancelable ScheduleSendOnceCancelable(ZonedDateTime forTime, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.True(forTime.IsGreaterThan(this.TimeNow()), "Given for time > time now.");

            var cancelable = new Cancelable(this);
            this.InternalSchedule(forTime - this.TimeNow(), Duration.Zero, new ScheduledSend(receiver, message, sender), cancelable);
            return cancelable;
        }

        /// <inheritdoc />
        public ICancelable ScheduleSendRepeatedlyCancelable(Duration initialDelay, Duration interval, IEndpoint receiver, object message, IEndpoint sender)
        {
            Condition.NotNegativeInt32((int)initialDelay.TotalMilliseconds, nameof(initialDelay.TotalMilliseconds));
            Condition.PositiveInt32((int)interval.TotalMilliseconds, nameof(interval.TotalMilliseconds));

            var cancelable = new Cancelable(this);
            this.InternalSchedule(initialDelay, interval, new ScheduledSend(receiver, message, sender), cancelable);
            return cancelable;
        }

        private static long DurationToTicksWithCheck(Duration ticksDuration, long wheelLength)
        {
            var ticksPerTock = ticksDuration.BclCompatibleTicks;
            var upperBound = (long.MaxValue / wheelLength) - 1;
            Condition.NotOutOfRangeInt64(ticksPerTock, 10, upperBound, nameof(ticksPerTock));

            return ticksPerTock;
        }

        private static Bucket[] CreateWheel(int ticksPerWheel, ILogger log)
        {
            Condition.NotOutOfRangeInt32(ticksPerWheel, 1, TWO_POWER_30, nameof(ticksPerWheel));

            ticksPerWheel = NormalizeToPowerOf2(ticksPerWheel);
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
        private static int NormalizeToPowerOf2(int ticksPerWheel)
        {
            var normalizedTicksPerWheel = 1;
            while (normalizedTicksPerWheel < ticksPerWheel)
            {
                normalizedTicksPerWheel <<= 1;
            }

            return normalizedTicksPerWheel;
        }

        /// <summary>
        /// Starts the <see cref="HashedWheelTimerScheduler"/>.
        /// </summary>
        /// <exception cref="SchedulerException">If the worker state is already shutdown.</exception>
        /// <exception cref="InvalidOperationException">If the worker state is invalid.</exception>
        private void OnStart()
        {
            switch (this.workerState)
            {
                case WORKER_STATE_STARTED:
                    break; // Do nothing
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
                    throw ExceptionFactory.InvalidSwitchArgument(this.workerState, nameof(this.workerState));
            }

            while (this.startTime == 0)
            {
                this.workerInitialized.Wait();
            }
        }

        /// <summary>
        /// Scheduler thread entry method.
        /// </summary>
        [PerformanceOptimized]
        private void Run()
        {
            // Initialize the clock.
            this.startTime = MonotonicClock.Elapsed.BclCompatibleTicks;
            if (this.startTime == 0)
            {
                // Uninitialized value, so bump to 1 to indicate it's started
                this.startTime = 1;
            }

            this.workerInitialized.Signal();

            while (this.workerState == WORKER_STATE_STARTED)
            {
                var deadline = this.WaitForNextTick();
                if (deadline <= 0)
                {
                    continue;
                }

                var idx = (int)(this.tick & this.mask);
                var bucket = this.wheel[idx];
                this.TransferRegistrationsToBuckets();
                bucket.Execute(deadline);
                this.tick++; // It will take 2^64 * 10ms for this to overflow (a long time)

                bucket.ClearReschedule(this.rescheduleRegistrations);
                this.ProcessReschedule();
            }

            // Empty all of the buckets
            for (var i = 0; i < this.wheel.Length; i++)
            {
                this.wheel[i].ClearRegistrations(this.unprocessedRegistrations);
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
                var nextDeadline = MonotonicClock.Elapsed.BclCompatibleTicks - this.startTime + toReschedule.Offset;
                toReschedule.Deadline = nextDeadline;
                this.PlaceInBucket(toReschedule);
            }

            this.rescheduleRegistrations.Clear();
        }

        private long WaitForNextTick()
        {
            var deadline = this.tickDuration * (this.tick + 1);

            // Unchecked avoids System.OutOfMemoryException for long-running applications
            unchecked
            {
                while (true)
                {
                    var currentTime = MonotonicClock.Elapsed.BclCompatibleTicks - this.startTime;
                    var sleepMs = (deadline - currentTime + TICKS_PER_MILLISECOND - 1) / TICKS_PER_MILLISECOND;

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
            // Transfer only max prevents a stale thread where its just adding new timeouts in a loop
            for (var i = 0; i < MAX_REGISTRATIONS_PER_TICK; i++)
            {
                if (!this.registrations.TryDequeue(out var registration))
                {
                    break;  // All processed.
                }

                if (registration.Cancelled)
                {
                    continue;  // Cancelled before processing
                }

                this.PlaceInBucket(registration);
            }
        }

        private void PlaceInBucket(SchedulerRegistration reg)
        {
            var calculated = reg.Deadline / this.tickDuration;
            reg.RemainingRounds = (calculated - this.tick) / this.wheel.Length;

            var ticks = Math.Max(calculated, this.tick); // Ensures not scheduled for the past
            var stopIndex = (int)(ticks & this.mask);

            var bucket = this.wheel[stopIndex];
            bucket.AddRegistration(reg);
        }

        private void InternalSchedule(
            Duration delay,
            Duration interval,
            IRunnable action,
            ICancelable? cancelable)
        {
            this.OnStart();
            var deadline = MonotonicClock.Elapsed.BclCompatibleTicks + delay.BclCompatibleTicks - this.startTime;
            var offset = interval.BclCompatibleTicks;
            var reg = new SchedulerRegistration(action, cancelable)
            {
                Deadline = deadline,
                Offset = offset,
            };
            this.registrations.Enqueue(reg);
        }

        private Task<IEnumerable<SchedulerRegistration>> OnStop()
        {
            var p = new TaskCompletionSource<IEnumerable<SchedulerRegistration>>();
            if (this.stopped.CompareAndSet(null, p) && Interlocked.CompareExchange(ref this.workerState, WORKER_STATE_SHUTDOWN, WORKER_STATE_STARTED) == WORKER_STATE_STARTED)
            {
                // Let remaining work that is already being processed finish.
                // The termination task will complete afterwards.
                return p.Task;
            }

            return Task.FromResult((IEnumerable<SchedulerRegistration>)new List<SchedulerRegistration>());
        }
    }
}
