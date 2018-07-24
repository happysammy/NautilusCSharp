//--------------------------------------------------------------------------------------------------
// <copyright file="MarketIfTouchedOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Orders
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a market if touched order.
    /// </summary>
    public class MarketIfTouchedOrder : Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketIfTouchedOrder" /> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        public MarketIfTouchedOrder(
            Symbol symbol,
            EntityId orderId,
            Label orderLabel,
            OrderSide orderSide,
            Quantity quantity,
            ZonedDateTime timestamp)
            : base(
                  symbol,
                  orderId,
                  orderLabel,
                  orderSide,
                  OrderType.MIT,
                  quantity,
                  timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(orderLabel, nameof(orderLabel));
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.EqualTo(this.Type, nameof(this.Type), OrderType.MIT);
            Debug.NotNull(quantity, nameof(quantity));
        }
    }
}
