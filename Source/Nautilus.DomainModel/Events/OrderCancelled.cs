//--------------------------------------------------------------
// <copyright file="OrderCancelled.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderCancelled"/> class. Represents an event where an order
    /// had been cancelled by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderCancelled : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCancelled"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="cancelledTime">The event order cancelled time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderCancelled(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime cancelledTime,
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
            Validate.NotDefault(cancelledTime, nameof(cancelledTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.CancelledTime = cancelledTime;
        }

        /// <summary>
        /// Gets the events order cancelled time.
        /// </summary>
        public ZonedDateTime CancelledTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderCancelled"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderCancelled);
    }
}