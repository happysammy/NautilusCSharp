//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <returns>The market order.</returns>
        public static Order Market(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            ZonedDateTime timestamp)
        {
            return new Order(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.MARKET,
                quantity,
                null,
                TimeInForce.DAY,
                null,
                timestamp);
        }

        /// <summary>
        /// Creates and returns a new market if touched order.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <returns>The market if touched order.</returns>
        public static Order MarketIfTouched(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            return new Order(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.MIT,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp);
        }

        /// <summary>
        /// Creates and returns a new limit order.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <returns>The limit order.</returns>
        public static Order Limit(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            return new Order(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.LIMIT,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp);
        }

        /// <summary>
        /// Creates and returns a new stop market order.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <returns>The stop market order.</returns>
        public static Order StopMarket(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            return new Order(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.STOP_MARKET,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp);
        }

        /// <summary>
        /// Creates and returns a new stop limit order.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price (optional).</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <returns>The stop limit order.</returns>
        public static Order StopLimit(
            Symbol symbol,
            OrderId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            Price? price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            return new Order(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.STOP_LIMIT,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp);
        }
    }
}
