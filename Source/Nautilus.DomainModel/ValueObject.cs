// -------------------------------------------------------------------------------------------------
// <copyright file="ValueObject.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Annotations;

    /// <summary>
    /// The immutable abstract <see cref="ValueObject{T}"/> class. The base class for all domain
    /// value objects.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    [Immutable]
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        /// <summary>
        /// Returns a value indicating whether the <see cref="ValueObject{T}"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(
            [CanBeNull] ValueObject<T> left,
            [CanBeNull] ValueObject<T> right)
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
        /// Returns a value indicating whether the <see cref="ValueObject{T}"/>(s) are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(
            [CanBeNull] ValueObject<T> left,
            [CanBeNull] ValueObject<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValueObject{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as T);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValueObject{T}"/> is equal
        /// to the given <see cref="ValueObject{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool Equals([CanBeNull] T other)
        {
            return other != null && this.GetMembersForEqualityCheck()
               .SequenceEqual(other.GetMembersForEqualityCheck());
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return this.GetMembersForEqualityCheck()
               .Aggregate(17, (current, obj) => (current * 31) + (obj == null ? 0 : obj.GetHashCode()));
        }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/>.</returns>
        protected abstract IEnumerable<object> GetMembersForEqualityCheck();
    }
}
