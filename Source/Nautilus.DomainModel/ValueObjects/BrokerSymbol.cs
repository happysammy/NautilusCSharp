//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerSymbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a broker symbol.
    /// </summary>
    [Immutable]
    public struct BrokerSymbol : IEquatable<object>, IEquatable<BrokerSymbol>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerSymbol"/> structure.
        /// </summary>
        /// <param name="symbol">The broker symbol.</param>
        public BrokerSymbol(string symbol)
        {
            Debug.NotEmptyOrWhiteSpace(symbol, nameof(symbol));

            this.value = symbol;
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BrokerSymbol"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(BrokerSymbol left, BrokerSymbol right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BrokerSymbol"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(BrokerSymbol left,  BrokerSymbol right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BrokerSymbol"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is BrokerSymbol symbol && this.Equals(symbol);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BrokerSymbol"/> is equal
        /// to the given <see cref="BrokerSymbol"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BrokerSymbol other)
        {
            return this.value == other.value;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BrokerSymbol"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.value);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BrokerSymbol"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.value;
    }
}
