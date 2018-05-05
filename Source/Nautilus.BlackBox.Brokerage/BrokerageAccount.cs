//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageAccount.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using System;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Core;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="BrokerageAccount"/> class.
    /// </summary>
    public sealed class BrokerageAccount : Aggregate<BrokerageAccount>, IBrokerageAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerageAccount"/> class.
        /// </summary>
        /// <param name="broker">The broker name.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        /// <param name="currency">The account base currency.</param>
        /// <param name="timestamp">The account creation timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if
        /// any struct argument is the default value.</exception>
        public BrokerageAccount(
            Broker broker,
            string username,
            string password,
            string accountNumber,
            CurrencyCode currency,
            ZonedDateTime timestamp)
            : base(
                EntityIdFactory.Account(broker, accountNumber),
                timestamp)
        {
            Validate.NotNull(accountNumber, nameof(accountNumber));
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(password, nameof(password));
            Validate.NotDefault(currency, nameof(currency));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Broker = broker;
            this.Username = username;
            this.Password = password;
            this.AccountNumber = accountNumber;
            this.Currency = currency;
            this.CashBalance = Money.Zero(this.Currency);
            this.CashStartDay = Money.Zero(this.Currency);
            this.CashActivityDay = Money.Zero(this.Currency);
            this.MarginUsedMaintenance = Money.Zero(this.Currency);
            this.MarginUsedLiquidation = Money.Zero(this.Currency);
            this.LastUpdated = timestamp;
        }

        /// <summary>
        /// Gets the accounts broker name.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Gets the accounts identifier.
        /// </summary>
        public EntityId AccountId => this.EntityId;

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
            Validate.NotNull(@event, nameof(@event));
            Validate.True(@event is AccountEvent, nameof(@event));

            var accountEvent = @event as AccountEvent;

            if (accountEvent is null)
            {
                return CommandResult.Fail($"Command Failure (Event not recognized by {this}).");
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
