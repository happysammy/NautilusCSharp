//--------------------------------------------------------------
// <copyright file="MarketOrder.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Orders
{
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="MarketOrder"/> class. Represents a market order type.
    /// </summary>
    public class MarketOrder : Order
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOrder" /> class.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="timestamp">The order timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public MarketOrder(
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
                  OrderType.Market,
                  quantity,
                  timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(orderId, nameof(orderId));
            Validate.NotNull(orderLabel, nameof(orderLabel));
            Validate.NotDefault(orderSide, nameof(orderSide));
            Validate.NotNull(quantity, nameof(quantity));

            Debug.EqualTo(this.OrderType, nameof(this.OrderType), OrderType.Market);
        }
    }
}