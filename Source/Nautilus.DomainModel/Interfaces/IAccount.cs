//--------------------------------------------------------------------------------------------------
// <copyright file="IAccount.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a read-only brokerage account abstraction.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Gets the broker name.
        /// </summary>
        Broker Broker { get; }

        /// <summary>
        /// Gets the brokerage accounts identifier.
        /// </summary>
        EntityId<Account> Id { get; }

        /// <summary>
        /// Gets the brokerage accounts number.
        /// </summary>
        string AccountNumber { get; }

        /// <summary>
        /// Gets the brokerage accounts username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the brokerage accounts password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the brokerage accounts base currency.
        /// </summary>
        CurrencyCode Currency { get; }

        /// <summary>
        /// Gets the brokerage accounts cash balance.
        /// </summary>
        Money CashBalance { get; }

        /// <summary>
        /// Gets the brokerage accounts cash at the start of the day.
        /// </summary>
        Money CashStartDay { get; }

        /// <summary>
        /// Gets the brokerage accounts cash activity for the day.
        /// </summary>
        Money CashActivityDay { get; }

        /// <summary>
        /// Gets the brokerage accounts free equity.
        /// </summary>
        Money FreeEquity { get; }

        /// <summary>
        /// Gets the brokerage accounts margin used until liquidation.
        /// </summary>
        Money MarginUsedLiquidation { get; }

        /// <summary>
        /// Gets the brokerage accounts margin used for position maintenance.
        /// </summary>
        Money MarginUsedMaintenance { get; }

        /// <summary>
        /// Gets the brokerage accounts margin ratio.
        /// </summary>
        decimal MarginRatio { get; }

        /// <summary>
        /// Gets the brokerage accounts margin call status.
        /// </summary>
        string MarginCallStatus { get; }

        /// <summary>
        /// Gets the brokerage accounts last updated time.
        /// </summary>
        ZonedDateTime LastUpdated { get; }

        /// <summary>
        /// Gets the accounts event count.
        /// </summary>
        int EventCount { get; }
    }
}
