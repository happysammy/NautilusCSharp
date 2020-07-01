//--------------------------------------------------------------------------------------------------
// <copyright file="DateKey.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using NodaTime;

namespace Nautilus.Data.Keys
{
    /// <summary>
    /// Represents a trading session date (UTC).
    /// </summary>
    [Immutable]
    public struct DateKey : IComparable<DateKey>, IEquatable<DateKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateKey"/> struct.
        /// </summary>
        /// <param name="date">The date for the key.</param>
        public DateKey(LocalDate date)
            : this(date.Year, date.Month, date.Day)
        {
            Debug.NotDefault(date, nameof(date));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateKey"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp for the key.</param>
        public DateKey(ZonedDateTime timestamp)
            : this(timestamp.Year, timestamp.Month, timestamp.Day)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateKey"/> struct.
        /// </summary>
        /// <param name="year">The date key year.</param>
        /// <param name="month">The date key month.</param>
        /// <param name="day">The date key day.</param>
        private DateKey(int year, int month, int day)
        {
            Debug.PositiveInt32(year, nameof(year));
            Debug.PositiveInt32(month, nameof(month));
            Debug.PositiveInt32(day, nameof(day));

            this.Year = year;
            this.Month = month;
            this.Day = day;
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
        /// Gets a <see cref="LocalDate"/> of this key UTC.
        /// </summary>
        public LocalDateTime DateUtc => new LocalDateTime(this.Year, this.Month, this.Day, 0, 0);

        /// <summary>
        /// Gets a <see cref="StartOfDay"/> timestamp of the <see cref="DateKey"/> in UTC.
        /// </summary>
        public ZonedDateTime StartOfDay => new ZonedDateTime(this.DateUtc, DateTimeZone.Utc, Offset.Zero);

        /// <summary>
        /// Return a new <see cref="DateKey"/> parsed from the given string.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The parsed date key.</returns>
        public static DateKey FromString(string value)
        {
            var parts = value.Split("-", 3);
            return new DateKey(
                Convert.ToInt32(parts[0]),
                Convert.ToInt32(parts[1]),
                Convert.ToInt32(parts[2]));
        }

        /// <summary>
        /// Compares the given <see cref="DateKey"/> to this <see cref="DateKey"/>.
        /// </summary>
        /// <param name="other">The other date key to compare.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(DateKey other)
        {
            return this.DateUtc.CompareTo(other.DateUtc);
        }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object? other) => other is DateKey dateKey && this.Equals(dateKey);

        /// <summary>
        /// Returns a value indicating whether this <see cref="DateKey"/> is equal to the given <see cref="DateKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="DateKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(DateKey other)
        {
            return this.Year == other.Year &&
                   this.Month == other.Month &&
                   this.Day == other.Day;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="DateKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Year, this.Month, this.Day);
        }

        /// <summary>
        /// Returns an ISO 8601 string representation of the <see cref="DateKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Year:D4}-{this.Month:D2}-{this.Day:D2}";
    }
}
