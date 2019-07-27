//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelled.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
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
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="cancelledTime">The event order cancelled time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderCancelled(
            OrderId orderId,
            ZonedDateTime cancelledTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderCancelled),
                eventId,
                eventTimestamp)
        {
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
