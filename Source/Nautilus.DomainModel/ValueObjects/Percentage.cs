//--------------------------------------------------------------------------------------------------
// <copyright file="Percentage.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Primitives;
    using Nautilus.Core;

    /// <summary>
    /// Represents a percentage value.
    /// </summary>
    [Immutable]
    public sealed class Percentage : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Percentage"/> class.
        /// </summary>
        /// <param name="percent">The percent.</param>
        private Percentage(decimal percent)
            : base(percent)
        {
            Debug.NotNegativeDecimal(percent, nameof(percent));
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> with a value of zero.
        /// </summary>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public static Percentage Zero()
        {
            return new Percentage(decimal.Zero);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> with the given percent value.
        /// </summary>
        /// <param name="percent">The percent value.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public static Percentage Create(decimal percent)
        {
            return new Percentage(percent);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the sum of this percent value
        /// and the given percent value.
        /// </summary>
        /// <param name="other">The other percentage.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public Percentage Add(Percentage other)
        {
            Debug.NotNull(other, nameof(other));

            return new Percentage(this.Value + other.Value);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the subtraction of the given
        /// percent value from this percent value.
        /// </summary>
        /// <param name="other">The other percentage.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public Percentage Subtract(Percentage other)
        {
            Debug.NotNull(other, nameof(other));
            Debug.True(other.Value <= this.Value, nameof(other));

            return new Percentage(this.Value - other.Value);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the value of this percent
        /// multiplier by the given multiplier.
        /// </summary>
        /// <param name="multiplier">The other percentage.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public Percentage MultiplyBy(decimal multiplier)
        {
            Debug.NotNegativeDecimal(multiplier, nameof(multiplier));

            return new Percentage(this.Value * multiplier);
        }

        /// <summary>
        /// Returns a value which is the percent of the value of this percentage.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public decimal PercentOf(decimal value)
        {
            Debug.NotNegativeDecimal(value, nameof(value));

            return (value * this.Value) / 100;
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the value of this percent
        /// divided by the given divisor (cannot be zero).
        /// </summary>
        /// <param name="divisor">The other price.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public Percentage DivideBy(decimal divisor)
        {
            Debug.PositiveDecimal(divisor, nameof(divisor));

            return new Percentage(this.Value / divisor);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Percentage"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Value}%";
        }
    }
}
