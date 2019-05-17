// -------------------------------------------------------------------------------------------------
// <copyright file="MonotonicClock.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Internal
{
    using System.Diagnostics;
    using NodaTime;
    using NodaTime.Extensions;

    /// <summary>
    /// Provides a monotonic clock implementation based on total uptime.
    /// Used for keeping accurate time internally.
    /// </summary>
    internal static class MonotonicClock
    {
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// Gets the elapsed time as determined by a <see cref="Stopwatch"/>
        /// running continuously in the background.
        /// </summary>
        internal static Duration Elapsed => Stopwatch.Elapsed.ToDuration();
    }
}
