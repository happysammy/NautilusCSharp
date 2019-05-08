//--------------------------------------------------------------------------------------------------
// <copyright file="Bar.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Text;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
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
        public Bar(
            Price open,
            Price high,
            Price low,
            Price close,
            Quantity volume,
            ZonedDateTime timestamp)
        {
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
        public Bar(
            decimal open,
            decimal high,
            decimal low,
            decimal close,
            int volume,
            ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Open = Price.Create(open);
            this.High = Price.Create(high);
            this.Low = Price.Create(low);
            this.Close = Price.Create(close);
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
            Debug.NotEmptyOrWhiteSpace(barString, nameof(barString));

            var values = barString.Split(',');
            var decimals = values[0].ToDecimalOr(0m).GetDecimalPlaces();

            return new Bar(
                Price.Create(values[0].ToDecimalOr(0m), decimals),
                Price.Create(values[1].ToDecimalOr(0m), decimals),
                Price.Create(values[2].ToDecimalOr(0m), decimals),
                Price.Create(values[3].ToDecimalOr(0m), decimals),
                Quantity.Create(Convert.ToInt32(values[4].ToDecimalOr(0m))),
                values[5].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a valid <see cref="Bar"/> from this <see cref="byte"/> array.
        /// </summary>
        /// <param name="barBytes">The bar bytes array.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public static Bar GetFromBytes(byte[] barBytes)
        {
            Debug.NotEmpty(barBytes, nameof(barBytes));

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
        /// Returns an array of objects to be included in equality checks.
        /// </summary>
        /// <returns>The array of equality members.</returns>
        protected override object[] GetEqualityArray()
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
