//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Identifiers;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a financial market tick.
    /// </summary>
    [Immutable]
    public sealed class Tick : IEquatable<object>, IEquatable<Tick>, IComparable<Tick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The quoted symbol.</param>
        /// <param name="bid">The quoted bid price.</param>
        /// <param name="ask">The quoted ask price.</param>
        /// <param name="bidSize">The ticks bid size.</param>
        /// <param name="askSize">The ticks ask size.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        public Tick(
            Symbol symbol,
            Price bid,
            Price ask,
            Volume bidSize,
            Volume askSize,
            ZonedDateTime timestamp)
        {
            this.Symbol = symbol;
            this.Bid = bid;
            this.Ask = ask;
            this.BidSize = bidSize;
            this.AskSize = askSize;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the ticks symbol.
        /// </summary>
        public Symbol Symbol { get; }

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
        public Volume BidSize { get; }

        /// <summary>
        /// Gets the ticks ask size.
        /// </summary>
        public Volume AskSize { get; }

        /// <summary>
        /// Gets the ticks timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Tick"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Tick left, Tick right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Tick"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Tick left,  Tick right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="tickString">The tick string which includes the symbol.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static Tick FromStringWhichIncludesSymbol(string tickString)
        {
            Debug.NotEmptyOrWhiteSpace(tickString, nameof(tickString));

            var values = tickString.Split(',');
            return FromString(Symbol.FromString(values[0]), values[1]);
        }

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbol">The symbol to create the tick with.</param>
        /// <param name="tickString">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static Tick FromString(Symbol symbol, string tickString)
        {
            Debug.NotEmptyOrWhiteSpace(tickString, nameof(tickString));

            var values = tickString.Split(',');

            return new Tick(
                symbol,
                Price.Create(values[0]),
                Price.Create(values[1]),
                Volume.Create(values[2]),
                Volume.Create(values[3]),
                values[4].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="Tick"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object? other) => other is Tick tick && this.Equals(tick);

#pragma warning disable CS8767
        /// <inheritdoc />
        public bool Equals(Tick other)
        {
            return this.Symbol == other.Symbol &&
                   this.Bid == other.Bid &&
                   this.Ask == other.Ask &&
                   this.BidSize == other.BidSize &&
                   this.AskSize == other.AskSize &&
                   this.Timestamp == other.Timestamp;
        }

#pragma warning disable CS8767
        /// <inheritdoc />
        public int CompareTo(Tick other) => this.Timestamp.Compare(other.Timestamp);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Symbol, this.Bid, this.Ask);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Bid},{this.Ask},{this.BidSize},{this.AskSize},{this.Timestamp.ToIso8601String()}";
    }
}
