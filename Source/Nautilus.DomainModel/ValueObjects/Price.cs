//--------------------------------------------------------------------------------------------------
// <copyright file="Price.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// Represents a none-negative financial market price.
    /// </summary>
    [Immutable]
    public sealed class Price : DecimalNumber<Price>, IComparable<DecimalNumber<Price>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Price"/> class.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="tickSize">The price tick size.</param>
        private Price(decimal value, decimal tickSize)
            : base(value)
        {
            Validate.DecimalNotOutOfRange(value, nameof(value), decimal.Zero, decimal.MaxValue);
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue);
            Validate.True(value.GetDecimalPlaces() <= tickSize.GetDecimalPlaces(), nameof(tickSize));

            this.TickSize = tickSize;
            this.Decimals = tickSize.GetDecimalPlaces();
        }

        /// <summary>
        /// Gets the prices tick size.
        /// </summary>
        public decimal TickSize { get; }

        /// <summary>
        /// Gets the prices number of decimal places.
        /// </summary>
        public int Decimals { get; }

        /// <summary>
        /// Creates a new <see cref="Price"/> with a value of zero.
        /// </summary>
        /// <returns>A <see cref="Price"/>.</returns>
        public static Price Zero()
        {
            return new Price(decimal.Zero, decimal.Zero);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> with the given value and tick size.
        /// </summary>
        /// <param name="value">The price value.</param>
        /// <param name="tickSize">The price tick size.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if either argument is negative, or if the
        /// decimal places of the value is greater than the decimal places of the tick size.</exception>
        public static Price Create(decimal value, decimal tickSize)
        {
            return new Price(value, tickSize);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the sum of this price value and the
        /// given price value.
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the other is null.</exception>
        public Price Add(Price other)
        {
            Validate.NotNull(other, nameof(other));

            return new Price(this.Value + other.Value, this.TickSize);
        }

        /// <summary>
        /// Returns a new <see cref="Price"/> as the result of the given <see cref="Price"/>
        /// subtracted from this <see cref="Price"/> (cannot return a <see cref="Price"/>
        /// with a negative value).
        /// </summary>
        /// <param name="other">The other price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if the other is null.</exception>
        public Price Subtract(Price other)
        {
            Validate.NotNull(other, nameof(other));

            return new Price(this.Value - other.Value, this.TickSize);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Price"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Value.ToString($"N{this.Decimals}", CultureInfo.InvariantCulture)}";

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/>.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[] { this.Value };
        }
    }
}
