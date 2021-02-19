﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Decimal64.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;

namespace Nautilus.Core.Primitives
{
    /// <summary>
    /// The base class for all primitive numbers based on a decimal.
    /// </summary>
    [Immutable]
    public abstract class Decimal64
        : IEquatable<object>, IEquatable<decimal>, IEquatable<Decimal64>,
            IComparable<decimal>, IComparable<Decimal64>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Decimal64" /> class.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        /// <param name="precision">The decimal precision.</param>
        protected Decimal64(decimal value, int precision)
        {
            this.Value = Math.Round(value, precision);
            this.Precision = precision;
        }

        /// <summary>
        /// Gets the decimal precision.
        /// </summary>
        public int Precision { get; }

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
        public static decimal operator +(Decimal64 left, Decimal64 right)
        {
            return left.Value + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator +(decimal left, Decimal64 right)
        {
            return left + right.Value;
        }

        /// <summary>
        /// Returns the sum of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator +(Decimal64 left, decimal right)
        {
            return left.Value + right;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator -(Decimal64 left, Decimal64 right)
        {
            return left.Value - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator -(decimal left, Decimal64 right)
        {
            return left - right.Value;
        }

        /// <summary>
        /// Returns the result of subtracting the right number from the left number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator -(Decimal64 left, decimal right)
        {
            return left.Value - right;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator *(Decimal64 left, Decimal64 right)
        {
            return left.Value * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator *(decimal left, Decimal64 right)
        {
            return left * right.Value;
        }

        /// <summary>
        /// Returns the product of the left number and the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator *(Decimal64 left, decimal right)
        {
            return left.Value * right;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator /(Decimal64 left, Decimal64 right)
        {
            Debug.PositiveDecimal(right.Value, nameof(right.Value));

            return left.Value / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator /(decimal left, Decimal64 right)
        {
            Debug.PositiveDecimal(right.Value, nameof(right.Value));

            return left / right.Value;
        }

        /// <summary>
        /// Returns the result of dividing the left number by the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal operator /(Decimal64 left, decimal right)
        {
            Debug.PositiveDecimal(right, nameof(right));

            return left.Value / right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(Decimal64 left, Decimal64 right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(decimal left, Decimal64 right)
        {
            return left > right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >(Decimal64 left, decimal right)
        {
            return left.Value > right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >=(Decimal64 left, Decimal64 right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >=(decimal left, Decimal64 right)
        {
            return left >= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is greater than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator >=(Decimal64 left, decimal right)
        {
            return left.Value >= right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <(Decimal64 left, Decimal64 right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <(decimal left, Decimal64 right)
        {
            return left < right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <(Decimal64 left, decimal right)
        {
            return left.Value < right;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <=(Decimal64 left, Decimal64 right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <=(decimal left, Decimal64 right)
        {
            return left <= right.Value;
        }

        /// <summary>
        /// Returns a value indicating whether the left number is less than or equal to the right number.
        /// </summary>
        /// <param name="left">The left number.</param>
        /// <param name="right">The right number.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool operator <=(Decimal64 left, decimal right)
        {
            return left.Value <= right;
        }

        /// <summary>
        /// Returns a value indicating whether the numbers are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(Decimal64 left, Decimal64 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the numbers are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(decimal left, Decimal64 right)
        {
            return left.Equals(right.Value);
        }

        /// <summary>
        /// Returns a value indicating whether the numbers are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(Decimal64 left, decimal right)
        {
            return left.Value.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Decimal64"/>s are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(Decimal64 left, Decimal64 right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether the numbers are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(decimal left, Decimal64 right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether the numbers are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(Decimal64 left, decimal right) => !(left == right);

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj is Decimal64 number && this.Equals(number);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value indicating whether this <see cref="Decimal64"/> is equal to the
        /// given <see cref="Decimal64"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(Decimal64 other) => this.Value.Equals(other.Value);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Decimal64"/> is equal to the
        /// given <see cref="decimal"/>.
        /// </summary>
        /// <param name="other">The other decimal.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(decimal other) => this.Value.Equals(other);

        // Due to the convention that an IEquatable<T> argument can be null the compiler now emits
        // a warning unless Equals is marked with [AllowNull] or takes a nullable param. We don't
        // want to allow null here for the sake of silencing the warning and so temporarily using
        // #pragma warning disable CS8767 until a better refactoring is determined.
#pragma warning disable CS8767
        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="Decimal64"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(Decimal64 other) => this.Value.CompareTo(other.Value);

        /// <summary>
        /// Returns a value which indicates the relative order of the <see cref="decimal"/>s
        /// being compared.
        /// </summary>
        /// <param name="other">The other number.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public int CompareTo(decimal other) => this.Value.CompareTo(other);

        /// <summary>
        /// Returns the hash code for this <see cref="Decimal64"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Decimal64"></see>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value.ToString($"F{this.Precision}", CultureInfo.InvariantCulture);

        /// <summary>
        /// Returns the formatted string representation of this object.
        /// </summary>
        /// <returns>The formatted string.</returns>
        protected string ToStringFormatted() => this.Value.ToString($"N{this.Precision}", CultureInfo.InvariantCulture);
    }
}
