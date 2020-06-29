//--------------------------------------------------------------------------------------------------
// <copyright file="TimingProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using NodaTime;

    /// <summary>
    /// Provides useful methods relating to time.
    /// </summary>
    public static class TimingProvider
    {
        /// <summary>
        /// Returns the datetime for the weekly day and time this week.
        /// </summary>
        /// <param name="weekly">The weekly day and time.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>The <see cref="ZonedDateTime"/> for this week.</returns>
        public static ZonedDateTime ThisWeek(WeeklyTime weekly, [CanBeDefault] Instant now)
        {
            Debug.NotDefault(weekly.DayOfWeek, nameof(weekly.DayOfWeek));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;

            var midnightToday = localNow.Date.AtMidnight();
            var difference = Period.FromDays(weekly.DayOfWeek - localNow.DayOfWeek);

            var weeklyMidnight = midnightToday + difference;
            var weeklyDateTime = new LocalDateTime(
                weeklyMidnight.Year,
                weeklyMidnight.Month,
                weeklyMidnight.Day,
                weekly.Time.Hour,
                weekly.Time.Minute,
                weekly.Time.Second,
                weekly.Time.Millisecond);

            return new ZonedDateTime(weeklyDateTime, DateTimeZone.Utc, Offset.Zero);
        }

        /// <summary>
        /// Returns a value indicating whether the given instant now is inside the given weekly interval.
        /// </summary>
        /// <param name="start">The start of the weekly interval.</param>
        /// <param name="end">The end of the weekly interval.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>True if now is inside the given interval, else false.</returns>
        public static bool IsInsideInterval(WeeklyTime start, WeeklyTime end, [CanBeDefault] Instant now)
        {
            Debug.NotDefault(start.DayOfWeek, nameof(start.DayOfWeek));
            Debug.NotDefault(end.DayOfWeek, nameof(end.DayOfWeek));
            Debug.True(start.DayOfWeek <= end.DayOfWeek, nameof(start.DayOfWeek));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;

            var startDateTime = localNow
                .Date.With(DateAdjusters.NextOrSame(start.DayOfWeek))
                .At(start.Time);

            var endDateTime = localNow
                .Date.With(DateAdjusters.NextOrSame(end.DayOfWeek))
                .At(end.Time);

            return localNow >= startDateTime && localNow <= endDateTime;
        }

        /// <summary>
        /// Returns a value indicating whether the given instant now is outside the given weekly interval.
        /// </summary>
        /// <param name="start">The start of the weekly interval.</param>
        /// <param name="end">The end of the weekly interval.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>True if now is outside the given interval, else false.</returns>
        public static bool IsOutsideInterval(WeeklyTime start, WeeklyTime end, [CanBeDefault] Instant now)
        {
            Debug.NotDefault(start.DayOfWeek, nameof(start.DayOfWeek));
            Debug.NotDefault(end.DayOfWeek, nameof(end.DayOfWeek));
            Debug.True(start.DayOfWeek <= end.DayOfWeek, nameof(start.DayOfWeek));

            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;

            var localStart = localNow
                .Date.With(DateAdjusters.NextOrSame(start.DayOfWeek))
                .At(start.Time);

            var localEnd = localNow
                .Date.With(DateAdjusters.NextOrSame(end.DayOfWeek))
                .At(end.Time);

            return localNow < localStart || localNow > localEnd;
        }

        /// <summary>
        /// Returns the next date time for the given target local time (UTC).
        /// </summary>
        /// <param name="target">The target time of day.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>The next date time (UTC).</returns>
        public static ZonedDateTime GetNextUtc([CanBeDefault] LocalTime target, [CanBeDefault] Instant now)
        {
            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;
            var localTarget = new LocalDateTime(
                localNow.Year,
                localNow.Month,
                localNow.Day,
                target.Hour,
                target.Minute,
                target.Second,
                target.Millisecond);

            var nextToday = new ZonedDateTime(localTarget, DateTimeZone.Utc, Offset.Zero);
            if (nextToday.IsLessThanOrEqualTo(now.InUtc()))
            {
                localTarget += Period.FromDays(1);
                nextToday = new ZonedDateTime(localTarget, DateTimeZone.Utc, Offset.Zero);
            }

            return nextToday;
        }

        /// <summary>
        /// Returns the duration to the next day of week and time of day (UTC).
        /// </summary>
        /// <param name="weekly">The weekly time.</param>
        /// <param name="now">The current time instant.</param>
        /// <returns>The next date time (UTC).</returns>
        public static ZonedDateTime GetNextUtc(WeeklyTime weekly, [CanBeDefault] Instant now)
        {
            var localNow = now.InZone(DateTimeZone.Utc).LocalDateTime;
            var localNext = localNow
                .Date.With(DateAdjusters.NextOrSame(weekly.DayOfWeek))
                .At(weekly.Time);

            // Handle "we're already on the right day-of-week, but later in the day"
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

        /// <summary>
        /// Returns the duration of delay to the next duration from the floored time now.
        /// </summary>
        /// <param name="now">The time now.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The delay duration.</returns>
        public static Duration GetDelayToNextDuration(ZonedDateTime now, Duration duration)
        {
            return (now.Floor(duration) + duration) - now;
        }
    }
}
