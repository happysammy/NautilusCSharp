//--------------------------------------------------------------------------------------------------
// <copyright file="TestClock.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Interfaces;
using NodaTime;

namespace Nautilus.TestSuite.TestKit.Components
{
    /// <inheritdoc/>
    public sealed class TestClock : IZonedClock
    {
        private bool isTimeFrozen;
        private ZonedDateTime frozenTime;

        /// <inheritdoc/>
        public DateTimeZone GetTimeZone()
        {
            return this.frozenTime.Zone;
        }

        /// <inheritdoc/>
        public ZonedDateTime TimeNow()
        {
            return this.isTimeFrozen
                ? this.frozenTime
                : SystemClock.Instance.GetCurrentInstant().InUtc();
        }

        /// <inheritdoc/>
        public Instant InstantNow()
        {
            return this.isTimeFrozen
                ? this.frozenTime.ToInstant()
                : SystemClock.Instance.GetCurrentInstant();
        }

        /// <summary>
        /// Unfreeze the clocks time.
        /// </summary>
        public void UnfreezeTime()
        {
            this.isTimeFrozen = false;
        }

        /// <summary>
        /// Freeze the clock at the current time.
        /// </summary>
        public void FreezeTimeNow()
        {
            this.isTimeFrozen = true;
            this.frozenTime = this.TimeNow();
        }

        /// <summary>
        /// Freeze the clock at the given time.
        /// </summary>
        /// <param name="dateTime">The freeze datetime.</param>
        public void FreezeSetTime(ZonedDateTime dateTime)
        {
            this.isTimeFrozen = true;
            this.frozenTime = dateTime;
        }
    }
}
