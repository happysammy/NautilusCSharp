//--------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a brokerage account.
    /// </summary>
    public sealed class Account : Aggregate<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="brokerage">The broker name.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        /// <param name="currency">The account base currency.</param>
        /// <param name="timestamp">The account creation timestamp.</param>
        /// <exception cref="ArgumentException">If any string is empty or whitespace.</exception>
        /// <exception cref="ArgumentException">If any struct is the default value.</exception>
        public Account(
            Brokerage brokerage,
            string accountNumber,
            string username,
            string password,
            Currency currency,
            ZonedDateTime timestamp)
            : base(
                AccountId.Create(brokerage, accountNumber),
                timestamp)
        {
            Condition.NotEmptyOrWhiteSpace(username, nameof(username));
            Condition.NotEmptyOrWhiteSpace(password, nameof(password));
            Condition.NotDefault(currency, nameof(currency));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Brokerage = brokerage;
            this.AccountNumber = accountNumber;
            this.Currency = currency;
            this.CashBalance = Money.Zero(this.Currency);
            this.CashStartDay = Money.Zero(this.Currency);
            this.CashActivityDay = Money.Zero(this.Currency);
            this.MarginUsedMaintenance = Money.Zero(this.Currency);
            this.MarginUsedLiquidation = Money.Zero(this.Currency);
            this.MarginCallStatus = string.Empty;
            this.LastUpdated = timestamp;
        }

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        public new AccountId Id => (AccountId)base.Id;

        /// <summary>
        /// Gets the accounts brokerage name.
        /// </summary>
        public Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the accounts number.
        /// </summary>
        public string AccountNumber { get; }

        /// <summary>
        /// Gets the accounts base currency.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Gets the accounts cash balance.
        /// </summary>
        public Money CashBalance { get; private set; }

        /// <summary>
        /// Gets the accounts cash at the start of day.
        /// </summary>
        public Money CashStartDay { get; private set; }

        /// <summary>
        /// Gets the accounts cash activity for the day.
        /// </summary>
        public Money CashActivityDay { get; private set; }

        /// <summary>
        /// Gets the accounts free equity.
        /// </summary>
        public Money FreeEquity => this.GetFreeEquity();

        /// <summary>
        /// Gets the accounts margin used before liquidation.
        /// </summary>
        public Money MarginUsedLiquidation { get; private set; }

        /// <summary>
        /// Gets the accounts margin used for maintenance.
        /// </summary>
        public Money MarginUsedMaintenance { get; private set; }

        /// <summary>
        /// Gets the accounts margin ratio.
        /// </summary>
        public decimal MarginRatio { get; private set; }

        /// <summary>
        /// Gets the accounts margin call status.
        /// </summary>
        public string MarginCallStatus { get; private set; }

        /// <summary>
        /// Gets the accounts last updated time.
        /// </summary>
        public ZonedDateTime LastUpdated { get; private set; }

        /// <summary>
        /// Applies the given event to the brokerage account.
        /// </summary>
        /// <param name="event">The event.</param>
        public void Apply(AccountStateEvent @event)
        {
            this.Events.Add(@event);
            this.CashBalance = @event.CashBalance;
            this.CashStartDay = @event.CashStartDay;
            this.CashActivityDay = @event.CashActivityDay;
            this.MarginRatio = @event.MarginRatio;
            this.MarginUsedMaintenance = @event.MarginUsedMaintenance;
            this.MarginUsedLiquidation = @event.MarginUsedLiquidation;
            this.MarginCallStatus = @event.MarginCallStatus;
            this.LastUpdated = @event.Timestamp;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Account"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(Account)}({this.Id})";

        private Money GetFreeEquity()
        {
            var marginUsed = this.MarginUsedMaintenance + this.MarginUsedLiquidation;
            var freeEquity = this.CashBalance - marginUsed;

            return freeEquity > 0
                ? Money.Create(Math.Max(0, this.CashBalance - marginUsed), this.Currency)
                : Money.Zero(this.Currency);
        }
    }
}
