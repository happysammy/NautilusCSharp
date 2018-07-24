//--------------------------------------------------------------------------------------------------
// <copyright file="StopMarketOrder.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a stop-market order type.
    /// </summary>
    public class StopMarketOrder : PricedOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopMarketOrder"/> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order id.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time.</param>
        /// <param name="timestamp">The order timestamp.</param>
        public StopMarketOrder(
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
                OrderType.STOP_MARKET,
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
            Debug.EqualTo(this.Type, nameof(this.Type), OrderType.STOP_MARKET);
            Debug.NotNull(quantity, nameof(quantity));
            Debug.NotNull(price, nameof(price));
            Debug.NotDefault(timeInForce, nameof(timeInForce));
            Debug.NotNull(expireTime, nameof(expireTime));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
