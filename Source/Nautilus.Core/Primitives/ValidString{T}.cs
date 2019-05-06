//--------------------------------------------------------------------------------------------------
// <copyright file="ValidString{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Primitives
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides an encapsulation for a validated string. A valid string is not null, less than
    /// or equal to 1024 characters, and contains no white space.
    /// </summary>
    /// <typeparam name="T">The valid string type.</typeparam>
    [Immutable]
    public abstract class ValidString<T> : IEquatable<ValidString<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidString{T}"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        protected ValidString(string value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
            Debug.NotOutOfRangeInt32(value.Length, 1, 1024, nameof(value));

            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the <see cref="ValidString{T}"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="ValidString{T}"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(ValidString<T> left, ValidString<T> right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="ValidString{T}"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(ValidString<T> left, ValidString<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValidString{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object other) => other is ValidString<T> && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValidString{T}"/> is equal
        /// to the given <see cref="ValidString{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(ValidString<T> other) => this.Value.Equals(other.Value);

        /// <summary>
        /// Returns the hash code for this <see cref="ValidString{T}"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="ValidString{T}"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
