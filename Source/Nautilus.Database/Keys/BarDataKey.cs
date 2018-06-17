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
    /// Represents a Redis Key based on the given <see cref="BarType"/> and <see cref="DateKey"/>.
    /// </summary>
    [Immutable]
    public struct BarDataKey : IEquatable<BarDataKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataKey"/> struct. The bar period
        /// must be 1 for a valid key.
        /// </summary>
        /// <param name="barType">The bar specification the key is based on.</param>
        /// <param name="dateKey">The date key the key is based on.</param>
        /// <exception cref="ValidationException">Throws if the bar period != 1.</exception>
        public BarDataKey(BarType barType, DateKey dateKey)
        {
            Debug.NotNull(barType, nameof(barType));
            Debug.NotDefault(dateKey, nameof(dateKey));
            Debug.EqualTo(1, nameof(barType.Specification.Period), barType.Specification.Period);

            this.Type = barType;
            this.DateKey = dateKey;
        }

        /// <summary>
        /// Gets the <see cref="BarDataKey"/>(s) bar specification.
        /// </summary>
        public BarType Type { get; }

        /// <summary>
        /// Gets the <see cref="BarDataKey"/>(s) date key.
        /// </summary>
        public DateKey DateKey { get; }

        /// <summary>
        /// Returns a value indicating whether this entity is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other)
        {
            return other is BarDataKey key && this.Equals(key);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarDataKey"/> is equal to the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarDataKey other)
        {
            // Do not add null check (causes error).
            return this.Type.Equals(other.Type) &&
                   this.DateKey.Equals(other.DateKey);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.Type.GetHashCode() +
                   this.DateKey.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() =>
            KeyProvider.BarsNamespace +
            $":{this.Type.Symbol.Exchange.ToString().ToLower()}" +
            $":{this.Type.Symbol.Code.ToLower()}" +
            $":{this.Type.Specification.Resolution.ToString().ToLower()}" +
            $":{this.Type.Specification.QuoteType.ToString().ToLower()}" +
            $":{this.DateKey}";
    }
}
