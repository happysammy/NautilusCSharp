//--------------------------------------------------------------------------------------------------
// <copyright file="DateKey.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Keys
{
    using System;
    using System.Diagnostics;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using NodaTime;

    /// <summary>
    /// Represents a trading session date.
    /// </summary>
    [Immutable]
    public struct DateKey : IComparable<DateKey>, IEquatable<DateKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateKey"/> struct.
        /// </summary>
        /// <param name="year">The date key year.</param>
        /// <param name="month">The date key month.</param>
        /// <param name="day">The date key day.</param>
        public DateKey(int year, int month, int day)
        {
            Debug.Assert(year > 0, "The value of year must be > 0.");
            Debug.Assert(month > 0, "The value of month must be > 0.");
            Debug.Assert(day > 0, "The value of day must be > 0.");

            this.Year = year;
            this.Month = month;
            this.Day = day;

            var localDateTime = new LocalDateTime(this.Year, this.Month, this.Day, 0, 0);
            this.StartOfDay = new ZonedDateTime(localDateTime, DateTimeZone.Utc, Offset.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateKey"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp for the key.</param>
        public DateKey(ZonedDateTime timestamp)
            : this(timestamp.Year, timestamp.Month, timestamp.Day)
        {
            Debug.Assert(timestamp != default, "The timestamp cannot be the default value.");
        }

        /// <summary>
        /// Gets the <see cref="DateKey"/>s year.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Gets the <see cref="DateKey"/>s month.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Gets the <see cref="DateKey"/>s day.
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// Gets a <see cref="StartOfDay"/> timestamp of the <see cref="DateKey"/> in UTC.
        /// </summary>
        public ZonedDateTime StartOfDay { get; }

        /// <summary>
        /// Compares the given <see cref="DateKey"/> to this <see cref="DateKey"/>.
        /// </summary>
        /// <param name="other">The other date key to compare.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(DateKey other)
        {
            return this.StartOfDay.Compare(other.StartOfDay);
        }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other != null && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="DateKey"/> is equal to the given <see cref="DateKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="DateKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(DateKey other)
        {
            return this.Year.Equals(other.Year) &&
                   this.Month.Equals(other.Month) &&
                   this.Day.Equals(other.Day);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="DateKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.Year.GetHashCode() +
                   this.Month.GetHashCode() +
                   this.Day.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="DateKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Year:D4}-{this.Month:D2}-{this.Day:D2}";
    }
}
