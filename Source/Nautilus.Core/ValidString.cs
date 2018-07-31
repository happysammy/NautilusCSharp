//--------------------------------------------------------------------------------------------------
// <copyright file="ValidString.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides an encapsulation for a validated string. A valid string is not null, less than
    /// or equal to 100 characters, and contains no white space.
    /// </summary>
    [Immutable]
    public class ValidString : IEquatable<ValidString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidString"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        public ValidString(string value)
        {
            Debug.NotNull(value, nameof(value));
            Debug.True(value.Length <= 100, nameof(value));

            this.Value = value.RemoveAllWhitespace();
        }

        /// <summary>
        /// Gets the value of the <see cref="ValidString"/>.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="ValidString"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(
            [CanBeNull] ValidString left,
            [CanBeNull] ValidString right)
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
        /// Returns a value indicating whether the <see cref="ValidString"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(
            [CanBeNull] ValidString left,
            [CanBeNull] ValidString right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValidString"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as ValidString);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValidString"/> is equal
        /// to the given <see cref="ValidString"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals([CanBeNull] ValidString other)
        {
            return other != null && this.Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="ValidString"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="ValidString"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
