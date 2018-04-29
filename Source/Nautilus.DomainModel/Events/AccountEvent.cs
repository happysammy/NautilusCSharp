//--------------------------------------------------------------------------------------------------
// <copyright file="AccountEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="AccountEvent"/> class. Represents an account event.
    /// </summary>
    [Immutable]
    public sealed class AccountEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountEvent" /> class.
        /// </summary>
        /// <param name="broker">The account broker.</param>
        /// <param name="accountNumber">The account identifier.</param>
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
            Broker broker,
            string accountNumber,
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
            Validate.NotNull(accountNumber, nameof(accountNumber));
            Validate.NotNull(cashBalance, nameof(cashBalance));
            Validate.NotNull(cashStartDay, nameof(cashStartDay));
            Validate.NotNull(cashActivityDay, nameof(cashActivityDay));
            Validate.NotNull(marginUsedLiquidation, nameof(marginUsedLiquidation));
            Validate.NotNull(marginUsedMaintenance, nameof(marginUsedMaintenance));

            this.Broker = broker;
            this.AccountNumber = accountNumber;
            this.CashBalance = cashBalance;
            this.CashStartDay = cashStartDay;
            this.CashActivityDay = cashActivityDay;
            this.MarginRatio = marginRatio;
            this.MarginUsedLiquidation = marginUsedLiquidation;
            this.MarginUsedMaintenance = marginUsedMaintenance;
            this.MarginCallStatus = marginCallStatus;
        }

        /// <summary>
        /// Gets the events broker name.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Gets the events account number.
        /// </summary>
        public string AccountNumber { get; }

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

        /// <summary>
        /// Returns a string representation of the <see cref="AccountEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(AccountEvent);
    }
}