// -------------------------------------------------------------------------------------------------
// <copyright file="ISendScheduler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduling
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
