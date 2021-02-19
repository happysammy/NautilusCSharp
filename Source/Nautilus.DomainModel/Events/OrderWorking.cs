﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderWorking.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Extensions;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events.Base;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Events
{
    /// <summary>
    /// Represents an event where an order is working with the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderWorking : OrderEvent
    {
        private static readonly Type EventType = typeof(OrderWorking);

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderWorking"/> class.
        /// </summary>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="orderIdBroker">The event order identifier from the broker.</param>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="orderType">The event order type.</param>
        /// <param name="quantity">The event order quantity.</param>
        /// <param name="price">The event order price.</param>
        /// <param name="timeInForce">The event order time in force.</param>
        /// <param name="expireTime">The event order expire time.</param>
        /// <param name="workingTime">The event order accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderWorking(
            AccountId accountId,
            OrderId orderId,
            OrderIdBroker orderIdBroker,
            Symbol symbol,
            OrderSide orderSide,
            OrderType orderType,
            Quantity quantity,
            Price price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime workingTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                EventType,
                eventId,
                eventTimestamp)
        {
            Condition.NotDefault(orderSide, nameof(orderSide));
            Condition.NotDefault(orderType, nameof(orderType));
            Condition.NotDefault(timeInForce, nameof(timeInForce));
            Debug.NotDefault(workingTime, nameof(workingTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
            this.OrderIdBroker = orderIdBroker;
            this.Symbol = symbol;
            this.OrderSide = orderSide;
            this.OrderType = orderType;
            this.Quantity = quantity;
            this.Price = price;
            this.TimeInForce = timeInForce;
            this.ExpireTime = expireTime;
            this.WorkingTime = workingTime;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the events order identifier from the broker.
        /// </summary>
        public OrderIdBroker OrderIdBroker { get; }

        /// <summary>
        /// Gets the events order symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the events order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the events order type.
        /// </summary>
        public OrderType OrderType { get; }

        /// <summary>
        /// Gets the events order quantity.
        /// </summary>
        public Quantity Quantity { get; }

        /// <summary>
        /// Gets the events order price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the events order time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the events order expire time (optional).
        /// </summary>
        public ZonedDateTime? ExpireTime { get; }

        /// <summary>
        /// Gets the events order working time.
        /// </summary>
        public ZonedDateTime WorkingTime { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            var expireTimeString = this.ExpireTime.HasValue
                ? " " + this.ExpireTime.Value.ToIso8601String()
                : string.Empty;

            return $"{this.Type.Name}(" +
                   $"AccountId={this.AccountId.Value},  " +
                   $"OrderId={this.OrderId.Value}, " +
                   $"OrderIdBroker={this.OrderIdBroker.Value}, " +
                   $"{this.OrderSide.ToString().ToUpper()} " +
                   $"{this.Quantity.ToStringFormatted()} " +
                   $"{this.Symbol.Value} " +
                   $"{this.OrderType} @ " +
                   $"{this.Price} " +
                   $"{this.TimeInForce}{expireTimeString})";
        }
    }
}
