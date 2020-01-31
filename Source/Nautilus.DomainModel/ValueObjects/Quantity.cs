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
    using System.Security.Permissions;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Primitives;

    /// <summary>
    /// Represents a non-negative decimal quantity.
    /// </summary>
    [Immutable]
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
        /// Returns a new <see cref="Quantity"/> parsed from the given string value.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The quantity value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Quantity Create(string value)
        {
            return Create(Convert.ToDecimal(value));
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
