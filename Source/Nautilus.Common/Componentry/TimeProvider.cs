//--------------------------------------------------------------------------------------------------
// <copyright file="TimeProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Provides useful methods relating to time.
    /// </summary>
    public static class TimeProvider
    {
        /// <summary>
        /// Returns a value indicating whether the given instant now is inside the given weekly interval.
        /// </summary>
        /// <param name="start">The start of the weekly interval.</param>
        /// <param name="end">The end of the weekly interval.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>True if now is inside the given interval, else false.</returns>
        public static bool IsInsideWeeklyInterval(
            (IsoDayOfWeek DayOfWeek, LocalTime time) start,
            (IsoDayOfWeek DayOfWeek, LocalTime time) end,
            [CanBeDefault] Instant now)
        {
            Debug.NotDefault(start.DayOfWeek, nameof(start.DayOfWeek));
            Debug.NotDefault(end.DayOfWeek, nameof(end.DayOfWeek));
            Debug.True(start.DayOfWeek <= end.DayOfWeek, nameof(start.DayOfWeek));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;

            var startThisWeek = localNow
                .Date.With(DateAdjusters.NextOrSame(start.DayOfWeek))
                .At(start.time);

            var endThisWeek = localNow
                .Date.With(DateAdjusters.NextOrSame(end.DayOfWeek))
                .At(end.time);

            return localNow >= startThisWeek && localNow <= endThisWeek;
        }

        /// <summary>
        /// Returns a value indicating whether the given instant now is outside the given weekly interval.
        /// </summary>
        /// <param name="start">The start of the weekly interval.</param>
        /// <param name="end">The end of the weekly interval.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>True if now is outside the given interval, else false.</returns>
        public static bool IsOutsideWeeklyInterval(
            (IsoDayOfWeek DayOfWeek, LocalTime time) start,
            (IsoDayOfWeek DayOfWeek, LocalTime time) end,
            [CanBeDefault] Instant now)
        {
            Debug.NotDefault(start.DayOfWeek, nameof(start.DayOfWeek));
            Debug.NotDefault(end.DayOfWeek, nameof(end.DayOfWeek));
            Debug.True(start.DayOfWeek <= end.DayOfWeek, nameof(start.DayOfWeek));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;

            var startThisWeek = localNow
                .Date.With(DateAdjusters.NextOrSame(start.DayOfWeek))
                .At(start.time);

            var endThisWeek = localNow
                .Date.With(DateAdjusters.NextOrSame(end.DayOfWeek))
                .At(end.time);

            return localNow < startThisWeek || localNow > endThisWeek;
        }

        /// <summary>
        /// Returns the duration to the next day of week and time of day (UTC).
        /// </summary>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <param name="timeOfDay">The time of day.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>The next date time (UTC).</returns>
        public static ZonedDateTime GetNextUtc(
            IsoDayOfWeek dayOfWeek,
            LocalTime timeOfDay,
            [CanBeDefault] Instant now)
        {
            Debug.NotDefault(dayOfWeek, nameof(dayOfWeek));
            Debug.NotDefault(timeOfDay, nameof(timeOfDay));
            Debug.NotDefault(now, nameof(now));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;
            var localNext = localNow
                .Date.With(DateAdjusters.NextOrSame(dayOfWeek))
                .At(timeOfDay);

            // Handle "we're already on the right day-of-week, but later in the day".
            if (localNext <= localNow)
            {
                localNext = localNext.PlusWeeks(1);
            }

            return localNext.InUtc();
        }

        /// <summary>
        /// Returns the duration to the given next time (UTC).
        /// </summary>
        /// <param name="next">The next date time (UTC).</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>The duration.</returns>
        public static Duration GetDurationToNextUtc(
            ZonedDateTime next,
            [CanBeDefault] Instant now)
        {
            Debug.NotDefault(next, nameof(next));

            return next.ToInstant() - now;
        }
    }
}
