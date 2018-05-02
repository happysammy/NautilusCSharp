// -------------------------------------------------------------------------------------------------
// <copyright file="StubZonedDateTime.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using NodaTime;

    /// <summary>
    /// The static <see cref="StubZonedDateTime"/> class.
    /// </summary>
    public static class StubZonedDateTime
    {
        // Unix time (also known as POSIX time or epoch time) is a system for describing instants in time,
        // defined as the number of seconds that have elapsed since 00:00:00 Coordinated Universal Time (UTC),
        // Thursday, 1 January 1970, minus the number of leap seconds that have taken place since then.
        private static readonly ZonedDateTime UnixEpochZonedDateTime = new ZonedDateTime(Instant.FromUnixTimeSeconds(0), DateTimeZone.Utc);

        /// <summary>
        /// Returns the stub time (unix epoch).
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime UnixEpoch()
        {
            return UnixEpochZonedDateTime;
        }
    }
}
