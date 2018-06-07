//--------------------------------------------------------------------------------------------------
// <copyright file="Percentage.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The immutable sealed <see cref="Percentage"/> class. Represents a percentage value.
    /// </summary>
    [Immutable]
    public sealed class Percentage
        : DecimalNumber<Percentage>, IEquatable<Percentage>, IComparable<DecimalNumber<Percentage>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Percentage"/> class.
        /// </summary>
        /// <param name="percent">The percent.</param>
        private Percentage(decimal percent)
            : base(percent)
        {
            Validate.DecimalNotOutOfRange(percent, nameof(percent), decimal.Zero, decimal.MaxValue);
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
        /// <exception cref="ValidationException">Throws if percent is negative.</exception>
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
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public Percentage Add(Percentage other)
        {
            Validate.NotNull(other, nameof(other));

            return new Percentage(this.Value + other.Value);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the subtraction of the given
        /// percent value from this percent value.
        /// </summary>
        /// <param name="other">The other percentage.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null, or if the other
        /// value is greater than this value.</exception>
        public Percentage Subtract(Percentage other)
        {
            Validate.NotNull(other, nameof(other));
            Validate.True(other.Value <= this.Value, nameof(other));

            return new Percentage(this.Value - other.Value);
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the value of this percent
        /// multiplier by the given multiplier.
        /// </summary>
        /// <param name="multiplier">The other percentage.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        /// <exception cref="ValidationException">Throws if the multiplier is negative.</exception>
        public Percentage MultiplyBy(decimal multiplier)
        {
            Validate.DecimalNotOutOfRange(multiplier, nameof(multiplier), decimal.Zero, decimal.MaxValue);

            return new Percentage(this.Value * multiplier);
        }

        /// <summary>
        /// Returns a value which is the percent of the value of this percentage.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the value is negative.</exception>
        public decimal PercentOf(decimal value)
        {
            Validate.DecimalNotOutOfRange(value, nameof(value), decimal.Zero, decimal.MaxValue);

            return (value * this.Value) / 100;
        }

        /// <summary>
        /// Returns a new <see cref="Percentage"/> as the result of the value of this percent
        /// divided by the given divisor (cannot be zero).
        /// </summary>
        /// <param name="divisor">The other price.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        /// <exception cref="ValidationException">Throws if the divisor is zero or negative.</exception>
        public Percentage DivideBy(decimal divisor)
        {
            Validate.DecimalNotOutOfRange(divisor, nameof(divisor), decimal.Zero, decimal.MaxValue);

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

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects for the equality check.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[] { this.Value };
        }
    }
}
