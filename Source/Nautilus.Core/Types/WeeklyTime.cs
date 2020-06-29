//--------------------------------------------------------------------------------------------------
// <copyright file="WeeklyTime.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Types
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a particular time every week.
    /// </summary>
    [Immutable]
    public sealed class WeeklyTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyTime"/> class.
        /// </summary>
        /// <param name="dayOfWeek">The day of the week.</param>
        /// <param name="time">The time of the week.</param>
        public WeeklyTime(IsoDayOfWeek dayOfWeek, [CanBeDefault] LocalTime time)
        {
            Condition.NotDefault(dayOfWeek, nameof(dayOfWeek));

            this.DayOfWeek = dayOfWeek;
            this.Time = time;
        }

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        public IsoDayOfWeek DayOfWeek { get; }

        /// <summary>
        /// Gets the time of the week.
        /// </summary>
        public LocalTime Time { get; }
    }
}
