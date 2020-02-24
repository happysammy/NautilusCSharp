//--------------------------------------------------------------------------------------------------
// <copyright file="Price.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Primitives;

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
        private Price(decimal value, byte precision)
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
        /// Returns a new <see cref="Price"/> with the given value and decimal places.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="precision">The price precision.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Create(decimal value, byte precision)
        {
            return new Price(value, precision);
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
