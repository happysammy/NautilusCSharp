//--------------------------------------------------------------------------------------------------
// <copyright file="AccountEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an account change event.
    /// </summary>
    [Immutable]
    public sealed class AccountEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEvent" /> class.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="broker">The account broker.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="currency">The currency of the account.</param>
        /// <param name="cashBalance">The account cash balance.</param>
        /// <param name="cashStartDay">The account cash start day.</param>
        /// <param name="cashActivityDay">The account cash activity day.</param>
        /// <param name="marginUsedLiquidation">The account margin used liquidation.</param>
        /// <param name="marginUsedMaintenance">The account margin used maintenance.</param>
        /// <param name="marginRatio">The account margin ratio.</param>
        /// <param name="marginCallStatus">The account margin call status.</param>
        /// <param name="eventId">The account event identifier.</param>
        /// <param name="eventTimestamp">The account event timestamp.</param>
        public AccountEvent(
            AccountId accountId,
            Brokerage broker,
            string accountNumber,
            CurrencyCode currency,
            Money cashBalance,
            Money cashStartDay,
            Money cashActivityDay,
            Money marginUsedLiquidation,
            Money marginUsedMaintenance,
            decimal marginRatio,
            string marginCallStatus,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(eventId, eventTimestamp)
        {
            Debug.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
            Debug.NotNull(cashBalance, nameof(cashBalance));
            Debug.NotNull(cashStartDay, nameof(cashStartDay));
            Debug.NotNull(cashActivityDay, nameof(cashActivityDay));
            Debug.NotNull(marginUsedLiquidation, nameof(marginUsedLiquidation));
            Debug.NotNull(marginUsedMaintenance, nameof(marginUsedMaintenance));
            Debug.NotNull(marginRatio, nameof(marginRatio));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
            this.Broker = broker;
            this.AccountNumber = accountNumber;
            this.Currency = currency;
            this.CashBalance = cashBalance;
            this.CashStartDay = cashStartDay;
            this.CashActivityDay = cashActivityDay;
            this.MarginRatio = marginRatio;
            this.MarginUsedLiquidation = marginUsedLiquidation;
            this.MarginUsedMaintenance = marginUsedMaintenance;
            this.MarginCallStatus = marginCallStatus;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the events broker name.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the events account number.
        /// </summary>
        public string AccountNumber { get; }

        /// <summary>
        /// Gets the events account currency.
        /// </summary>
        public CurrencyCode Currency { get; }

        /// <summary>
        /// Gets the events cash balance.
        /// </summary>
        public Money CashBalance { get; }

        /// <summary>
        /// Gets the events cash at the start of day.
        /// </summary>
        public Money CashStartDay { get; }

        /// <summary>
        /// Gets the events cash activity for the day.
        /// </summary>
        public Money CashActivityDay { get; }

        /// <summary>
        /// Gets the events margin used liquidation.
        /// </summary>
        public Money MarginUsedLiquidation { get; }

        /// <summary>
        /// Gets the events margin used maintenance.
        /// </summary>
        public Money MarginUsedMaintenance { get; }

        /// <summary>
        /// Gets the events margin ratio.
        /// </summary>
        public decimal MarginRatio { get; }

        /// <summary>
        /// Gets the events margin call status.
        /// </summary>
        public Option<string> MarginCallStatus { get; }
    }
}
