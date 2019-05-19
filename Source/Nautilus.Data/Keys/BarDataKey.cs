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
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
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
            Condition.EqualTo(barType.Specification.Period, 1, nameof(barType.Specification.Period));

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
        public override bool Equals(object other) => other is BarDataKey dataKey && this.Equals(dataKey);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarDataKey"/> is equal to the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="other">The other <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarDataKey other) => this.Type == other.Type && this.DateKey.Equals(other.DateKey);

        /// <summary>
        /// Returns the hash code of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Type, this.DateKey);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarDataKey"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() =>
            KeyProvider.BarsNamespace +
            $":{this.Type.Symbol.Venue}" +
            $":{this.Type.Symbol.Code}" +
            $":{this.Type.Specification.Resolution}" +
            $":{this.Type.Specification.QuoteType}" +
            $":{this.DateKey}";
    }
}
