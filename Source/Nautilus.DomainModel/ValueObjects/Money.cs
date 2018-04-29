//--------------------------------------------------------------------------------------------------
// <copyright file="Money.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// The immutable sealed <see cref="Money"/> class.
    /// </summary>
    [Immutable]
    public sealed class Money : DecimalNumber<Money>, IComparable<DecimalNumber<Money>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        private Money(decimal amount, CurrencyCode currency)
            : base(amount)
        {
            Validate.NotDefault(currency, nameof(currency));
            Validate.DecimalNotOutOfRange(amount, nameof(amount), decimal.Zero, decimal.MaxValue);
            Validate.True(amount % 0.01m == 0, nameof(amount));

            this.Currency = currency;
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public CurrencyCode Currency { get; }

        /// <summary>
        /// Returns a new <see cref="Money"/> object with a value of zero.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the currency is the default value
        /// (unknown).</exception>
        public static Money Zero(CurrencyCode currency)
        {
            return new Money(decimal.Zero, currency);
        }

        /// <summary>
        /// Returns a new <see cref="Money"/> object with the given amount, of the given currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the amount is negative, or if the
        /// currency is the default value (Unknown).</exception>
        public static Money Create(decimal amount, CurrencyCode currency)
        {
            return new Money(amount, currency);
        }

        /// <summary>
        /// Adds the given money to this money.
        /// </summary>
        /// <param name="other">The amount.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public Money Add(Money other)
        {
            Validate.NotNull(other, nameof(other));

            return new Money(this.Value + other.Value, this.Currency);
        }

        /// <summary>
        /// Subtracts the given money from this money.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the argument is null, or if the other
        /// value is greater than this value.</exception>
        public Money Subtract(Money other)
        {
            Validate.NotNull(other, nameof(other));
            Validate.True(other.Value <= this.Value, nameof(other));

            return new Money(this.Value - other.Value, this.Currency);
        }

        /// <summary>
        /// Multiplies this money by the given multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the multiplier is negative.</exception>
        public Money MultiplyBy(int multiplier)
        {
            Validate.Int32NotOutOfRange(multiplier, nameof(multiplier), 0, int.MaxValue);

            return new Money(this.Value * multiplier, this.Currency);
        }

        /// <summary>
        /// Divides this money by the given divisor.
        /// </summary>
        /// <param name="divisor">The divisor.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        /// <exception cref="ValidationException">Throws if the divisor is zero or negative.</exception>
        public Money DivideBy(int divisor)
        {
            Validate.Int32NotOutOfRange(divisor, nameof(divisor), 0, int.MaxValue, RangeEndPoints.LowerExclusive);

            return new Money(this.Value / divisor, this.Currency);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Money"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Value:N2}({this.Currency})";
        }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[] { this.Value, this.Currency };
        }
    }
}
