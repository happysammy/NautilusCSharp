//--------------------------------------------------------------------------------------------------
// <copyright file="Quantity.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.Core.Primitives;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a non-negative quantity with a specified decimal precision.
    /// </summary>
    [Immutable]
    public sealed class Quantity : DecimalNumber
    {
        private static readonly Quantity QuantityOfZero = new Quantity(decimal.Zero, 0);
        private static readonly Quantity QuantityOfOne = new Quantity(decimal.One, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Quantity"/> class.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <param name="precision">The precision of the quantity.</param>
        private Quantity(decimal value, int precision)
            : base(value, precision)
        {
            Condition.NotNegativeDecimal(value, nameof(value));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with zero value.
        /// </summary>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Zero() => QuantityOfZero;

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with a value of one.
        /// </summary>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity One() => QuantityOfOne;

        /// <summary>
        /// Returns a new <see cref="Quantity"/> parsed from the given string value.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Create(string value)
        {
            return Create(Parser.ToDecimal(value));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with the given amount.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Create(decimal value)
        {
            return new Quantity(value, value.GetDecimalPlaces());
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with the given amount.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        /// <param name="precision">The precision of the quantity.</param>
        public static Quantity Create(decimal value, int precision)
        {
            return new Quantity(value, precision);
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with the given value and decimal precision.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <param name="precision">The quantity precision.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Create(double value, int precision)
        {
            return new Quantity(Parser.ToDecimal(value.ToString($"F{precision}")), precision);
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> as the result of the sum of this <see cref="Quantity"/>
        /// and the given <see cref="Quantity"/>.
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public Quantity Add(Quantity other)
        {
            return new Quantity(this.Value + other.Value, Math.Max(this.Precision, other.Precision));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> as the result of the given <see cref="Quantity"/>
        /// subtracted from this <see cref="Quantity"/> (cannot return a <see cref="Quantity"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public Quantity Sub(Quantity other)
        {
            Debug.True(other.Value <= this.Value, nameof(other));

            return new Quantity(this.Value - other.Value, Math.Max(this.Precision, other.Precision));
        }

        /// <summary>
        /// Returns a formatted string representation of this <see cref="Quantity"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public new string ToStringFormatted()
        {
            if (this.Precision > 0)
            {
                return this.ToString();
            }

            if (this.Value < 1000m || this.Value % 1000 != 0)
            {
                return this.ToString();
            }

            if (this.Value < 1000000m)
            {
                return (this.Value / 1000m).ToString("F0") + "K";
            }

            return (this.Value / 1000000m).ToString("F3").TrimEnd('0').TrimEnd('.') + "M";
        }
    }
}
