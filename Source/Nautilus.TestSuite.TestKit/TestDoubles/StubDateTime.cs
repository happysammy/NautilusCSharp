//--------------------------------------------------------------------------------------------------
// <copyright file="StubDateTime.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using NautechSystems.CSharp.Annotations;
    using NodaTime;

    /// <summary>
    /// The immutable static <see cref="StubDateTime"/> class.
    /// </summary>
    [Immutable]
    public static class StubDateTime
    {
        // Unix time (also known as POSIX time or epoch time) is a system for describing instants in time,
        // defined as the number of seconds that have elapsed since 00:00:00 Coordinated Universal SignalTimestamp (UTC),
        // Thursday, 1 January 1970, minus the number of leap seconds that have taken place since then.
        private static readonly ZonedDateTime UnixEpoch = new ZonedDateTime(Instant.FromUnixTimeSeconds(1), DateTimeZone.Utc);

        /// <summary>
        /// Returns the stub <see cref="ZonedDateTime"/> for testing purposes.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime Now()
        {
            return UnixEpoch;
        }
    }
}
