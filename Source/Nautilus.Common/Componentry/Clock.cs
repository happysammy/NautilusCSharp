//--------------------------------------------------------------------------------------------------
// <copyright file="Clock.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
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
