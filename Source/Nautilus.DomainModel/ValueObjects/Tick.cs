//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// Represents a financial market tick/quote.
    /// </summary>
    [Immutable]
    public sealed class Tick : ValueObject<Tick>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tick"/> class.
        /// </summary>
        /// <param name="symbol">The quoted symbol.</param>
        /// <param name="bid">The quoted bid price.</param>
        /// <param name="ask">The quoted ask price.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// timestamp is the default struct value.</exception>
        public Tick(
            Symbol symbol,
            Price bid,
            Price ask,
            ZonedDateTime timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(bid, nameof(bid));
            Validate.NotNull(ask, nameof(ask));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.Bid = bid;
            this.Ask = ask;
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
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(Tick)}({this.Symbol})";

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
                       {
                           this.Symbol,
                           this.Bid,
                           this.Ask,
                           this.Timestamp
                       };
        }
    }
}
