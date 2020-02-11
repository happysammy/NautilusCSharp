//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

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
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="purpose">The order purpose.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The market order.</returns>
        public static Order Market(
            OrderId orderId,
            Symbol symbol,
            Label label,
            OrderSide side,
            OrderPurpose purpose,
            Quantity quantity,
            ZonedDateTime timestamp,
            Guid initEventId)
        {
            return Order.Create(
                orderId,
                symbol,
                label,
                side,
                OrderType.Market,
                purpose,
                quantity,
                null,
                TimeInForce.DAY,
                null,
                timestamp,
                initEventId);
        }

        /// <summary>
        /// Creates and returns a new market if touched order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="purpose">The order purpose.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <param name="initEventId">The order initialization event GUID.</param>
        /// <returns>The market if touched order.</returns>
        public static Order MarketIfTouched(
            OrderId orderId,
            Symbol symbol,
            Label label,
            OrderSide side,
            OrderPurpose purpose,
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
                label,
                side,
                OrderType.MIT,
                purpose,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp,
                initEventId);
        }

        /// <summary>
        /// Creates and returns a new limit order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="purpose">The order purpose.</param>
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
            Label label,
            OrderSide side,
            OrderPurpose purpose,
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
                label,
                side,
                OrderType.Limit,
                purpose,
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
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="purpose">The order purpose.</param>
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
            Label label,
            OrderSide side,
            OrderPurpose purpose,
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
                label,
                side,
                OrderType.Stop,
                purpose,
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
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="purpose">The order purpose.</param>
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
            Label label,
            OrderSide side,
            OrderPurpose purpose,
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
                label,
                side,
                OrderType.StopLimit,
                purpose,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp,
                initEventId);
        }
    }
}
