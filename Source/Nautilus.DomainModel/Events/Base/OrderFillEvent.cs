//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFillEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events.Base
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been completely filled.
    /// </summary>
    [Immutable]
    public abstract class OrderFillEvent : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFillEvent"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="executionTicket">The event order execution ticket.</param>
        /// <param name="symbol">The event order symbol.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        protected OrderFillEvent(
            OrderId orderId,
            AccountId accountId,
            ExecutionId executionId,
            ExecutionTicket executionTicket,
            Symbol symbol,
            OrderSide orderSide,
            Quantity filledQuantity,
            Price averagePrice,
            ZonedDateTime executionTime,
            Type eventType,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                eventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(orderSide, nameof(orderSide));
            Debug.NotDefault(executionTime, nameof(executionTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
            this.ExecutionId = executionId;
            this.ExecutionTicket = executionTicket;
            this.Symbol = symbol;
            this.OrderSide = orderSide;
            this.FilledQuantity = filledQuantity;
            this.AveragePrice = averagePrice;
            this.ExecutionTime = executionTime;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the events order execution identifier.
        /// </summary>
        public ExecutionId ExecutionId { get; }

        /// <summary>
        /// Gets the events order execution ticket.
        /// </summary>
        public ExecutionTicket ExecutionTicket { get; }

        /// <summary>
        /// Gets the events order symbol.
        /// </summary>
        public Symbol Symbol { get; }

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
