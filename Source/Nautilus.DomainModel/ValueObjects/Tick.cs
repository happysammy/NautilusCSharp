//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using NodaTime;

    /// <summary>
    /// Represents a financial market tick.
    /// </summary>
    [Immutable]
    public struct Tick : IEquatable<object>, IEquatable<Tick>, IComparable<Tick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> structure.
        /// </summary>
        /// <param name="symbol">The quoted symbol.</param>
        /// <param name="bid">The quoted bid price.</param>
        /// <param name="ask">The quoted ask price.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        public Tick(
            Symbol symbol,
            Price bid,
            Price ask,
            ZonedDateTime timestamp)
        {
            this.Symbol = symbol;
            this.Bid = bid;
            this.Ask = ask;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> structure.
        /// </summary>
        /// <param name="symbol">The quoted symbol.</param>
        /// <param name="bid">The quoted bid price.</param>
        /// <param name="ask">The quoted ask price.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        public Tick(
            Symbol symbol,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Symbol = symbol;
            this.Bid = Price.Create(bid, bid.GetDecimalPlaces());
            this.Ask = Price.Create(ask, ask.GetDecimalPlaces());
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
        /// Returns a value indicating whether this <see cref="Tick"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Tick tick && this.Equals(tick);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Tick"/> is equal
        /// to the given <see cref="Tick"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Tick other)
        {
            return this.Symbol == other.Symbol &&
                   this.Bid == other.Bid &&
                   this.Ask == other.Ask &&
                   this.Timestamp == other.Timestamp;
        }

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Tick"/> is less than, equal
        /// to or greater than the right <see cref="Tick"/>.
        /// </summary>
        /// <param name="other">The other tick to compare.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public int CompareTo(Tick other) => this.Timestamp.Compare(other.Timestamp);

        /// <summary>
        /// Returns the hash code of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Symbol, this.Bid, this.Ask);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Bid},{this.Ask},{this.Timestamp.ToIsoString()}";
    }
}
