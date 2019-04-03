//--------------------------------------------------------------------------------------------------
// <copyright file="FloatingPointNumber.cs" company="Nautech Systems Pty Ltd">
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
    /// The base class for all primitive numbers based on a double-precision floating-point number.
    /// </summary>
    [Immutable]
    public abstract class FloatingPointNumber
        : IEquatable<FloatingPointNumber>, IComparable<FloatingPointNumber>,
          IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingPointNumber" /> class.
        /// </summary>
        /// <param name="value">The floating-point value.</param>
        protected FloatingPointNumber(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of the double number.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator +(FloatingPointNumber left, FloatingPointNumber right)
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
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator +(double left, FloatingPointNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator +(FloatingPointNumber left, double right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value + right;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator -(FloatingPointNumber left, FloatingPointNumber right)
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
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator -(double left, FloatingPointNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator -(FloatingPointNumber left, double right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value - right;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator *(FloatingPointNumber left, FloatingPointNumber right)
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
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator *(double left, FloatingPointNumber right)
        {
            Debug.NotNull(right, nameof(right));

            return left * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator *(FloatingPointNumber left, double right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value * right;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator /(FloatingPointNumber left, FloatingPointNumber right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.NotNull(right, nameof(right));
            Debug.PositiveDouble(right.Value, nameof(right));

            return left.Value / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator /(double left, FloatingPointNumber right)
        {
            Debug.NotNull(right, nameof(right));
            Debug.PositiveDouble(right.Value, nameof(right));

            return left / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="double"/>.</returns>
        public static double operator /(FloatingPointNumber left, double right)
        {
            Debug.NotNull(left, nameof(left));
            Debug.PositiveDouble(right, nameof(right));

            return left.Value / right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(FloatingPointNumber left, FloatingPointNumber right)
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
        public static bool operator >(double left, FloatingPointNumber right)
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
        public static bool operator >(FloatingPointNumber left, double right)
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
        public static bool operator >=(FloatingPointNumber left, FloatingPointNumber right)
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
        public static bool operator >=(double left, FloatingPointNumber right)
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
        public static bool operator >=(FloatingPointNumber left, double right)
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
        public static bool operator <(FloatingPointNumber left, FloatingPointNumber right)
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
        public static bool operator <(double left, FloatingPointNumber right)
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
        public static bool operator <(FloatingPointNumber left, double right)
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
        public static bool operator <=(FloatingPointNumber left, FloatingPointNumber right)
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
        public static bool operator <=(double left, FloatingPointNumber right)
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
        public static bool operator <=(FloatingPointNumber left, double right)
        {
            Debug.NotNull(left, nameof(left));

            return left.Value <= right;
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="FloatingPointNumber"/>s are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(
            [CanBeNull] FloatingPointNumber left,
            [CanBeNull] FloatingPointNumber right)
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
        /// Returns a value indicating whether the <see cref="FloatingPointNumber"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(
            [CanBeNull] FloatingPointNumber left,
            [CanBeNull] FloatingPointNumber right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this <see cref="FloatingPointNumber"/> is equal
        /// to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals([CanBeNull] object other) => this.Equals(other as FloatingPointNumber);

        /// <summary>
        /// Returns a value indicating whether this <see cref="FloatingPointNumber"/> is equal to the
        /// given <see cref="FloatingPointNumber"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals([CanBeNull] FloatingPointNumber other)
        {
            return other != null && this.Value.Equals(other.Value);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="FloatingPointNumber"/> is equal
        /// to the given <see cref="double"/>.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(double other) => this.Value.Equals(other);

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="FloatingPointNumber"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(FloatingPointNumber other)
        {
            Debug.NotNull(other, nameof(other));

            return this.Value.CompareTo(other.Value);
        }

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="double"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(double other) => this.Value.CompareTo(other);

        /// <summary>
        /// Returns the hash code for this <see cref="FloatingPointNumber"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="FloatingPointNumber"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value.ToString(CultureInfo.InvariantCulture);
    }
}
