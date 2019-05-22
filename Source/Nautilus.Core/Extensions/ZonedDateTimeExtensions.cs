//--------------------------------------------------------------------------------------------------
// <copyright file="ZonedDateTimeExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Nautilus.Core.Correctness;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// Provides useful comparison operations for the <see cref="ZonedDateTime"/> type.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read with underscores.")]
    public static class ZonedDateTimeExtensions
    {
        private const string ISO_STRING_PARSE_PATTERN = "yyyy-MM-ddTHH:mm:ss.fff";

        private static readonly ZonedDateTimePattern NodaIsoStringParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                ISO_STRING_PARSE_PATTERN,
                DateTimeZoneProviders.Tzdb);

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given (ISO-8601) string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ToZonedDateTimeFromIso(this string dateTime)
        {
            Debug.NotEmptyOrWhiteSpace(dateTime, nameof(dateTime));

            return NodaIsoStringParsePattern.Parse(dateTime.Replace("Z", string.Empty)).Value;
        }

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="parsePattern">The parse pattern.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ToZonedDateTime(this string dateTime, string parsePattern)
        {
            Debug.NotEmptyOrWhiteSpace(dateTime, nameof(dateTime));
            Debug.NotEmptyOrWhiteSpace(parsePattern, nameof(parsePattern));

            return ZonedDateTimePattern
                .CreateWithInvariantCulture(parsePattern, DateTimeZoneProviders.Tzdb)
                .Parse(dateTime)
                .GetValueOrThrow();
        }

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToIsoString(this ZonedDateTime time)
        {
            Debug.NotDefault(time, nameof(time));

            return time.ToString(ISO_STRING_PARSE_PATTERN, CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }

        /// <summary>
        /// Returns a string formatted as per the given parse pattern <see cref="string"/>.
        /// </summary>
        /// <param name="time">The time to convert..</param>
        /// <param name="parsePattern">The parse pattern to convert to.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToStringWithParsePattern(this ZonedDateTime time, string parsePattern)
        {
            Debug.NotDefault(time, nameof(time));
            Debug.NotEmptyOrWhiteSpace(parsePattern, nameof(parsePattern));

            return time.ToString(parsePattern, CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
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
