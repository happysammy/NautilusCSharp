//-------------------------------------------------------------------------------------------------
// <copyright file="OptionRef{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents an optional reference type which wraps a potentially null value of type T.
    /// </summary>
    /// <typeparam name="T">The option object type.</typeparam>
    [Immutable]
    public struct OptionRef<T> : IEquatable<OptionRef<T>>
        where T : class
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionRef{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private OptionRef(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the <see cref="OptionRef{T}"/> (value cannot be null).
        /// </summary>
        public T Value => this.GetValue();

        /// <summary>
        /// Gets a value indicating whether the <see cref="OptionRef{T}"/> has a value.
        /// </summary>
        public bool HasValue => this.value != null;

        /// <summary>
        /// Gets a value indicating whether the <see cref="OptionRef{T}"/> has NO value.
        /// </summary>
        public bool HasNoValue => this.value is null;

        /// <summary>
        /// Returns a new <see cref="OptionRef{T}"/> with the given value T.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="OptionRef{T}"/>.</returns>
        public static implicit operator OptionRef<T>(T value) => new OptionRef<T>(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionRef{T}"/> is equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(OptionRef<T> option, T value) => option.value.Equals(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionRef{T}"/> is not equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(OptionRef<T> option, T value) => !(option == value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionRef{T}"/> is equal to the
        /// right <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(OptionRef<T> left, OptionRef<T> right) => left.Equals(right);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionRef{T}"/> is not equal to the
        /// right <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="OptionRef{T}"/> (cannot be null).</param>
        /// <returns>True if the <see cref="OptionRef{T}"/>(s) are not equal; otherwise returns false.</returns>
        public static bool operator !=(OptionRef<T> left, OptionRef<T> right) => !(left == right);

        /// <summary>
        /// Gets a <see cref="OptionRef{T}"/> with no value.
        /// </summary>
        /// <returns>A <see cref="OptionRef{T}"/>.</returns>
        public static OptionRef<T> None() => default;

        /// <summary>
        /// Gets the given object wrapped in an <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="obj">The object (cannot be null).</param>
        /// <returns>A <see cref="OptionRef{T}"/>.</returns>
        public static OptionRef<T> Some(T obj) => new OptionRef<T>(obj);

        /// <summary>
        /// Returns a result which indicates whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other <see cref="OptionRef{T}"/> is equal to this <see cref="OptionRef{T}"/>
        /// otherwise returns false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is T o)
            {
                obj = new OptionRef<T>(o);
            }

            return obj is OptionRef<T> option && this.Equals(option);
        }

        /// <summary>
        /// Returns a result indicating whether the specified <see cref="OptionRef{T}"/>
        /// is equal to the current <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="other">The other object (cannot be null).</param>
        /// <returns>True if the other <see cref="OptionRef{T}"/> is equal to this <see cref="OptionRef{T}"/>
        /// otherwise returns false.</returns>
        public bool Equals(OptionRef<T> other)
        {
            if (this.value is null && other.value is null)
            {
                return true;
            }

            if (this.value is null || other.value is null)
            {
                return false;
            }

            return this.value.Equals(other.Value);
        }

        /// <summary>
        /// Returns the hash code of the wrapped object.
        /// </summary>
        /// <returns>An <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.value);

        /// <summary>
        /// Returns a string representation of the wrapped value.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return this.value != null
                ? this.value.ToString()
                : "NONE";
        }

        private T GetValue()
        {
            if (this.value is null)
            {
                throw new InvalidOperationException("Cannot get value (the value is null).");
            }

            return this.value;
        }
    }
}
