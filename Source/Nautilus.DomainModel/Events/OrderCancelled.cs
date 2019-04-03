//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelled.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been cancelled by the broker.
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
            OrderId orderId,
            ZonedDateTime cancelledTime,
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
            Debug.NotDefault(cancelledTime, nameof(cancelledTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.CancelledTime = cancelledTime;
        }

        /// <summary>
        /// Gets the events order cancelled time.
        /// </summary>
        public ZonedDateTime CancelledTime { get; }
    }
}
