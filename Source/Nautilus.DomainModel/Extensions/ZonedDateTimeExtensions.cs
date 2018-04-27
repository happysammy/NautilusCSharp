// -------------------------------------------------------------------------------------------------
// <copyright file="ZonedDateTimeExtensions.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Extensions
{
    using System.Globalization;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using NodaTime.Text;

    /// <summary>
    /// The immutable static <see cref="ZonedDateTimeExtensions"/> class.
    /// </summary>
    [Immutable]
    public static class ZonedDateTimeExtensions
    {
        private static readonly ZonedDateTimePattern FormattedStringParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                "yyyy-MM-ddTHH:mm:ss.fff",
                DateTimeZoneProviders.Tzdb);

        /// <summary>
        /// Returns a <see cref="ZonedDateTime"/> parsed from the given string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static ZonedDateTime GetFromString(string dateTime)
        {
            Validate.NotNull(dateTime, nameof(dateTime));

            var dateTimeToParse = dateTime.Replace("Z", string.Empty);

            return FormattedStringParsePattern.Parse(dateTimeToParse).Value;
        }

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>A <see cref="string"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public static string ToStringFormattedIsoUtc(this ZonedDateTime time)
        {
            Validate.NotNull(time, nameof(time));

            return time.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }

        /// <summary>
        /// Returns a string formatted to the millisecond (ISO-8601) from the <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToStringFormattedIsoUtc(this ZonedDateTime? time)
        {
            if (time == null)
            {
                return string.Empty;
            }

            var timeZoned = (ZonedDateTime)time;

            return timeZoned.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture.DateTimeFormat) + "Z";
        }
    }
}