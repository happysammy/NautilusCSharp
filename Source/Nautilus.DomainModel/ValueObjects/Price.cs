//--------------------------------------------------------------------------------------------------
// <copyright file="Price.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Extensions;
using Nautilus.Core.Primitives;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents a non-negative financial market price with a specified decimal precision.
    /// </summary>
    [Immutable]
    public sealed class Price : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Price"/> class.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="precision">The price decimal precision.</param>
        private Price(decimal value, int precision)
            : base(value, precision)
        {
            Condition.NotNegativeDecimal(value, nameof(value));
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> parsed from the given string value.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(string value)
        {
            return Create(Convert.ToDecimal(value));
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> with the given value.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(decimal value)
        {
            return new Price(value, value.GetDecimalPlaces());
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> with the given value and decimal precision.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="precision">The price precision.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(decimal value, int precision)
        {
            return new Price(value, precision);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> with the given value and decimal precision.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="precision">The price precision.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(double value, int precision)
        {
            return new Price(decimal.Parse(value.ToString($"F{precision}")), precision);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the sum of this price value and the
        /// given price value.
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public Price Add(Price other)
        {
            Debug.True(this.Precision >= other.Precision, "this.Precision >= other.Precision");

            return new Price(this.Value + other.Value, this.Precision);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the given <see cref="Price"/>
        /// subtracted from this <see cref="Price"/> (cannot return a <see cref="Price"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public Price Subtract(Price other)
        {
            Debug.True(this.Precision >= other.Precision, "this.Precision >= other.Precision");

            return new Price(this.Value - other.Value, this.Precision);
        }
    }
}
