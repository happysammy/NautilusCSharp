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
    using Nautilus.Core;
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
        protected SchedulerBase(ILogger log)
        {
            this.Log = log;
        }

        /// <summary>
        /// Gets the logging adapter.
        /// </summary>
        protected ILogger Log { get; }

        void ISendScheduler.ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleSendOnce(delay, receiver, message, sender, OptionRef<ICancelable>.None());
        }

        void ISendScheduler.ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender, OptionRef<ICancelable> cancelable)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleSendOnce(delay, receiver, message, sender, cancelable);
        }

        void ISendScheduler.ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleSendRepeatedly(initialDelay, interval, receiver, message, sender, OptionRef<ICancelable>.None());
        }

        void ISendScheduler.ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender, OptionRef<ICancelable> cancelable)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleSendRepeatedly(initialDelay, interval, receiver, message, sender, cancelable);
        }

        void IActionScheduler.ScheduleOnce(TimeSpan delay, Action action)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleOnce(delay, action, OptionRef<ICancelable>.None());
        }

        void IActionScheduler.ScheduleOnce(TimeSpan delay, Action action, OptionRef<ICancelable> cancelable)
        {
            ValidateDelay(delay, "delay");
            this.InternalScheduleOnce(delay, action, cancelable);
        }

        void IActionScheduler.ScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleRepeatedly(initialDelay, interval, action, OptionRef<ICancelable>.None());
        }

        void IActionScheduler.ScheduleRepeatedly(TimeSpan initialDelay, TimeSpan interval, Action action, OptionRef<ICancelable> cancelable)
        {
            ValidateDelay(initialDelay, "initialDelay");
            ValidateInterval(interval, "interval");
            this.InternalScheduleRepeatedly(initialDelay, interval, action, cancelable);
        }

        /// <summary>
        /// Schedule a message to send once.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected abstract void InternalScheduleSendOnce(
            TimeSpan delay,
            IEndpoint receiver,
            object message,
            IEndpoint sender,
            OptionRef<ICancelable> cancelable);

        /// <summary>
        /// Schedule a message to send repeatedly.
        /// </summary>
        /// <param name="initialDelay">The initial delay.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="receiver">The receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected abstract void InternalScheduleSendRepeatedly(
            TimeSpan initialDelay,
            TimeSpan interval,
            IEndpoint receiver,
            object message,
            IEndpoint sender,
            OptionRef<ICancelable> cancelable);

        /// <summary>
        /// Schedule a action to be invoked once.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected abstract void InternalScheduleOnce(
            TimeSpan delay,
            Action action,
            OptionRef<ICancelable> cancelable);

        /// <summary>
        /// Schedule a action to be invoked repeatedly.
        /// </summary>
        /// <param name="initialDelay">The initial delay.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancelable">The cancelable.</param>
        protected abstract void InternalScheduleRepeatedly(
            TimeSpan initialDelay,
            TimeSpan interval,
            Action action,
            OptionRef<ICancelable> cancelable);

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
                throw new ArgumentOutOfRangeException(nameof(parameterName), $"Interval must be > 0. It was {interval}");
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
                throw new ArgumentOutOfRangeException(nameof(parameterName), $"Delay must be >= 0. It was {delay}");
            }
        }
    }
}
