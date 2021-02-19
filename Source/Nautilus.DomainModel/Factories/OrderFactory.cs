//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFactory.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Factories
{
    /// <summary>
    /// Provides a factory for creating different <see cref="Order"/> types.
    /// </summary>
    public static class OrderFactory
    {
        /// <summary>
        /// Creates and returns a new market order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="side">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The market order.</returns>
        public static Order Market(
            OrderId orderId,
            Symbol symbol,
            OrderSide side,
            Quantity quantity,
            ZonedDateTime timestamp,
            Guid initEventId)
        {
            return Order.Create(
                orderId,
                symbol,
                side,
                OrderType.Market,
                quantity,
                null,
                TimeInForce.DAY,
                null,
                timestamp,
                initEventId);
        }

        /// <summary>
        /// Creates and returns a new limit order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="side">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The limit order.</returns>
        public static Order Limit(
            OrderId orderId,
            Symbol symbol,
            OrderSide side,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp,
            Guid initEventId)
        {
            return Order.Create(
                orderId,
                symbol,
                side,
                OrderType.Limit,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp,
                initEventId);
        }

        /// <summary>
        /// Creates and returns a new stop market order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="side">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The stop market order.</returns>
        public static Order StopMarket(
            OrderId orderId,
            Symbol symbol,
            OrderSide side,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp,
            Guid initEventId)
        {
            return Order.Create(
                orderId,
                symbol,
                side,
                OrderType.Stop,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp,
                initEventId);
        }

        /// <summary>
        /// Creates and returns a new stop limit order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="side">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The stop limit order.</returns>
        public static Order StopLimit(
            OrderId orderId,
            Symbol symbol,
            OrderSide side,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp,
            Guid initEventId)
        {
            return Order.Create(
                orderId,
                symbol,
                side,
                OrderType.StopLimit,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp,
                initEventId);
        }
    }
}
