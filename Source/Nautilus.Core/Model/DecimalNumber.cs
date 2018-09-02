//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalNumber.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Model
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The base class for all <see cref="ValueObject{T}"/>(s) based on a decimal number.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ValueObject{T}"/>.</typeparam>
    [Immutable]
    public abstract class DecimalNumber<T> : ValueObject<T>
        where T : DecimalNumber<T>, IEquatable<T>, IComparable<DecimalNumber<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalNumber{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected DecimalNumber(decimal value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the decimal number.
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static decimal operator +(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static decimal operator +(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static decimal operator +(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value + right;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the either argument is null.</exception>
        public static decimal operator -(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static decimal operator -(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static decimal operator -(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value - right;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static decimal operator *(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static decimal operator *(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static decimal operator *(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value * right;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null, or if the right
        /// value is zero or negative.</exception>
        public static decimal operator /(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));
            Debug.PositiveDecimal(right.Value, nameof(right));

            return left.Value / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null, or if the right value
        /// is zero or negative.</exception>
        public static decimal operator /(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));
            Debug.PositiveDecimal(right.Value, nameof(right));

            return left / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null, or if the right is
        /// negative.</exception>
        public static decimal operator /(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.PositiveDecimal(right, nameof(right));

            return left.Value / right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator >(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));
            Debug.NotEqualTo(right.Value, nameof(right.Value), decimal.Zero);

            return left.Value > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator >(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator >(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value > right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator >=(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator >=(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator >=(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value >= right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the either argument is null.</exception>
        public static bool operator <(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator <(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator <(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value < right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator <=(DecimalNumber<T> left, DecimalNumber<T> right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator <=(decimal left, DecimalNumber<T> right)
        {
            Debug.NotNull(right, nameof(right));

            return left <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator <=(DecimalNumber<T> left, decimal right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value <= right;
        }

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="IntegerNumber{T}"/>(s) being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the other is null.</exception>
        public int CompareTo(DecimalNumber<T> other)
        {
            Debug.NotNull(other, nameof(other));

            return this.Value.CompareTo(other.Value);
        }
    }
}
