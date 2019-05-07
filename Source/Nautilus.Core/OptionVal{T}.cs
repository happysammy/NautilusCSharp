//-------------------------------------------------------------------------------------------------
// <copyright file="OptionVal{T}.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an optional value type which wraps a potentially null value of <see cref="Nullable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The option object type.</typeparam>
    [Immutable]
    public struct OptionVal<T> : IEquatable<object>
        where T : struct
    {
        private readonly T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionVal{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private OptionVal(T value)
        {
            if (Nullable.GetUnderlyingType(typeof(T)) == default)
            {
                this.value = null;
            }

            this.value = value;
        }

        /// <summary>
        /// Gets the value of the <see cref="OptionVal{T}"/> (value cannot be null).
        /// </summary>
        public T Value => this.GetValue();

        /// <summary>
        /// Gets a value indicating whether the <see cref="OptionVal{T}"/> has a value.
        /// </summary>
        public bool HasValue => this.value != null;

        /// <summary>
        /// Gets a value indicating whether the <see cref="OptionVal{T}"/> has NO value.
        /// </summary>
        public bool HasNoValue => this.value is null;

        /// <summary>
        /// Returns a new <see cref="OptionVal{T}"/> with the given value T.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="OptionVal{T}"/>.</returns>
        public static implicit operator OptionVal<T>(T value) => new OptionVal<T>(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionVal{T}"/> is equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(OptionVal<T> option, T value) => option.value.Equals(value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionVal{T}"/> is not equal to the
        /// right T.
        /// </summary>
        /// <param name="option">The <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator !=(OptionVal<T> option, T value) => !(option == value);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionVal{T}"/> is equal to the
        /// right <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator ==(OptionVal<T> left, OptionVal<T> right) => left.Equals(right);

        /// <summary>
        /// Returns a result indicating whether the left <see cref="OptionVal{T}"/> is not equal to the
        /// right <see cref="OptionRef{T}"/>.
        /// </summary>
        /// <param name="left">The left <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <param name="right">The right <see cref="OptionVal{T}"/> (cannot be null).</param>
        /// <returns>True if the <see cref="OptionVal{T}"/>(s) are not equal; otherwise returns false.</returns>
        public static bool operator !=(OptionVal<T> left, OptionVal<T> right) => !(left == right);

        /// <summary>
        /// Gets a <see cref="OptionVal{T}"/> with no value.
        /// </summary>
        /// <returns>A <see cref="OptionVal{T}"/>.</returns>
        public static OptionVal<T> None() => default;

        /// <summary>
        /// Gets the given object wrapped in an <see cref="OptionVal{T}"/>.
        /// </summary>
        /// <param name="value">The value (cannot be null).</param>
        /// <returns>A <see cref="OptionVal{T}"/>.</returns>
        public static OptionVal<T> Some(T value) => new OptionVal<T>(value);

        /// <summary>
        /// Returns a result which indicates whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other <see cref="OptionVal{T}"/> is equal to this <see cref="OptionVal{T}"/>
        /// otherwise returns false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is T o)
            {
                obj = new OptionVal<T>(o);
            }

            return obj is OptionVal<T> option && this.Equals(option);
        }

        /// <summary>
        /// Returns a result indicating whether the specified <see cref="OptionVal{T}"/>
        /// is equal to the current <see cref="OptionVal{T}"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>True if the other <see cref="OptionVal{T}"/> is equal to this <see cref="OptionVal{T}"/>
        /// otherwise returns false.</returns>
        public bool Equals(OptionVal<T> other)
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
                throw new InvalidOperationException("Cannot get a null value.");
            }

            return (T)this.value;
        }
    }
}
