//--------------------------------------------------------------------------------------------------
// <copyright file="AccountStateEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events.Base;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Events
{
    /// <summary>
    /// Represents an event where an account state has changed or been updated.
    /// </summary>
    [Immutable]
    public sealed class AccountStateEvent : AccountEvent
    {
        private static readonly Type EventType = typeof(AccountStateEvent);

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountStateEvent" /> class.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="currency">The currency of the account.</param>
        /// <param name="cashBalance">The account cash balance.</param>
        /// <param name="cashStartDay">The account cash start day.</param>
        /// <param name="cashActivityDay">The account cash activity day.</param>
        /// <param name="marginUsedLiquidation">The account margin used liquidation.</param>
        /// <param name="marginUsedMaintenance">The account margin used maintenance.</param>
        /// <param name="marginRatio">The account margin ratio.</param>
        /// <param name="marginCallStatus">The account margin call status.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public AccountStateEvent(
            AccountId accountId,
            Currency currency,
            Money cashBalance,
            Money cashStartDay,
            Money cashActivityDay,
            Money marginUsedLiquidation,
            Money marginUsedMaintenance,
            decimal marginRatio,
            string marginCallStatus,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                accountId,
                EventType,
                eventId,
                eventTimestamp)
        {
            Condition.NotDefault(currency, nameof(currency));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

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
        /// Gets the events account currency.
        /// </summary>
        public Currency Currency { get; }

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
        public string MarginCallStatus { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"Cash={this.CashBalance}, " +
                                             $"MarginUsed={this.MarginUsedMaintenance})";
    }
}
