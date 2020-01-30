//--------------------------------------------------------------------------------------------------
// <copyright file="Quantity.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Primitives;

    /// <summary>
    /// Represents a non-negative decimal quantity.
    /// </summary>
    [Immutable]
    [SuppressMessage("ReSharper", "InterpolatedStringExpressionIsNotIFormattable", Justification = "Formatting.")]
    public sealed class Quantity : DecimalNumber
    {
        private static readonly Quantity ZeroQuantity = new Quantity(decimal.Zero, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Quantity"/> class.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <param name="precision">The precision of the quantity.</param>
        private Quantity(decimal value, int precision)
            : base(value, precision)
        {
            Debug.NotNegativeDecimal(value, nameof(value));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with zero value.
        /// </summary>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Zero() => ZeroQuantity;

        /// <summary>
        /// Returns a new <see cref="Quantity"/> with the given amount.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        /// <param name="precision">The precision of the quantity.</param>
        public static Quantity Create(decimal value, int precision = 0)
        {
            return new Quantity(value, precision);
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
        public Quantity Subtract(Quantity other)
        {
            Debug.True(other.Value <= this.Value, nameof(other));

            return new Quantity(this.Value - other.Value, Math.Max(this.Precision, other.Precision));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> as the result of the given <see cref="Quantity"/>
        /// divided from this <see cref="Quantity"/> (cannot return a <see cref="Quantity"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public Quantity Divide(Quantity other)
        {
            return new Quantity(this.Value / other.Value, Math.Max(this.Precision, other.Precision));
        }

        /// <summary>
        /// Returns a new <see cref="Quantity"/> as the result of multiplying this <see cref="Quantity"/>
        /// by a negative quantity (cannot return a <see cref="Quantity"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other quantity.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public Quantity Multiply(Quantity other)
        {
            return new Quantity(this.Value * other.Value, Math.Max(this.Precision, other.Precision));
        }
    }
}
