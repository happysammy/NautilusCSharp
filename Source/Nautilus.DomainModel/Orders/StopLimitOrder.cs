//--------------------------------------------------------------------------------------------------
// <copyright file="StopLimitOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Orders
{
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a StopLimit order type.
    /// </summary>
    public class StopLimitOrder : StopOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopLimitOrder"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order entry price.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The expire time (optional).</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public StopLimitOrder(
            Symbol symbol,
            EntityId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            Price price,
            TimeInForce timeInForce,
            Option<ZonedDateTime?> expireTime,
            ZonedDateTime timestamp)
            : base(
                symbol,
                orderId,
                orderLabel,
                orderSide,
                OrderType.StopLimit,
                quantity,
                price,
                timeInForce,
                expireTime,
                timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(orderLabel, nameof(orderLabel));
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotNull(quantity, nameof(quantity));
            Debug.NotNull(price, nameof(price));
            Debug.NotDefault(timeInForce, nameof(timeInForce));
            Debug.NotNull(expireTime, nameof(expireTime));

            Debug.EqualTo(this.OrderType, nameof(this.OrderType), OrderType.StopLimit);
        }
    }
}
