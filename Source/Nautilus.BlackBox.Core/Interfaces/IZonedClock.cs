// -------------------------------------------------------------------------------------------------
// <copyright file="IZonedClock.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NodaTime;

    /// <summary>
    /// The <see cref="IZonedClock"/> interface. The clock for the <see cref="BlackBox"/> system.
    /// </summary>
    public interface IZonedClock
    {
        /// <summary>
        /// Returns the current time of this clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        ZonedDateTime TimeNow();

        /// <summary>
        /// Returns the time zone of this clock.
        /// </summary>
        /// <returns>A <see cref="DateTimeZone"/>.</returns>
        DateTimeZone GetTimeZone();
    }
}
