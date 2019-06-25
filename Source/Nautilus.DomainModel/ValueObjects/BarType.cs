//--------------------------------------------------------------------------------------------------
// <copyright file="BarType.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core;

    /// <summary>
    /// Represents a bar type including a <see cref="Symbol"/> and <see cref="BarSpecification"/>.
    /// </summary>
    public struct BarType : IEquatable<object>, IEquatable<BarType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarType"/> structure.
        /// </summary>
        /// <param name="symbol">The bar type symbol.</param>
        /// <param name="specification">The bar type specification.</param>
        public BarType(
            Symbol symbol,
            BarSpecification specification)
        {
            this.Symbol = symbol;
            this.Specification = specification;
        }

        /// <summary>
        /// Gets the bar types symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the bar types specification.
        /// </summary>
        public BarSpecification Specification { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BarType"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(BarType left, BarType right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="BarType"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(BarType left,  BarType right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarType"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is BarType barType && this.Equals(barType);

        /// <summary>
        /// Returns a value indicating whether this <see cref="BarType"/> is equal
        /// to the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(BarType other)
        {
            return this.Symbol.Equals(other.Symbol) && this.Specification.Equals(other.Specification);
        }

        /// <summary>
        /// Returns the hash code of the <see cref="BarType"/>.
        /// </summary>
        /// <remarks>Non-readonly properties referenced in GetHashCode for serialization.</remarks>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.Symbol, this.Specification);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="BarType"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Symbol}-{this.Specification}";
        }
    }
}
