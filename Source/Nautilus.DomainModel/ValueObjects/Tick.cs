//--------------------------------------------------------------------------------------------------
// <copyright file="Tick.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.DomainModel.Enums;
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
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// timestamp is the default struct value.</exception>
        public Tick(
            Symbol symbol,
            Price bid,
            Price ask,
            ZonedDateTime timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(bid, nameof(bid));
            Debug.NotNull(ask, nameof(ask));
            Debug.NotDefault(timestamp, nameof(timestamp));

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
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// timestamp is the default struct value.</exception>
        public Tick(
            Symbol symbol,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(bid, nameof(bid));
            Debug.NotNull(ask, nameof(ask));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.Bid = Price.Create(bid, bid.GetDecimalPlaces());
            this.Ask = Price.Create(bid, bid.GetDecimalPlaces());
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
        public int CompareTo(Tick other)
        {
            Debug.NotNull(other, nameof(other));

            return this.Timestamp.Compare(other.Timestamp);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Symbol},{this.Bid},{this.Ask},{this.Timestamp.ToIsoString()}";

        /// <summary>
        /// Returns a string representation of the <see cref="Tick"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public string ToChannel() => $"{this.Bid},{this.Ask},{this.Timestamp.ToIsoString()}";

        /// <summary>
        /// Returns a valid <see cref="byte"/> array from this <see cref="Bar"/>.
        /// </summary>
        /// <returns>A <see cref="byte"/> array.</returns>
        public byte[] ToUtf8Bytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString());
        }

        /// <summary>
        /// Returns a valid <see cref="Tick"/> from this <see cref="string"/>.
        /// </summary>
        /// <param name="tickString">The tick string.</param>
        /// <returns>A <see cref="Tick"/>.</returns>
        public static Tick GetFromString(string tickString)
        {
            Debug.NotNull(tickString, nameof(tickString));

            var values = tickString.Split(',');
            var header = values[0].Split('.');
            var code = header[0];
            var exchange = header[1];

            return new Tick(
                new Symbol(code, exchange.ToEnum<Exchange>()),
                SafeConvert.ToDecimalOr(values[1], 0m),
                SafeConvert.ToDecimalOr(values[2], 0m),
                values[3].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a valid <see cref="Tick"/> from this <see cref="byte"/> array.
        /// </summary>
        /// <param name="tickBytes">The tick bytes array.</param>
        /// <returns>A <see cref="Tick"/>.</returns>
        public static Tick GetFromBytes(byte[] tickBytes)
        {
            Debug.CollectionNotNullOrEmpty(tickBytes, nameof(tickBytes));

            return GetFromString(Encoding.UTF8.GetString(tickBytes));
        }

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
