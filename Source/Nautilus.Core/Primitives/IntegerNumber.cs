//--------------------------------------------------------------------------------------------------
// <copyright file="IntegerNumber.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Primitives
{
    using System;
    using System.Globalization;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The base class for all primitive numbers based on an integer.
    /// </summary>
    [Immutable]
    public abstract class IntegerNumber
        : IEquatable<IntegerNumber>, IComparable<IntegerNumber>,
          IEquatable<int>, IComparable<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerNumber" /> class.
        /// </summary>
        /// <param name="value">The integer value.</param>
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
        public static int operator +(IntegerNumber left, IntegerNumber right)
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
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator +(int left, IntegerNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator +(IntegerNumber left, int right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value + right;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator -(IntegerNumber left, IntegerNumber right)
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
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator -(int left, IntegerNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator -(IntegerNumber left, int right)
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
        public static int operator *(IntegerNumber left, IntegerNumber right)
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
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator *(int left, IntegerNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator *(IntegerNumber left, int right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value * right;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator /(IntegerNumber left, IntegerNumber right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));
            Debug.PositiveInt32(right.Value, nameof(right));

            return left.Value / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator /(int left, IntegerNumber right)
        {
            Debug.NotNull(right, nameof(right));
            Debug.PositiveInt32(right.Value, nameof(right));

            return left / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public static int operator /(IntegerNumber left, int right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.PositiveInt32(right, nameof(right));

            return left.Value / right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(IntegerNumber left, IntegerNumber right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));

            return left.Value > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(int left, IntegerNumber right)
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
        public static bool operator >(IntegerNumber left, int right)
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
        public static bool operator >=(IntegerNumber left, IntegerNumber right)
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
        public static bool operator >=(int left, IntegerNumber right)
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
        public static bool operator >=(IntegerNumber left, int right)
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
        public static bool operator <(IntegerNumber left, IntegerNumber right)
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
        public static bool operator <(int left, IntegerNumber right)
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
        public static bool operator <(IntegerNumber left, int right)
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
        public static bool operator <=(IntegerNumber left, IntegerNumber right)
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
        public static bool operator <=(int left, IntegerNumber right)
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
        public static bool operator <=(IntegerNumber left, int right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value <= right;
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="DecimalNumber"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(
            [CanBeNull] IntegerNumber left,
            [CanBeNull] IntegerNumber right)
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
        /// Returns a value indicating whether the <see cref="IntegerNumber"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(
            [CanBeNull] IntegerNumber left,
            [CanBeNull] IntegerNumber right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="IntegerNumber"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as IntegerNumber);

        /// <summary>
        /// Returns a value indicating whether this <see cref="IntegerNumber"/> is equal to the
        /// given <see cref="int"/>.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals([CanBeNull] IntegerNumber other)
        {
            return other != null && this.Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="IntegerNumber"/> is equal to the
        /// given <see cref="int"/>.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(int other) => this.Value.Equals(other);

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="IntegerNumber"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(IntegerNumber other)
        {
            Debug.NotNull(other, nameof(other));

            return this.Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="int"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(int other) => this.Value.CompareTo(other);

        /// <summary>
        /// Returns the hash code for this <see cref="IntegerNumber"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value);

        /// <summary>
        /// Returns a string representation of the <see cref="IntegerNumber"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value.ToString(CultureInfo.InvariantCulture);
    }
}
