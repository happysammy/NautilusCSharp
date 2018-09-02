//--------------------------------------------------------------------------------------------------
// <copyright file="EntityId{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers.Base
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities.Base;

    /// <summary>
    /// Represents a validated and unique entity identifier.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    [Immutable]
    public class EntityId<T>
        where T : Entity<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityId{T}"/> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        protected EntityId(string value)
        {
            Debug.NotNull(value, nameof(value));
            Debug.True(value.Length <= 100, nameof(value));

            this.Value = value.RemoveAllWhitespace();
        }

        /// <summary>
        /// Gets the value of the entity identifier.
        /// </summary>
        public string Value { get; }

                /// <summary>
        /// Returns a value indicating whether the <see cref="EntityId{T}"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(
            [CanBeNull] EntityId<T> left,
            [CanBeNull] EntityId<T> right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="EntityId{T}"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(
            [CanBeNull] EntityId<T> left,
            [CanBeNull] EntityId<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="EntityId{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as EntityId<T>);

        /// <summary>
        /// Returns a value indicating whether this <see cref="EntityId{T}"/> is equal
        /// to the given <see cref="EntityId{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals([CanBeNull] EntityId<T> other) => other != null && this.Value == other.Value;

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="ValidString"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
