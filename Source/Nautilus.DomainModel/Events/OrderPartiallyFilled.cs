//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPartiallyFilled.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been partially filled in the market (filled quantity
    /// less than order quantity).
    /// </summary>
    [Immutable]
    public sealed class OrderPartiallyFilled : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPartiallyFilled"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="executionTicket">The event order execution ticket.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="leavesQuantity">The event leaves quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderPartiallyFilled(
            Symbol symbol,
            EntityId orderId,
            EntityId executionId,
            EntityId executionTicket,
            OrderSide orderSide,
            Quantity filledQuantity,
            Quantity leavesQuantity,
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
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotNull(executionId, nameof(executionId));
            Debug.NotNull(executionTicket, nameof(executionTicket));
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotNull(filledQuantity, nameof(filledQuantity));
            Debug.NotNull(leavesQuantity, nameof(leavesQuantity));
            Debug.NotNull(averagePrice, nameof(averagePrice));
            Debug.NotDefault(executionTime, nameof(executionTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.ExecutionId = executionId;
            this.ExecutionTicket = executionTicket;
            this.OrderSide = orderSide;
            this.FilledQuantity = filledQuantity;
            this.LeavesQuantity = leavesQuantity;
            this.AveragePrice = averagePrice;
            this.ExecutionTime = executionTime;
        }

        /// <summary>
        /// Gets the events order execution identifier.
        /// </summary>
        public EntityId ExecutionId { get; }

        /// <summary>
        /// Gets the events order execution ticket.
        /// </summary>
        public EntityId ExecutionTicket { get; }

        /// <summary>
        /// Gets the events order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the events order filled quantity.
        /// </summary>
        public Quantity FilledQuantity { get; }

        /// <summary>
        /// Gets the events leaves quantity.
        /// </summary>
        public Quantity LeavesQuantity { get; }

        /// <summary>
        /// Gets the events order average price.
        /// </summary>
        public Price AveragePrice { get; }

        /// <summary>
        /// Gets the events order execution time.
        /// </summary>
        public ZonedDateTime ExecutionTime { get; }
    }
}
