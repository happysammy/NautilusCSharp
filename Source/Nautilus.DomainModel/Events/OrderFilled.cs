//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFilled.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events.Base;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Events
{
    /// <summary>
    /// Represents an event where an order has been completely filled.
    /// </summary>
    [Immutable]
    public sealed class OrderFilled : OrderFillEvent
    {
        private static readonly Type EventType = typeof(OrderFilled);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFilled"/> class.
        /// </summary>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="positionIdBroker">The event order execution ticket.</param>
        /// <param name="symbol">The event order symbol.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="currency">The event order transaction currency.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderFilled(
            AccountId accountId,
            OrderId orderId,
            ExecutionId executionId,
            PositionIdBroker positionIdBroker,
            Symbol symbol,
            OrderSide orderSide,
            Quantity filledQuantity,
            Price averagePrice,
            Currency currency,
            ZonedDateTime executionTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                accountId,
                orderId,
                executionId,
                positionIdBroker,
                symbol,
                orderSide,
                filledQuantity,
                averagePrice,
                currency,
                executionTime,
                EventType,
                eventId,
                eventTimestamp)
        {
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"ExecutionId={this.ExecutionId.Value}, " +
                                             $"PositionIdBroker={this.PositionIdBroker.Value}, " +
                                             $"{this.OrderSide.ToString().ToUpper()} " +
                                             $"{this.FilledQuantity.ToStringFormatted()} " +
                                             $"{this.Symbol.Value} @ " +
                                             $"{this.AveragePrice})";
    }
}
