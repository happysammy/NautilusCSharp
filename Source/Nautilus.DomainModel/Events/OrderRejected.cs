//--------------------------------------------------------------------------------------------------
// <copyright file="OrderRejected.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been rejected by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderRejected : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRejected"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="rejectedTime">The event order rejected time.</param>
        /// <param name="rejectedReason">The event order rejected reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderRejected(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime rejectedTime,
            string rejectedReason,
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
            Debug.NotDefault(rejectedTime, nameof(rejectedTime));
            Debug.NotNull(rejectedReason, nameof(rejectedReason));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.RejectedTime = rejectedTime;
            this.RejectedReason = rejectedReason;
        }

        /// <summary>
        /// Gets the events order rejected time.
        /// </summary>
        public ZonedDateTime RejectedTime { get; }

        /// <summary>
        /// Gets the events order rejected reason.
        /// </summary>
        public string RejectedReason { get; }
    }
}
