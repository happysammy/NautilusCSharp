//--------------------------------------------------------------------------------------------------
// <copyright file="TestClock.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
    using Nautilus.Common.Interfaces;
    using NodaTime;

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
