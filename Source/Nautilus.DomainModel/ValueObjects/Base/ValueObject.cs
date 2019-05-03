//--------------------------------------------------------------------------------------------------
// <copyright file="ValueObject.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The base class for all domain model value objects.
    /// </summary>
    /// <typeparam name="T">The value object type.</typeparam>
    [Immutable]
    public abstract class ValueObject<T>
        where T : ValueObject<T>, IEquatable<T>
    {
        /// <summary>
        /// Returns a value indicating whether the <see cref="ValueObject{T}"/>(s) are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
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
        public static bool operator !=(ValueObject<T> left,  ValueObject<T> right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValueObject{T}"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object other) => other is T && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="ValueObject{T}"/> is equal
        /// to the given <see cref="ValueObject{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        [PerformanceOptimized]
        public bool Equals(T other)
        {
            if (other is null)
            {
                return false;
            }

            var thisEqualityArray = this.GetEqualityArray();
            var otherEqualityArray = other.GetEqualityArray();
            for (var i = 0; i < thisEqualityArray.Length; i++)
            {
                if (!thisEqualityArray[i].Equals(otherEqualityArray[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the hash code for this object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        [PerformanceOptimized]
        public override int GetHashCode()
        {
            var hash = 0;
            var equalityArray = this.GetEqualityArray();
            for (var i = 0; i < equalityArray.Length; i++)
            {
                hash += Hash.GetCode(equalityArray[i]);
            }

            return hash;
        }

        /// <summary>
        /// Returns an array of objects to be included in equality checks.
        /// </summary>
        /// <returns>The array of equality members.</returns>
        protected abstract object[] GetEqualityArray();
    }
}
