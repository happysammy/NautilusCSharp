//--------------------------------------------------------------------------------------------------
// <copyright file="OrderAccepted.cs" company="Nautech Systems Pty Ltd">
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
    /// The immutable sealed <see cref="OrderAccepted"/> class. Represents an event where an order
    /// had been accepted by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderAccepted : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAccepted"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="acceptedTime">The event accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderAccepted(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime acceptedTime,
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
            Validate.NotDefault(acceptedTime, nameof(acceptedTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AcceptedTime = acceptedTime;
        }

        /// <summary>
        /// Gets the events order accepted time.
        /// </summary>
        public ZonedDateTime AcceptedTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderAccepted"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderAccepted);
    }
}