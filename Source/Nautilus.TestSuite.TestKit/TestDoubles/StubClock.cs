// -------------------------------------------------------------------------------------------------
// <copyright file="StubClock.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.BlackBox.Core.Interfaces;
    using NodaTime;

    /// <summary>
    /// The system clock.
    /// </summary>
    public class StubClock : IZonedClock
    {
        private bool isTimeFrozen;
        private ZonedDateTime frozenTime;

        /// <summary>
        /// The time now.
        /// </summary>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public ZonedDateTime TimeNow()
        {
            if (this.isTimeFrozen)
            {
                return this.frozenTime;
            }

            return SystemClock.Instance.GetCurrentInstant().InUtc();
        }

        /// <summary>
        /// The unfreeze time.
        /// </summary>
        public void UnfreezeTime()
        {
            this.isTimeFrozen = false;
        }

        /// <summary>
        /// The freeze current time.
        /// </summary>
        public void FreezeTimeNow()
        {
            this.isTimeFrozen = true;
            this.frozenTime = this.TimeNow();
        }

        /// <summary>
        /// The freeze time.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        public void FreezeSetTime(ZonedDateTime dateTime)
        {
            this.isTimeFrozen = true;
            this.frozenTime = dateTime;
        }

        /// <summary>
        /// The get time zone.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTimeZone"/>.
        /// </returns>
        public DateTimeZone GetTimeZone()
        {
            return this.frozenTime.Zone;
        }
    }
}