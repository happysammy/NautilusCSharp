//--------------------------------------------------------------------------------------------------
// <copyright file="Identifier.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;

namespace Nautilus.Core.Types
{
    /// <summary>
    /// The base class for all identifiers.
    /// </summary>
    /// <typeparam name="T">The identifier type.</typeparam>
    [Immutable]
    public abstract class Identifier<T> : IEquatable<object>, IEquatable<Identifier<T>>, IComparable<Identifier<T>>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifier{T}"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        protected Identifier(string value)
        {
            Condition.NotEmptyOrWhiteSpace(value, nameof(value));

            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the identifier.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Identifier{T}"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Identifier<T> left, Identifier<T> right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Identifier{T}"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Identifier<T> left, Identifier<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Identifier{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object? other) => other is Identifier<T> identifier && this.Equals(identifier);

#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="Identifier{T}"/> is equal
        /// to the given <see cref="Identifier{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Identifier<T> other) => this.Value == other.Value;

        /// <inheritdoc />
        public int CompareTo(Identifier<T> other)
        {
            return string.Compare(this.Value, other.Value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="Identifier{T}"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{typeof(T).Name}({this.Value})";
    }
}
