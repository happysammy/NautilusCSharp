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

    /// <summary>
    /// Represents a brokerage account.
    /// </summary>
    public sealed class Account : Aggregate<AccountId, AccountStateEvent, Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="event">The initial account state event.</param>
        /// <exception cref="ArgumentException">If any string is empty or whitespace.</exception>
        /// <exception cref="ArgumentException">If any struct is the default value.</exception>
        public Account(AccountStateEvent @event)
            : base(@event.AccountId, @event)
        {
            this.Brokerage = this.Id.Brokerage;
            this.AccountNumber = this.Id.AccountNumber;

            this.Currency = @event.Currency;
            this.CashBalance = @event.CashBalance;
            this.CashStartDay = @event.CashStartDay;
            this.CashActivityDay = @event.CashActivityDay;
            this.MarginUsedMaintenance = @event.MarginUsedMaintenance;
            this.MarginUsedLiquidation = @event.MarginUsedLiquidation;
            this.MarginCallStatus = @event.MarginCallStatus;
        }

        /// <summary>
        /// Gets the accounts brokerage name.
        /// </summary>
        public Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the accounts number.
        /// </summary>
        public AccountNumber AccountNumber { get; }

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

        /// <inheritdoc />
        protected override void OnEvent(AccountStateEvent @event)
        {
            Debug.EqualTo(@event.AccountId, this.Id, nameof(@event.AccountId));

            this.CashBalance = @event.CashBalance;
            this.CashStartDay = @event.CashStartDay;
            this.CashActivityDay = @event.CashActivityDay;
            this.MarginRatio = @event.MarginRatio;
            this.MarginUsedMaintenance = @event.MarginUsedMaintenance;
            this.MarginUsedLiquidation = @event.MarginUsedLiquidation;
            this.MarginCallStatus = @event.MarginCallStatus;
        }

        private Money GetFreeEquity()
        {
            var totalUsedMargin = this.MarginUsedMaintenance + this.MarginUsedLiquidation;

            return Money.Create(Math.Max(0, this.CashBalance - totalUsedMargin), this.Currency);
        }
    }
}
