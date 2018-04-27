// -------------------------------------------------------------------------------------------------
// <copyright file="OrderExpired.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderExpired"/> class. Represents an event where an order
    /// had expired at the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderExpired : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpired"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="expiredTime">The event order expired time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderExpired(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime expiredTime,
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
            Validate.NotDefault(expiredTime, nameof(expiredTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.ExpiredTime = expiredTime;
        }

        /// <summary>
        /// Gets the events order expired time.
        /// </summary>
        public ZonedDateTime ExpiredTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderExpired"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderExpired);
    }
}