//--------------------------------------------------------------------------------------------------
// <copyright file="IOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a read-only interface for orders.
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
        EntityId<Order> Id { get; }

        /// <summary>
        /// Gets the orders identifier count.
        /// </summary>
        int IdCount { get; }

        /// <summary>
        /// Gets the orders current identifier.
        /// </summary>
        OrderId IdCurrent { get; }

        /// <summary>
        /// Gets the orders current identifier for the broker.
        /// </summary>
        Option<OrderId> IdBroker { get; }

        /// <summary>
        /// Gets the orders current execution identifier.
        /// </summary>
        Option<ExecutionId> ExecutionId { get; }

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
        /// Gets the orders price (optional).
        /// </summary>
        Option<Price> Price { get; }

        /// <summary>
        /// Gets the orders average fill price (optional, may be unfilled).
        /// </summary>
        Option<Price> AveragePrice { get; }

        /// <summary>
        /// The orders slippage.
        /// </summary>
        decimal Slippage { get; }

        /// <summary>
        /// Gets the orders time in force.
        /// </summary>
        TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the orders expire time (optional).
        /// </summary>
        Option<ZonedDateTime?> ExpireTime { get; }

        /// <summary>
        /// Gets the orders last event time.
        /// </summary>
        ZonedDateTime LastEventTime { get; }

        /// <summary>
        /// Gets the orders initialization timestamp.
        /// </summary>
        ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Gets the current order status.
        /// </summary>
        OrderStatus Status { get; }

        /// <summary>
        /// Gets a result indicating whether the order status is complete.
        /// </summary>
        bool IsComplete { get; }
    }
}
