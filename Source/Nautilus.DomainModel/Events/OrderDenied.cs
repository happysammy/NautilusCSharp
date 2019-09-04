//--------------------------------------------------------------------------------------------------
// <copyright file="OrderDenied.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order had been denied by the system.
    /// </summary>
    [Immutable]
    public sealed class OrderDenied : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDenied"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="message">The event message.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderDenied(
            OrderId orderId,
            string message,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderAccepted),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.Message = message;
        }

        /// <summary>
        /// Gets the events message.
        /// </summary>
        public string Message { get; }
    }
}
