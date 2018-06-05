//--------------------------------------------------------------------------------------------------
// <copyright file="ZonedDateTimeExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System.Globalization;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// Provides useful comparison operations for the <see cref="ZonedDateTime"/> type.
    /// </summary>
    public static class ZonedDateTimeExtensions
    {
        private const string IsoStringParsePattern = "yyyy-MM-ddTHH:mm:ss.fff";

        private static readonly ZonedDateTimePattern NodaIsoStringParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                IsoStringParsePattern,
                DateTimeZoneProviders.Tzdb);

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given (ISO-8601) string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ToZonedDateTimeFromIso(this string dateTime)
        {
            Debug.NotNull(dateTime, nameof(dateTime));

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
            Debug.NotNull(dateTime, nameof(dateTime));
            Debug.NotNull(parsePattern, nameof(parsePattern));

            return ZonedDateTimePattern.CreateWithInvariantCulture(parsePattern, DateTimeZoneProviders.Tzdb)
                .Parse(dateTime)
                .Value;
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

            return time.ToString(IsoStringParsePattern, CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time to convert to.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToIsoString([CanBeNull] this ZonedDateTime? time)
        {
            return time == null
                   ? string.Empty
                   : ToIsoString((ZonedDateTime)time);
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
            Debug.NotNull(parsePattern, nameof(parsePattern));

            return time.ToString(parsePattern, CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array from this <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>A <see cref="byte"/> array</returns>
        public static byte[] ToBytes(this ZonedDateTime time)
        {
            Debug.NotDefault(time, nameof(time));

            return Encoding.UTF8.GetBytes(ToIsoString(time));
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
    }
}
