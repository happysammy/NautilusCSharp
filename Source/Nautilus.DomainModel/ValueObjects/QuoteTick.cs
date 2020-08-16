//--------------------------------------------------------------------------------------------------
// <copyright file="QuoteTick.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a financial market tick.
    /// </summary>
    [Immutable]
    public sealed class QuoteTick : Tick, IEquatable<object>, IEquatable<QuoteTick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteTick"/> class.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="bid">The best quoted bid price.</param>
        /// <param name="ask">The best quoted ask price.</param>
        /// <param name="bidSize">The tick bid size.</param>
        /// <param name="askSize">The tick ask size.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        public QuoteTick(
            Symbol symbol,
            Price bid,
            Price ask,
            Quantity bidSize,
            Quantity askSize,
            ZonedDateTime timestamp)
            : base(symbol, TickSpecification.Quote, timestamp)
        {
            this.Bid = bid;
            this.Ask = ask;
            this.BidSize = bidSize;
            this.AskSize = askSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The quoted symbol.</param>
        /// <param name="bid">The quoted bid price.</param>
        /// <param name="ask">The quoted ask price.</param>
        /// <param name="bidSize">The ticks bid size.</param>
        /// <param name="askSize">The ticks ask size.</param>
        /// <param name="unixTimestamp">The unix timestamp in milliseconds.</param>
        public QuoteTick(
            Symbol symbol,
            Price bid,
            Price ask,
            Quantity bidSize,
            Quantity askSize,
            long unixTimestamp)
            : base(symbol, TickSpecification.Quote, unixTimestamp)
        {
            this.Bid = bid;
            this.Ask = ask;
            this.BidSize = bidSize;
            this.AskSize = askSize;
        }

        /// <summary>
        /// Gets the ticks bid price.
        /// </summary>
        public Price Bid { get; }

        /// <summary>
        /// Gets the ticks ask price.
        /// </summary>
        public Price Ask { get; }

        /// <summary>
        /// Gets the ticks bid size.
        /// </summary>
        public Quantity BidSize { get; }

        /// <summary>
        /// Gets the ticks ask size.
        /// </summary>
        public Quantity AskSize { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="QuoteTick"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(QuoteTick left, QuoteTick right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="QuoteTick"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(QuoteTick left,  QuoteTick right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbol">The symbol to create the tick with.</param>
        /// <param name="values">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static QuoteTick FromSerializableString(Symbol symbol, string values)
        {
            Debug.NotEmptyOrWhiteSpace(values, nameof(values));

            var pieces = values.Split(',', 5);

            return new QuoteTick(
                symbol,
                Price.Create(pieces[0]),
                Price.Create(pieces[1]),
                Quantity.Create(pieces[2]),
                Quantity.Create(pieces[3]),
                Instant.FromUnixTimeMilliseconds(Convert.ToInt64(pieces[4])).InUtc());
        }

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is QuoteTick tick && this.Equals(tick);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <inheritdoc />
        public bool Equals(QuoteTick other)
        {
            return this.Symbol == other.Symbol &&
                   this.Bid == other.Bid &&
                   this.Ask == other.Ask &&
                   this.BidSize == other.BidSize &&
                   this.AskSize == other.AskSize &&
                   this.Timestamp == other.Timestamp;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Symbol, this.Timestamp);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="QuoteTick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Symbol},{this.Bid},{this.Ask},{this.BidSize},{this.AskSize},{this.Timestamp.ToIso8601String()}";

        /// <summary>
        /// Returns a serializable string representation of the <see cref="QuoteTick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ToSerializableString() => $"{this.Bid},{this.Ask},{this.BidSize},{this.AskSize},{this.Timestamp.ToInstant().ToUnixTimeMilliseconds()}";
    }
}
