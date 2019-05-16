// -------------------------------------------------------------------------------------------------
// <copyright file="MonotonicClock.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// INTERNAL API
    ///
    /// A Monotonic clock implementation based on total uptime.
    /// Used for keeping accurate time internally.
    /// </summary>
    internal static class MonotonicClock
    {
        private const int TicksInMillisecond = 10000;
        private const long NanosPerTick = 100;
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// Time as measured by the current system up-time.
        /// </summary>
        public static TimeSpan Elapsed => TimeSpan.FromTicks(GetTicks());

        /// <summary>
        /// High resolution elapsed time as determined by a <see cref="Stopwatch"/>
        /// running continuously in the background.
        /// </summary>
        public static TimeSpan ElapsedHighRes => Stopwatch.Elapsed;

    /// <summary>
        /// TBD
        /// </summary>
        /// <returns>TBD</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetMilliseconds() => Stopwatch.ElapsedMilliseconds;

    /// <summary>
        /// TBD
        /// </summary>
        /// <returns>TBD</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetNanos() => GetTicks() * NanosPerTick;

    /// <summary>
        /// TBD
        /// </summary>
        /// <returns>TBD</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetTicks() => GetMilliseconds() * TicksInMillisecond;

    /// <summary>
        /// Ticks represent 100 nanos. https://msdn.microsoft.com/en-us/library/system.datetime.ticks(v=vs.110).aspx
        ///
        /// This extension method converts a Ticks value to nano seconds.
        /// </summary>
        /// <param name="ticks">TBD</param>
        /// <returns>TBD</returns>
        internal static long ToNanos(this long ticks) => ticks * NanosPerTick;

    /// <summary>
        /// Ticks represent 100 nanos. https://msdn.microsoft.com/en-us/library/system.datetime.ticks(v=vs.110).aspx
        ///
        /// This extension method converts a nano seconds value to Ticks.
        /// </summary>
        /// <param name="nanos">TBD</param>
        /// <returns>TBD</returns>
        internal static long ToTicks(this long nanos)
        {
            return nanos / NanosPerTick;
        }
    }
}
