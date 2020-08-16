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
    public sealed class TradeTick : Tick, IEquatable<object>, IEquatable<TradeTick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteTick"/> class.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="price">The best quoted bid price.</param>
        /// <param name="size">The best quoted ask price.</param>
        /// <param name="maker">The tick bid size.</param>
        /// <param name="matchId">The tick ask size.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        public TradeTick(
            Symbol symbol,
            Price price,
            Quantity size,
            Maker maker,
            MatchId matchId,
            ZonedDateTime timestamp)
            : base(symbol, TickSpecification.Quote, timestamp)
        {
            this.Price = price;
            this.Size = size;
            this.Maker = maker;
            this.MatchId = matchId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="price">The best quoted bid price.</param>
        /// <param name="size">The best quoted ask price.</param>
        /// <param name="maker">The tick bid size.</param>
        /// <param name="matchId">The tick ask size.</param>
        /// <param name="unixTimestamp">The unix timestamp in milliseconds.</param>
        public TradeTick(
            Symbol symbol,
            Price price,
            Quantity size,
            Maker maker,
            MatchId matchId,
            long unixTimestamp)
            : base(symbol, TickSpecification.Quote, unixTimestamp)
        {
            this.Price = price;
            this.Size = size;
            this.Maker = maker;
            this.MatchId = matchId;
        }

        /// <summary>
        /// Gets the ticks price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the ticks size.
        /// </summary>
        public Quantity Size { get; }

        /// <summary>
        /// Gets the ticks trade size.
        /// </summary>
        public Maker Maker { get; }

        /// <summary>
        /// Gets the ticks ask size.
        /// </summary>
        public MatchId MatchId { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="TradeTick"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(TradeTick left, TradeTick right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="TradeTick"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(TradeTick left,  TradeTick right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbol">The symbol to create the tick with.</param>
        /// <param name="values">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static TradeTick FromSerializableString(Symbol symbol, string values)
        {
            Debug.NotEmptyOrWhiteSpace(values, nameof(values));

            var pieces = values.Split(',', 5);

            return new TradeTick(
                symbol,
                Price.Create(pieces[0]),
                Quantity.Create(pieces[1]),
                pieces[2].ToEnum<Maker>(),
                new MatchId(pieces[3]),
                Instant.FromUnixTimeMilliseconds(Convert.ToInt64(pieces[4])).InUtc());
        }

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is TradeTick tick && this.Equals(tick);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <inheritdoc />
        public bool Equals(TradeTick other)
        {
            return this.Symbol == other.Symbol &&
                   this.Price == other.Price &&
                   this.Size == other.Size &&
                   this.Maker == other.Maker &&
                   this.MatchId == other.MatchId &&
                   this.Timestamp == other.Timestamp;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Symbol, this.Timestamp);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Symbol},{this.Price},{this.Size},{this.Maker},{this.MatchId},{this.Timestamp.ToIso8601String()}";

        /// <summary>
        /// Returns a serializable string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ToSerializableString() => $"{this.Price},{this.Size},{this.Maker},{this.MatchId},{this.Timestamp.ToInstant().ToUnixTimeMilliseconds()}";
    }
}
