// -------------------------------------------------------------------------------------------------
// <copyright file="ISendScheduler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides an interface which defines a scheduler that is able to send messages on a set schedule.
    /// </summary>
    public interface ISendScheduler
    {
        /// <summary>
        /// Schedules a message to be sent once after a specified period of time.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the message is sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        void ScheduleSendOnce(Duration delay, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent once at the given time.
        /// </summary>
        /// <param name="forTime">The time the message should be sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        void ScheduleSendOnce(ZonedDateTime forTime, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent repeatedly after an initial delay.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param>
        /// <param name="interval">The time period that has to pass between sending of the message.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        void ScheduleSendRepeatedly(Duration initialDelay, Duration interval, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent once after a specified period of time.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the message is sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleSendOnceCancelable(Duration delay, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent at the given time, which can be cancelled.
        /// </summary>
        /// <param name="forTime">The time the message should be sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleSendOnceCancelable(ZonedDateTime forTime, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent repeatedly after an initial delay.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param>
        /// <param name="interval">The time period that has to pass between sending of the message.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleSendRepeatedlyCancelable(Duration initialDelay, Duration interval, IEndpoint receiver, object message, IEndpoint sender);
    }
}
