//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects.Base;
    using NodaTime;

    /// <summary>
    /// Represents a financial market tick.
    /// </summary>
    [Immutable]
    public sealed class Tick : ValueObject<Tick>, IEquatable<Tick>, IComparable<Tick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
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
        /// Initializes a new instance of the <see cref="Tick"/> class.
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
        /// Returns a result indicating whether the left <see cref="Tick"/> is less than, equal
        /// to or greater than the right <see cref="Tick"/>.
        /// </summary>
        /// <param name="other">The other tick to compare.</param>
        /// <returns>An <see cref="int"/>.</returns>
        public int CompareTo(Tick other) => this.Timestamp.Compare(other.Timestamp);

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Bid},{this.Ask},{this.Timestamp.ToIsoString()}";

        /// <summary>
        /// Returns an array of objects to be included in equality checks.
        /// </summary>
        /// <returns>The array of equality members.</returns>
        protected override object[] GetEqualityArray()
        {
            return new object[]
                       {
                           this.Symbol,
                           this.Bid,
                           this.Ask,
                           this.Timestamp,
                       };
        }
    }
}
