//--------------------------------------------------------------------------------------------------
// <copyright file="EntityId{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers.Base
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
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
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
            Debug.NotOutOfRangeInt32(value.Length, 1,  1024, nameof(value));

            this.Value = value;
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
        public static bool operator ==(EntityId<T> left, EntityId<T> right)
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
        public static bool operator !=(EntityId<T> left, EntityId<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="EntityId{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other != null && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="EntityId{T}"/> is equal
        /// to the given <see cref="EntityId{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals(EntityId<T> other) => this.Value == other.Value;

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="EntityId{T}"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
