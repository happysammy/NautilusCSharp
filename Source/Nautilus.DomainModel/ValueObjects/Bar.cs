//--------------------------------------------------------------------------------------------------
// <copyright file="Bar.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Extensions;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a financial market trade bar.
    /// </summary>
    [Immutable]
    public sealed class Bar : IEquatable<object>, IEquatable<Bar>
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
        /// <param name="unixTimestamp">The unix timestamp in milliseconds.</param>
        public Bar(
            Price open,
            Price high,
            Price low,
            Price close,
            Quantity volume,
            long unixTimestamp)
        {
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            this.Timestamp = Instant.FromUnixTimeMilliseconds(unixTimestamp).InUtc();
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
        /// Returns a value indicating whether the <see cref="Bar"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Bar left, Bar right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Bar"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Bar left,  Bar right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Bar"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barString">The bar string.</param>
        /// <returns>The created <see cref="Bar"/>.</returns>
        public static Bar FromSerializableString(string barString)
        {
            Debug.NotEmptyOrWhiteSpace(barString, nameof(barString));

            var values = barString.Split(',', 6);

            return new Bar(
                Price.Create(Parser.ToDecimal(values[0])),
                Price.Create(Parser.ToDecimal(values[1])),
                Price.Create(Parser.ToDecimal(values[2])),
                Price.Create(Parser.ToDecimal(values[3])),
                Quantity.Create(Parser.ToDecimal(values[4])),
                Instant.FromUnixTimeMilliseconds(Convert.ToInt64(values[5])).InUtc());
        }

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is Bar bar && this.Equals(bar);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="Bar"/> is equal
        /// to the given <see cref="Bar"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Bar other)
        {
            return this.Open == other.Open &&
                   this.High == other.High &&
                   this.Low == other.Low &&
                   this.Close == other.Close &&
                   this.Volume == other.Volume &&
                   this.Timestamp == other.Timestamp;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="Bar"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(
                this.Open,
                this.High,
                this.Low,
                this.Close,
                this.Volume,
                this.Timestamp);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Open},{this.High},{this.Low},{this.Close},{this.Volume},{this.Timestamp.ToIso8601String()}";
        }

        /// <summary>
        /// Returns a serializable string representation of the <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ToSerializableString()
        {
            return $"{this.Open},{this.High},{this.Low},{this.Close},{this.Volume},{this.Timestamp.ToInstant().ToUnixTimeMilliseconds()}";
        }
    }
}
