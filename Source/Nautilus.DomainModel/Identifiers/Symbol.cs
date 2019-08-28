//--------------------------------------------------------------------------------------------------
// <copyright file="Symbol.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents a financial market instruments symbol.
    /// </summary>
    [Immutable]
    public struct Symbol : IEquatable<object>, IEquatable<Symbol>
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> structure.
        /// </summary>
        /// <param name="code">The symbols code.</param>
        /// <param name="venue">The symbols venue.</param>
        public Symbol(string code, Venue venue)
        {
            Debug.NotEmptyOrWhiteSpace(code, nameof(code));
            Debug.True(code.IsAllUpperCase(), nameof(code));

            this.value = $"{code}.{venue}";

            this.Code = code;
            this.Venue = venue;
        }

        /// <summary>
        /// Gets the symbols code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the symbols venue.
        /// </summary>
        public Venue Venue { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Symbol"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Symbol left, Symbol right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Symbol"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Symbol left,  Symbol right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Symbol"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is Symbol symbol && this.Equals(symbol);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Symbol"/> is equal
        /// to the given <see cref="Symbol"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(Symbol other)
        {
            return this.value == other.value;
        }

        /// <summary>
        /// Returns the hash code of the <see cref="Symbol"/>.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return Hash.GetCode(this.value);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Symbol"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.value;
    }
}
