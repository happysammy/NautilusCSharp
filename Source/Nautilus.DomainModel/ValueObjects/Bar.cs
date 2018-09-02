//--------------------------------------------------------------------------------------------------
// <copyright file="Bar.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects.Base;
    using NodaTime;

    /// <summary>
    /// Represents a financial market trade bar.
    /// </summary>
    [Immutable]
    public sealed class Bar : ValueObject<Bar>, IEquatable<Bar>, IComparable<Bar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bar"/> class.
        /// </summary>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Bar(
            Price open,
            Price high,
            Price low,
            Price close,
            Quantity volume,
            ZonedDateTime timestamp)
        {
            Debug.NotNull(open, nameof(open));
            Debug.NotNull(high, nameof(high));
            Debug.NotNull(low, nameof(low));
            Debug.NotNull(close, nameof(close));
            Debug.NotNull(volume, nameof(volume));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bar"/> class.
        /// </summary>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Bar(
            decimal open,
            decimal high,
            decimal low,
            decimal close,
            int volume,
            ZonedDateTime timestamp)
        {
            Debug.NotNull(volume, nameof(volume));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Open = Price.Create(open, open.GetDecimalPlaces());
            this.High = Price.Create(high, open.GetDecimalPlaces());
            this.Low = Price.Create(low, open.GetDecimalPlaces());
            this.Close = Price.Create(close, open.GetDecimalPlaces());
            this.Volume = Quantity.Create(volume);
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the bars open <see cref="Price"/>.
        /// </summary>
        public Price Open { get; }

        /// <summary>
        /// Gets the bars high <see cref="Price"/>.
        /// </summary>
        public Price High { get; }

        /// <summary>
        /// Gets the bars low <see cref="Price"/>.
        /// </summary>
        public Price Low { get; }

        /// <summary>
        /// Gets the bars close <see cref="Price"/>.
        /// </summary>
        public Price Close { get; }

        /// <summary>
        /// Gets the bars volume.
        /// </summary>
        public Quantity Volume { get; }

        /// <summary>
        /// Gets the bars timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="string"/>.
        /// </summary>
        /// <param name="barString">The bar string.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public static Bar GetFromString(string barString)
        {
            Debug.NotNull(barString, nameof(barString));

            var values = barString.Split(',');
            var decimals = SafeConvert.ToDecimalOr(values[0], 0m).GetDecimalPlaces();

            return new Bar(
                Price.Create(SafeConvert.ToDecimalOr(values[0], 0m), decimals),
                Price.Create(SafeConvert.ToDecimalOr(values[1], 0m), decimals),
                Price.Create(SafeConvert.ToDecimalOr(values[2], 0m), decimals),
                Price.Create(SafeConvert.ToDecimalOr(values[3], 0m), decimals),
                Quantity.Create(Convert.ToInt32(SafeConvert.ToDecimalOr(values[4], 0m))),
                values[5].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="byte"/> array.
        /// </summary>
        /// <param name="barBytes">The bar bytes array.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static Bar GetFromBytes(byte[] barBytes)
        {
            Debug.NotNullOrEmpty(barBytes, nameof(barBytes));

            return GetFromString(Encoding.UTF8.GetString(barBytes));
        }

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Bar"/> is less than, equal
        /// to or greater than the right <see cref="Bar"/>.
        /// </summary>
        /// <param name="other">The other bar to compare.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public int CompareTo(Bar other)
        {
            Debug.NotNull(other, nameof(other));

            return this.Timestamp.Compare(other.Timestamp);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return this.Open + "," +
                   this.High + "," +
                   this.Low + "," +
                   this.Close + "," +
                   this.Volume + "," +
                   this.Timestamp.ToIsoString();
        }

        /// <summary>
        /// Returns a valid <see cref="string"/> of values from this <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ValuesToString()
        {
            return this.Open + "," +
                   this.High + "," +
                   this.Low + "," +
                   this.Close + "," +
                   this.Volume;
        }

        /// <summary>
        /// Returns a valid <see cref="byte"/> array from this <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] ToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString());
        }

        /// <summary>
        /// Returns a valid <see cref="byte"/> array of values from this <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] ValuesToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(this.ValuesToString());
        }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
            {
                this.Open,
                this.High,
                this.Low,
                this.Close,
                this.Volume,
                this.Timestamp,
            };
        }
    }
}
