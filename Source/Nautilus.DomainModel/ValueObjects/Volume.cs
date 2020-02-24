//--------------------------------------------------------------------------------------------------
// <copyright file="Volume.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a non-negative volume with a specified decimal precision.
    /// </summary>
    [Immutable]
    public sealed class Volume : DecimalNumber
    {
        private static readonly Volume VolumeZero = new Volume(decimal.Zero, 0);
        private static readonly Volume VolumeOne = new Volume(decimal.One, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Volume"/> class.
        /// </summary>
        /// <param name="value">The volume value.</param>
        /// <param name="precision">The precision of the volume.</param>
        private Volume(decimal value, byte precision)
            : base(value, precision)
        {
            Debug.NotNegativeDecimal(value, nameof(value));
        }

        /// <summary>
        /// Returns a new <see cref="Volume"/> with zero value.
        /// </summary>
        /// <returns>A <see cref="Volume"/>.</returns>
        public static Volume Zero() => VolumeZero;

        /// <summary>
        /// Returns a new <see cref="Volume"/> with zero value.
        /// </summary>
        /// <returns>A <see cref="Volume"/>.</returns>
        public static Volume One() => VolumeOne;

        /// <summary>
        /// Returns a new <see cref="Volume"/> parsed from the given string value.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The volume value.</param>
        /// <returns>A <see cref="Volume"/>.</returns>
        public static Volume Create(string value)
        {
            return Create(Convert.ToDecimal(value));
        }

        /// <summary>
        /// Returns a new <see cref="Volume"/> with the given amount.
        /// The decimal precision is inferred.
        /// </summary>
        /// <param name="value">The volume value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public static Volume Create(decimal value)
        {
            return new Volume(value, value.GetDecimalPlaces());
        }

        /// <summary>
        /// Returns a new <see cref="Volume"/> with the given amount.
        /// </summary>
        /// <param name="value">The volume value.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        /// <param name="precision">The precision of the volume.</param>
        public static Volume Create(decimal value, byte precision)
        {
            return new Volume(value, precision);
        }

        /// <summary>
        /// Returns a new <see cref="Volume"/> as the result of the sum of this <see cref="Volume"/>
        /// and the given <see cref="Volume"/>.
        /// </summary>
        /// <param name="other">The other volume.</param>
        /// <returns>A <see cref="Volume"/>.</returns>
        public Volume Add(Volume other)
        {
            return new Volume(this.Value + other.Value, Math.Max(this.Precision, other.Precision));
        }

        /// <summary>
        /// Returns a new <see cref="Volume"/> as the result of the given <see cref="Volume"/>
        /// subtracted from this <see cref="Volume"/> (cannot return a <see cref="Volume"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other volume.</param>
        /// <returns>A <see cref="Volume"/>.</returns>
        public Volume Subtract(Volume other)
        {
            Debug.True(other.Value <= this.Value, nameof(other));

            return new Volume(this.Value - other.Value, Math.Max(this.Precision, other.Precision));
        }
    }
}
