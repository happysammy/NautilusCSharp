//--------------------------------------------------------------------------------------------------
// <copyright file="BarType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a bar type including a <see cref="Symbol"/> and <see cref="BarSpecification"/>.
    /// </summary>
    [Immutable]
    public sealed class BarType : IEquatable<object>, IEquatable<BarType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarType"/> class.
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
        public override bool Equals(object? other) => other is BarType barType && this.Equals(barType);

#pragma warning disable CS8767
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
            return $"{this.Symbol.Value}-{this.Specification}";
        }
    }
}
