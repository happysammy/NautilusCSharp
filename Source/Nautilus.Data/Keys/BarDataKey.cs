// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataKey.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Keys
{
    using System;
    using System.Diagnostics;
    using Nautilus.Core.Annotations;
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
        public BarDataKey(BarType barType, DateKey dateKey)
        {
            Debug.Assert(barType.Specification.Period == 1, "The bar type period must be 1.");

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
        public override bool Equals(object other) => other != null && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarDataKey"/> is equal to the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarDataKey other) => this.Type.Equals(other.Type) && this.DateKey.Equals(other.DateKey);

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
            $":{this.Type.Symbol.Venue.ToString().ToLower()}" +
            $":{this.Type.Symbol.Code.ToLower()}" +
            $":{this.Type.Specification.Resolution.ToString().ToLower()}" +
            $":{this.Type.Specification.QuoteType.ToString().ToLower()}" +
            $":{this.DateKey}";
    }
}
