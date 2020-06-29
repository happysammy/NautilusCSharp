// -------------------------------------------------------------------------------------------------
// <copyright file="IActionScheduler.cs" company="Nautech Systems Pty Ltd">
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
    using System;
    using NodaTime;

    /// <summary>
    /// Provides an interface which defines a scheduler that is able to execute actions on a set schedule.
    /// </summary>
    public interface IActionScheduler
    {
        /// <summary>
        /// Schedules an action to be invoked after a delay.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the action is invoked.</param>
        /// <param name="action">The action that is being scheduled.</param>
        void ScheduleOnce(Duration delay, Action action);

        /// <summary>
        /// Schedules an action to be invoked at the given time.
        /// </summary>
        /// <param name="forTime">The time the action should be invoked.</param>
        /// <param name="action">The action that is being scheduled.</param>
        void ScheduleOnce(ZonedDateTime forTime, Action action);

        /// <summary>
        /// Schedules an action to be invoked after an initial delay and then repeatedly.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before first invocation of the action.</param>
        /// <param name="interval">The time period that has to pass between each invocation of the action.</param>
        /// <param name="action">The action that is being scheduled.</param>
        void ScheduleRepeatedly(Duration initialDelay, Duration interval, Action action);

        /// <summary>
        /// Schedules an action to be invoked after a delay.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the action is invoked.</param>
        /// <param name="action">The action that is being scheduled.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleOnceCancelable(Duration delay, Action action);

        /// <summary>
        /// Schedules an action to be invoked at the given time.
        /// </summary>
        /// <param name="forTime">The time the action should be invoked.</param>
        /// <param name="action">The action that is being scheduled.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleOnceCancelable(ZonedDateTime forTime, Action action);

        /// <summary>
        /// Schedules an action to be invoked after an initial delay and then repeatedly.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before first invocation of the action.</param>
        /// <param name="interval">The time period that has to pass between each invocation of the action.</param>
        /// <param name="action">The action that is being scheduled.</param>
        /// <returns>The cancellable token.</returns>
        ICancelable ScheduleRepeatedlyCancelable(Duration initialDelay, Duration interval, Action action);
    }
}
