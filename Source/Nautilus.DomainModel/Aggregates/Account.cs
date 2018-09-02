//--------------------------------------------------------------------------------------------------
// <copyright file="Account.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates.Base;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a brokerage account.
    /// </summary>
    public sealed class Account : Aggregate<Account>, IAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="broker">The broker name.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        /// <param name="currency">The account base currency.</param>
        /// <param name="timestamp">The account creation timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if
        /// any struct argument is the default value.</exception>
        public Account(
            AccountId accountId,
            Broker broker,
            string accountNumber,
            string username,
            string password,
            CurrencyCode currency,
            ZonedDateTime timestamp)
            : base(
                accountId,
                timestamp)
        {
            Validate.NotNull(accountId, nameof(accountId));
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(password, nameof(password));
            Validate.NotDefault(currency, nameof(currency));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Broker = broker;
            this.AccountNumber = accountNumber;
            this.Username = username;
            this.Password = password;
            this.Currency = currency;
            this.CashBalance = Money.Zero(this.Currency);
            this.CashStartDay = Money.Zero(this.Currency);
            this.CashActivityDay = Money.Zero(this.Currency);
            this.MarginUsedMaintenance = Money.Zero(this.Currency);
            this.MarginUsedLiquidation = Money.Zero(this.Currency);
            this.LastUpdated = timestamp;
        }

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        public new AccountId Id => base.Id as AccountId;

        /// <summary>
        /// Gets the accounts broker name.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Gets the accounts number.
        /// </summary>
        public string AccountNumber { get; }

        /// <summary>
        /// Gets the accounts username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the accounts password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the accounts base currency.
        /// </summary>
        public CurrencyCode Currency { get; }

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
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the event is null.</exception>
        public override CommandResult Apply(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.True(@event is AccountEvent, nameof(@event));

            var accountEvent = @event as AccountEvent;

            if (accountEvent is null)
            {
                return CommandResult.Fail($"Event not recognized by {this}).");
            }

            if (accountEvent.AccountId != this.Id)
            {
                return CommandResult.Fail($"Event account identifier does not match {this}).");
            }

            this.CashBalance = accountEvent.CashBalance;
            this.CashStartDay = accountEvent.CashStartDay;
            this.CashActivityDay = accountEvent.CashActivityDay;
            this.MarginRatio = accountEvent.MarginRatio;
            this.MarginUsedMaintenance = accountEvent.MarginUsedMaintenance;
            this.MarginUsedLiquidation = accountEvent.MarginUsedLiquidation;
            this.MarginCallStatus = accountEvent.MarginCallStatus.Value;
            this.LastUpdated = accountEvent.Timestamp;

            this.Events.Add(@event);

            return CommandResult.Ok();
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
