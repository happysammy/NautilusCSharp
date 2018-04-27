// -------------------------------------------------------------------------------------------------
// <copyright file="OrderFilled.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderFilled"/> class. Represents an event where an order had
    /// been completely filled in the market (filled quantity equal to order quantity).
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
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderFilled(
            Symbol symbol,
            EntityId orderId,
            EntityId executionId,
            EntityId executionTicket,
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
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(orderId, nameof(orderId));
            Validate.NotNull(executionId, nameof(executionId));
            Validate.NotNull(executionTicket, nameof(executionTicket));
            Validate.NotDefault(orderSide, nameof(orderSide));
            Validate.NotNull(filledQuantity, nameof(filledQuantity));
            Validate.NotNull(averagePrice, nameof(averagePrice));
            Validate.NotDefault(executionTime, nameof(executionTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

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
        /// Gets the events order average filled price.
        /// </summary>
        public Price AveragePrice { get; }

        /// <summary>
        /// Gets the events order execution time.
        /// </summary>
        public ZonedDateTime ExecutionTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderFilled"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderFilled);
    }
}