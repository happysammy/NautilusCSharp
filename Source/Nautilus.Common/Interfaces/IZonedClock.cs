//--------------------------------------------------------------------------------------------------
// <copyright file="IZonedClock.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

using NodaTime;

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// The <see cref="IZonedClock"/> interface. The clock for the database system.
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
