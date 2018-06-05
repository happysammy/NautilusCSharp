//--------------------------------------------------------------------------------------------------
// <copyright file="IntegerNumber.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The base class for all <see cref="ValueObject{T}"/>(s) based on an integer number.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ValueObject{T}"/>.</typeparam>
    [Immutable]
    public abstract class IntegerNumber<T> : ValueObject<T> where T : IntegerNumber<T>,
        IComparable<IntegerNumber<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerNumber{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected IntegerNumber(int value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the integer number.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static int operator +(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static int operator +(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static int operator +(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value + right;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static int operator -(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static int operator -(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static int operator -(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value - right;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static int operator *(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static int operator *(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static int operator *(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value * right;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static int operator /(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));
            Validate.Int32NotOutOfRange(right.Value, nameof(right), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return left.Value / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static int operator /(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));
            Validate.Int32NotOutOfRange(right.Value, nameof(right), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return left / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static int operator /(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.Int32NotOutOfRange(right, nameof(right), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return left.Value / right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator >(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));
            Validate.Int32NotOutOfRange(right.Value, nameof(right), 0, int.MaxValue, RangeEndPoints.Exclusive);

            return left.Value > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator >(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator >(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value > right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator >=(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator >=(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator >=(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value >= right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator <(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator <(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator <(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value < right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public static bool operator <=(IntegerNumber<T> left, IntegerNumber<T> right)
        {
            Validate.NotNull(left, nameof(left));
            Validate.NotNull(right, nameof(right));

            return left.Value <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the right is null.</exception>
        public static bool operator <=(int left, IntegerNumber<T> right)
        {
            Validate.NotNull(right, nameof(right));

            return left <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        /// <exception cref="ValidationException">Throws if the left is null.</exception>
        public static bool operator <=(IntegerNumber<T> left, int right)
        {
            Validate.NotNull(left, nameof(left));

            return left.Value <= right;
        }

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="IntegerNumber{T}"/>(s) being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        /// <exception cref="ValidationException">Throws if the other is null.</exception>
        public int CompareTo(IntegerNumber<T> other)
        {
            Validate.NotNull(other, nameof(other));

            return this.Value.CompareTo(other.Value);
        }
    }
}
