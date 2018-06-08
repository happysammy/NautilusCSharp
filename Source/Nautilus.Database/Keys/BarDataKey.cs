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
    using Nautilus.Database.Types;

    /// <summary>
    /// Represents a Redis Key based on the given symbol and bar specification.
    /// </summary>
    [Immutable]
    public struct BarDataKey : IEquatable<BarDataKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataKey"/> struct. The bar period
        /// must be 1 for a valid key.
        /// </summary>
        /// <param name="barSpec">The bar specification the key is based on.</param>
        /// <param name="dateKey">The date key the key is based on.</param>
        /// <exception cref="ValidationException">Throws if the bar period != 1.</exception>
        public BarDataKey(SymbolBarSpec barSpec, DateKey dateKey)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(dateKey, nameof(dateKey));
            Validate.EqualTo(1, nameof(barSpec.BarSpecification.Period), barSpec.BarSpecification.Period);

            this.BarSpecification = barSpec;
            this.DateKey = dateKey;
        }

        /// <summary>
        /// Gets the <see cref="BarDataKey"/>(s) bar specification.
        /// </summary>
        public SymbolBarSpec BarSpecification { get; }

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
            return this.BarSpecification.Equals(other.BarSpecification) &&
                   this.DateKey.Equals(other.DateKey);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.BarSpecification.GetHashCode() +
                   this.DateKey.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() =>
            KeyProvider.BarsNamespace +
            $":{this.BarSpecification.Symbol.Exchange.ToString().ToLower()}" +
            $":{this.BarSpecification.Symbol.Code.ToLower()}" +
            $":{this.BarSpecification.BarSpecification.Resolution.ToString().ToLower()}" +
            $":{this.BarSpecification.BarSpecification.QuoteType.ToString().ToLower()}" +
            $":{this.DateKey}";
    }
}
