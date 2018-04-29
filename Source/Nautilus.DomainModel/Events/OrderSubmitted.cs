//--------------------------------------------------------------------------------------------------
// <copyright file="OrderSubmitted.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderSubmitted"/> class. Represents an event where an order
    /// had been submitted by the system to the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderSubmitted : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSubmitted"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="submittedTime">The event submitted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderSubmitted(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime submittedTime,
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
            Validate.NotDefault(submittedTime, nameof(submittedTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.SubmittedTime = submittedTime;
        }

        /// <summary>
        /// Gets the events order submitted time.
        /// </summary>
        public ZonedDateTime SubmittedTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderSubmitted"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderSubmitted);
    }
}