//-------------------------------------------------------------------------------------------------
// <copyright file="Option{T}.cs" company="Nautech Systems Pty Ltd">
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
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private Option(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the <see cref="Option{T}"/> (value cannot be null).
        /// </summary>
        public T Value => this.GetValue();

        /// <summary>
        /// Gets a value indicating whether the <see cref="Option{T}"/> has a value.
        /// </summary>
        public bool HasValue => this.value != null;

        /// <summary>
        /// Gets a value indicating whether the <see cref="Option{T}"/> has NO value.
        /// </summary>
        public bool HasNoValue => !this.HasValue;

        /// <summary>
        /// Returns a new <see cref="Option{T}"/> with the given value T.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Option{T}"/>.</returns>
        public static implicit operator Option<T>(T value) => new Option<T>(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Option{T}"/> is equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="Option{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Option<T> option, T value) => option.value != null && option.value.Equals(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Option{T}"/> is not equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="Option{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(Option<T> option, T value) => !(option == value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Option{T}"/> is equal to the
        /// right <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="Option{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="Option{T}"/> (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="Option{T}"/> is not equal to the
        /// right <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="Option{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="Option{T}"/> (cannot be null).</param>
        /// <returns>True if the <see cref="Option{T}"/>(s) are not equal; otherwise returns false.</returns>
        public static bool operator !=(Option<T> left, Option<T> right) => !(left == right);

        /// <summary>
        /// Gets a <see cref="Option{T}"/> with no value.
        /// </summary>
        /// <returns>A <see cref="Option{T}"/>.</returns>
        public static Option<T> None() => default;

        /// <summary>
        /// Gets the given object wrapped in an <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="obj">The object (cannot be null).</param>
        /// <returns>A <see cref="Option{T}"/>.</returns>
        public static Option<T> Some(T obj) => new Option<T>(obj);

        /// <summary>
        /// Returns a result which indicates whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other <see cref="Option{T}"/> is equal to this <see cref="Option{T}"/>
        /// otherwise returns false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is T o)
            {
                obj = new Option<T>(o);
            }

            return obj is Option<T> option && this.Equals(option);
        }

        /// <summary>
        /// Returns a result indicating whether the specified <see cref="Option{T}"/>
        /// is equal to the current <see cref="Option{T}"/>.
        /// </summary>
        /// <param name="other">The other object (cannot be null).</param>
        /// <returns>True if the other <see cref="Option{T}"/> is equal to this <see cref="Option{T}"/>
        /// otherwise returns false.</returns>
        public bool Equals(Option<T> other)
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
