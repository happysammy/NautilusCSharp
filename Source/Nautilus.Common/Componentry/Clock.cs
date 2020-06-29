//--------------------------------------------------------------------------------------------------
// <copyright file="Clock.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Componentry
{
    using Nautilus.Common.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a clock with an embedded timezone.
    /// </summary>
    public sealed class Clock : IZonedClock
    {
        private readonly ZonedClock clock;
        private readonly DateTimeZone dateTimeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="Clock"/> class.
        /// </summary>
        /// <param name="dateTimeZone">The date time zone.</param>
        public Clock(DateTimeZone dateTimeZone)
        {
            this.clock = new ZonedClock(SystemClock.Instance, dateTimeZone, CalendarSystem.Iso);
            this.dateTimeZone = dateTimeZone;
        }

        /// <inheritdoc />
        public ZonedDateTime TimeNow() => this.clock.GetCurrentZonedDateTime();

        /// <inheritdoc />
        public Instant InstantNow() => this.clock.GetCurrentInstant();

        /// <inheritdoc />
        public DateTimeZone GetTimeZone() => this.dateTimeZone;
    }
}
