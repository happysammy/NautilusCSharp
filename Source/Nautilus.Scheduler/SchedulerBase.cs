// -------------------------------------------------------------------------------------------------
// <copyright file="SchedulerBase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Abstract base class for implementing any custom <see cref="IScheduler"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1600", Justification = "Justified!")]
    public abstract class SchedulerBase : IScheduler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerBase"/> class.
        /// </summary>
        /// <param name="log">The logging adapter.</param>
        protected SchedulerBase(ILoggingAdapter log)
        {
            this.Log = log;
        }

        /// <summary>
        /// Gets the time now.
        /// </summary>
        DateTimeOffset ITimeProvider.Now => this.TimeNow;

        /// <summary>
        /// Gets the current time since startup, as determined by the high resolution monotonic clock implementation.
        /// </summary>
        /// <remarks>
        /// Typically uses <see cref="MonotonicClock"/> in most implementations, but in some cases a
        /// custom implementation is used - such as when we need to do virtual time scheduling in the Akka.TestKit.
        /// </remarks>
        public abstract TimeSpan ElapsedHighRes { get; }

        /// <summary>
        /// Gets the logging adapter.
        /// </summary>
        protected ILoggingAdapter Log { get; }

        /// <summary>
        /// Gets the time now.
        /// </summary>
        protected abstract DateTimeOffset TimeNow { get; }

        void ISendScheduler.ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleSendOnce(delay, receiver, message, sender, null);
        }

        void ISendScheduler.ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleSendOnce(delay, receiver, message, sender, cancelable);
        }

        void ISendScheduler.ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleSendRepeatedly(initialDelay, interval, receiver, message, sender, null);
        }

        void ISendScheduler.ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleSendRepeatedly(initialDelay, interval, receiver, message, sender, cancelable);
        }

        void IActionScheduler.ScheduleOnce(TimeSpan delay, Action action)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleOnce(delay, action, null);
        }

        void IActionScheduler.ScheduleOnce(TimeSpan delay, Action action, ICancelable cancelable)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleOnce(delay, action, cancelable);
        }

        void IActionScheduler.ScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleRepeatedly(initialDelay, interval, action, null);
        }

        void IActionScheduler.ScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action, ICancelable cancelable)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleRepeatedly(initialDelay, interval, action, cancelable);
        }

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected abstract void InternalScheduleSendOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable);

        /// <summary>
        /// TBD.
        /// </summary>
        /// <param name="initialDelay">TBD</param>
        /// <param name="interval">TBD</param>
        /// <param name="receiver">TBD</param>
        /// <param name="message">TBD</param>
        /// <param name="sender">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected abstract void InternalScheduleSendRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable);

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="delay">TBD</param>
        /// <param name="action">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected abstract void InternalScheduleOnce(TimeSpan delay, Action action, ICancelable cancelable);
        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="initialDelay">TBD</param>
        /// <param name="interval">TBD</param>
        /// <param name="action">TBD</param>
        /// <param name="cancelable">TBD</param>
        protected abstract void InternalScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action, ICancelable cancelable);

        /// <summary>
        /// Validate the given interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown if the given <paramref name="interval"/> is negative or zero.</exception>
        private static void ValidateInterval(TimeSpan interval, string parameterName)
        {
            if (interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(parameterName), $"Interval must be >0. It was {interval}");
            }
        }

        /// <summary>
        /// Validate the delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown if the given <paramref name="delay"/> is negative.</exception>
        private static void ValidateDelay(TimeSpan delay, string parameterName)
        {
            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(parameterName), $"Delay must be >=0. It was {delay}");
            }
        }
    }
}
