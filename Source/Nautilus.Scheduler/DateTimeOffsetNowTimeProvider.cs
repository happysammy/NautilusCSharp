// -------------------------------------------------------------------------------------------------
// <copyright file="DateTimeOffsetNowTimeProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;

    /// <summary>
    /// TBD.
    /// </summary>
    public class DateTimeOffsetNowTimeProvider : ITimeProvider
    {
        private static readonly DateTimeOffsetNowTimeProvider StaticInstance = new DateTimeOffsetNowTimeProvider();

        private DateTimeOffsetNowTimeProvider()
        {
        }

        /// <summary>
        /// Gets tBD.
        /// </summary>
        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets tBD.
        /// </summary>
        public TimeSpan Elapsed => MonotonicClock.Elapsed;

        /// <summary>
        /// Gets tBD.
        /// </summary>
        public TimeSpan ElapsedHighRes => MonotonicClock.ElapsedHighRes;

        /// <summary>
        /// Gets tbd.
        /// </summary>
        public static DateTimeOffsetNowTimeProvider Instance => StaticInstance;
    }
}
