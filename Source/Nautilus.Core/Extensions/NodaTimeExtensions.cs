//--------------------------------------------------------------------------------------------------
// <copyright file="NodaTimeExtensions.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Globalization;
using Nautilus.Core.Correctness;
using NodaTime;
using NodaTime.Text;

namespace Nautilus.Core.Extensions
{
    /// <summary>
    /// Provides useful comparison operations for the <see cref="ZonedDateTime"/> type.
    /// </summary>
    public static class NodaTimeExtensions
    {
        private const string Iso8601DateTimeParsePattern = "yyyy-MM-ddTHH:mm:ss.fff";
        private const string Iso8601DateParsePattern = "yyyy-MM-dd";

        private static readonly ZonedDateTimePattern NodaIsoStringParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                Iso8601DateTimeParsePattern,
                DateTimeZoneProviders.Tzdb);

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToIso8601String(this ZonedDateTime time)
        {
            Debug.NotDefault(time, nameof(time));

            return time.ToString(Iso8601DateTimeParsePattern, CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToIso8601String(this LocalDate date)
        {
            Debug.NotDefault(date, nameof(date));

            return date.ToString(Iso8601DateParsePattern, CultureInfo.InvariantCulture.DateTimeFormat);
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given (ISO-8601) string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ToZonedDateTimeFromIso(this string dateTime)
        {
            Condition.NotEmptyOrWhiteSpace(dateTime, nameof(dateTime));

            // Ensure format
            dateTime = dateTime
                .Replace(" ", "T")
                .Replace("Z", string.Empty);

            return NodaIsoStringParsePattern.Parse(dateTime).Value;
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given (ISO-8601) string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime? ToNullableZonedDateTimeFromIso(this string dateTime)
        {
            Condition.NotEmptyOrWhiteSpace(dateTime, nameof(dateTime));

            // Ensure format
            dateTime = dateTime
                .Replace(" ", "T")
                .Replace("Z", string.Empty);

            return NodaIsoStringParsePattern.Parse(dateTime).Value;
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="parsePattern">The parse pattern.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ToZonedDateTime(this string dateTime, string parsePattern)
        {
            Condition.NotEmptyOrWhiteSpace(dateTime, nameof(dateTime));
            Condition.NotEmptyOrWhiteSpace(parsePattern, nameof(parsePattern));

            return ZonedDateTimePattern
                .CreateWithInvariantCulture(parsePattern, DateTimeZoneProviders.Tzdb)
                .Parse(dateTime)
                .GetValueOrThrow();
        }

        /// <summary>
        /// Returns a value indicating whether the left <see cref="ZonedDateTime"/> is less than
        /// equal to or greater than the right <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int Compare(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return ZonedDateTime.Comparer.Instant.Compare(left, right);
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="ZonedDateTime"/> is equal to
        /// this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static bool IsEqualTo(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return Compare(left, right) == 0;
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="ZonedDateTime"/> is greater than
        /// this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static bool IsGreaterThan(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return Compare(left, right) == 1;
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="ZonedDateTime"/> is greater than
        /// or equal to this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static bool IsGreaterThanOrEqualTo(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return Compare(left, right) >= 0;
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="ZonedDateTime"/> is less than
        /// this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static bool IsLessThan(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return Compare(left, right) == -1;
        }

        /// <summary>
        /// Returns a value indicating whether the given <see cref="ZonedDateTime"/> is less than
        /// or equal to this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="left">The left <see cref="ZonedDateTime"/>.</param>
        /// <param name="right">The right <see cref="ZonedDateTime"/>.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static bool IsLessThanOrEqualTo(this ZonedDateTime left, ZonedDateTime right)
        {
            Debug.NotDefault(left, nameof(left));
            Debug.NotDefault(right, nameof(right));

            return Compare(left, right) <= 0;
        }

        /// <summary>
        /// Returns the last floored time based on the given duration.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="duration">The duration floor.</param>
        /// <returns>The <see cref="ZonedDateTime"/> less the rounded duration floor.</returns>
        public static ZonedDateTime Floor(this ZonedDateTime time, Duration duration)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotDefault(duration, nameof(duration));

            var offset = FloorOffsetMilliseconds(time, duration);

            return time - Duration.FromMilliseconds(offset);
        }

        /// <summary>
        /// Returns the next time interval with a ceiling based on the given duration.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="duration">The duration ceiling.</param>
        /// <returns>The <see cref="ZonedDateTime"/> plus the rounded duration ceiling.</returns>
        public static ZonedDateTime Ceiling(this ZonedDateTime time, Duration duration)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotDefault(duration, nameof(duration));

            var offset = CeilingOffsetMilliseconds(time, duration);

            return time + Duration.FromMilliseconds(offset);
        }

        /// <summary>
        /// Returns the offset milliseconds of the given time floored from the given duration.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The milliseconds offset.</returns>
        public static int FloorOffsetMilliseconds(this ZonedDateTime time, Duration duration)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotDefault(duration, nameof(duration));

            var durationMs = duration.TotalMilliseconds;
            var unixMs = time.ToInstant().ToUnixTimeMilliseconds();

            var floored = Math.Floor(unixMs / durationMs) * durationMs;

            return (int)(unixMs - floored);
        }

        /// <summary>
        /// Returns the offset milliseconds of the given time ceiling from the given duration.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>The milliseconds offset.</returns>
        public static int CeilingOffsetMilliseconds(this ZonedDateTime time, Duration duration)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotDefault(duration, nameof(duration));

            var durationMs = duration.TotalMilliseconds;
            var unixMs = time.ToInstant().ToUnixTimeMilliseconds();

            var ceiling = Math.Ceiling(unixMs / durationMs) * durationMs;

            return (int)(ceiling - unixMs);
        }
    }
}
