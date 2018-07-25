//--------------------------------------------------------------------------------------------------
// <copyright file="IOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a read-only interface for all orders.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Gets the orders symbol.
        /// </summary>
        Symbol Symbol { get; }

        /// <summary>
        /// Gets the orders identifier.
        /// </summary>
        EntityId OrderId { get; }

        /// <summary>
        /// Gets the orders identifiers count.
        /// </summary>
        int OrderIdCount { get; }

        /// <summary>
        /// Gets the orders current identifier.
        /// </summary>
        EntityId OrderIdCurrent { get; }

        /// <summary>
        /// Gets the orders current identifier for the broker.
        /// </summary>
        EntityId OrderIdBroker { get; }

        /// <summary>
        /// Gets the orders current execution identifier.
        /// </summary>
        EntityId ExecutionId { get; }

        /// <summary>
        /// Gets the orders label.
        /// </summary>
        Label Label { get; }

        /// <summary>
        /// Gets the orders type.
        /// </summary>
        OrderType Type { get; }

        /// <summary>
        /// Gets the orders side.
        /// </summary>
        OrderSide Side { get; }

        /// <summary>
        /// Gets the orders quantity.
        /// </summary>
        Quantity Quantity { get; }

        /// <summary>
        /// Gets the orders filled quantity.
        /// </summary>
        Quantity FilledQuantity { get; }

        /// <summary>
        /// Gets the orders average fill price.
        /// </summary>
        Price AveragePrice { get; }

        /// <summary>
        /// Gets the current order status.
        /// </summary>
        OrderStatus Status { get; }

        /// <summary>
        /// Gets the orders last event time.
        /// </summary>
        ZonedDateTime LastEventTime { get; }

        /// <summary>
        /// Gets the orders timestamp.
        /// </summary>
        ZonedDateTime OrderTimestamp { get; }

        /// <summary>
        /// Gets a result indicating whether the order status is complete.
        /// </summary>
        bool IsComplete { get; }
    }
}
