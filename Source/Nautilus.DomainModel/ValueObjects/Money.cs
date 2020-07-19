//--------------------------------------------------------------------------------------------------
// <copyright file="Money.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Primitives;
using Nautilus.DomainModel.Enums;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents the 'concept' of money.
    /// </summary>
    [Immutable]
    public sealed class Money : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        private Money(decimal amount, Currency currency)
            : base(amount, 2)
        {
            Debug.NotDefault(currency, nameof(currency));

            this.Currency = currency;
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Money"/> objects are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(Money left, Money right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Money"/> objects are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(Money left, Money right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Money"/> object with a value of zero.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public static Money Zero(Currency currency)
        {
            return new Money(decimal.Zero, currency);
        }

        /// <summary>
        /// Returns a new <see cref="Money"/> object with the given amount, of the given currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public static Money Create(decimal amount, Currency currency)
        {
            return new Money(amount, currency);
        }

        /// <summary>
        /// Adds the given money to this money.
        /// </summary>
        /// <param name="other">The amount.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money Add(Money other)
        {
            return new Money(this.Value + other.Value, this.Currency);
        }

        /// <summary>
        /// Subtracts the given money from this money.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money Subtract(Money other)
        {
            return new Money(this.Value - other.Value, this.Currency);
        }

        /// <summary>
        /// Multiplies this money by the given multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money MultiplyBy(int multiplier)
        {
            return new Money(this.Value * multiplier, this.Currency);
        }

        /// <summary>
        /// Divides this money by the given divisor.
        /// </summary>
        /// <param name="divisor">The divisor.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money DivideBy(int divisor)
        {
            return new Money(this.Value / divisor, this.Currency);
        }

        /// <summary>
        /// Returns a value indicating whether this object is equal to the given object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj) => obj != null && this.Equals(obj);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Money"/> is equal to the given <see cref="Money"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(Money other) => this.Value.Equals(other.Value) && this.Currency.Equals(other.Currency);

        /// <summary>
        /// Returns the hash code for this <see cref="DecimalNumber"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value, this.Currency);

        /// <summary>
        /// Returns a formatted string representation of the <see cref="Money"/> object including currency.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public new string ToStringFormatted()
        {
            return $"{base.ToStringFormatted()} {this.Currency}";
        }
    }
}
