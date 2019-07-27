//--------------------------------------------------------------------------------------------------
// <copyright file="Label.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a validated label.
    /// </summary>
    [Immutable]
    public struct Label : IEquatable<object>, IEquatable<Label>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> structure.
        /// </summary>
        /// <param name="value">The label value.</param>
        public Label(string value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));

            this.value = value;
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Label"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Label left, Label right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Label"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Label left,  Label right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Label"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Label symbol && this.Equals(symbol);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Label"/> is equal
        /// to the given <see cref="Label"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Label other)
        {
            return this.value == other.value;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="Label"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.value);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Label"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.value;
    }
}
