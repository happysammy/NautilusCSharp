//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFilled.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been completely filled in the market (filled quantity
    /// equal to order quantity).
    /// </summary>
    [Immutable]
    public sealed class OrderFilled : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFilled"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="executionTicket">The event order execution ticket.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderFilled(
            Symbol symbol,
            OrderId orderId,
            ExecutionId executionId,
            ExecutionId executionTicket,
            OrderSide orderSide,
            Quantity filledQuantity,
            Price averagePrice,
            ZonedDateTime executionTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                  symbol,
                  orderId,
                  eventId,
                  eventTimestamp)
        {
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotDefault(executionTime, nameof(executionTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.ExecutionId = executionId;
            this.ExecutionTicket = executionTicket;
            this.OrderSide = orderSide;
            this.FilledQuantity = filledQuantity;
            this.AveragePrice = averagePrice;
            this.ExecutionTime = executionTime;
        }

        /// <summary>
        /// Gets the events order execution identifier.
        /// </summary>
        public ExecutionId ExecutionId { get; }

        /// <summary>
        /// Gets the events order execution ticket.
        /// </summary>
        public ExecutionId ExecutionTicket { get; }

        /// <summary>
        /// Gets the events order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the events order filled quantity.
        /// </summary>
        public Quantity FilledQuantity { get; }

        /// <summary>
        /// Gets the events order average filled price.
        /// </summary>
        public Price AveragePrice { get; }

        /// <summary>
        /// Gets the events order execution time.
        /// </summary>
        public ZonedDateTime ExecutionTime { get; }
    }
}
