// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataKey.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Keys
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a Redis Key based on the given tick specification.
    /// </summary>
    [Immutable]
    public struct TickDataKey : IEquatable<TickDataKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataKey"/> struct. The bar period
        /// must be 1 for a valid key.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="dateKey">The date key the key is based on.</param>
        /// <exception cref="ValidationException">Throws if the bar period != 1.</exception>
        public TickDataKey(Symbol symbol, DateKey dateKey)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotDefault(dateKey, nameof(dateKey));

            this.Symbol = symbol;
            this.DateKey = dateKey;
        }

        /// <summary>
        /// Gets the <see cref="TickDataKey"/> symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the <see cref="TickDataKey"/> date key.
        /// </summary>
        public DateKey DateKey { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other)
        {
            return other is TickDataKey key && this.Equals(key);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="TickDataKey"/> is equal to the given
        /// <see cref="TickDataKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="TickDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(TickDataKey other)
        {
            // Do not add null check (causes error).

            return this.Symbol.Equals(other.Symbol) &&
                   this.DateKey.Equals(other.DateKey);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="TickDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.Symbol.GetHashCode() +
                   this.DateKey.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="TickDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() =>
            KeyProvider.TicksNamespace +
            $":{this.Symbol.Exchange.ToString().ToLower()}" +
            $":{this.Symbol.Code.ToLower()}" +
            $":{this.DateKey}";
    }
}
