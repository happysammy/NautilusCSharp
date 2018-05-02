// -------------------------------------------------------------------------------------------------
// <copyright file="BarData.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Types
{
    using System;
    using System.Globalization;
    using System.Text;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using Nautilus.Core.Extensions;

    /// <summary>
    /// Represents a financial market trade bar.
    /// </summary>
    /// <remarks>Equality and comparison only based on the bars timestamp for high speed sorting
    /// by timestamp.</remarks>
    [Immutable]
    public struct BarData : IComparable<BarData>, IEquatable<BarData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarData"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        /// <param name="volume">The trading volume.</param>
        public BarData(
            decimal open,
            decimal high,
            decimal low,
            decimal close,
            long volume,
            ZonedDateTime timestamp)
        {
            Debug.DecimalNotOutOfRange(open, nameof(open), decimal.Zero, decimal.MaxValue);
            Debug.DecimalNotOutOfRange(high, nameof(high), decimal.Zero, decimal.MaxValue);
            Debug.DecimalNotOutOfRange(low, nameof(low), decimal.Zero, decimal.MaxValue);
            Debug.DecimalNotOutOfRange(close, nameof(close), decimal.Zero, decimal.MaxValue);
            Debug.LongNotOutOfRange(volume, nameof(volume), 0, long.MaxValue);
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the bar datas open price.
        /// </summary>
        public decimal Open { get; }

        /// <summary>
        /// Gets the bar datas high price.
        /// </summary>
        public decimal High { get; }

        /// <summary>
        /// Gets the bar datas low price.
        /// </summary>
        public decimal Low { get; }

        /// <summary>
        /// Gets the bar datas close price.
        /// </summary>
        public decimal Close { get; }

        /// <summary>
        /// Gets the bar datas volume.
        /// </summary>
        public long Volume { get; }

        /// <summary>
        /// Gets the bar datas timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a result indicating whether the left <see cref="BarData"/> is equal to the right
        /// <see cref="BarData"/>.
        /// </summary>
        /// <param name="left">The left bar.</param>
        /// <param name="right">The right bar.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(BarData left, BarData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a result indicating whether the left <see cref="BarData"/> is NOT equal to the
        /// right <see cref="BarData"/>.
        /// </summary>
        /// <param name="left">The left bar.</param>
        /// <param name="right">The right bar.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(BarData left, BarData right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a result indicating whether the left <see cref="BarData"/> is less than, equal
        /// to or greater than the right <see cref="BarData"/>.
        /// </summary>
        /// <param name="other">The other bar to compare.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public int CompareTo(BarData other)
        {
            Debug.NotDefault(other, nameof(other));

            return this.Timestamp.Compare(other.Timestamp);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="object"/> is equal to the given
        /// <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other)
        {
            return other is BarData barData && this.Equals(barData);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarData"/> is equal to the given
        /// <see cref="BarData"/>.
        /// </summary>
        /// <param name="other">The other bar.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarData other)
        {
            return this.Timestamp.Equals(other.Timestamp);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarData"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.Timestamp.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarData"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return this.Timestamp.ToIsoString() + "," +
                   this.Open.ToString(CultureInfo.InvariantCulture) + "," +
                   this.High.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Low.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Close.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Volume.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a valid <see cref="string"/> of values from this <see cref="BarData"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ValuesToString()
        {
            return this.Open.ToString(CultureInfo.InvariantCulture) + "," +
                   this.High.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Low.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Close.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Volume.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a valid <see cref="byte"/> array from this <see cref="BarData"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] ToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString());
        }

        /// <summary>
        /// Returns a valid <see cref="byte"/> array of values from this <see cref="BarData"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] ValuesToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(this.ValuesToString());
        }
    }
}
