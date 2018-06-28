//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelReject.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where a request to cancel an order had been rejected by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderCancelReject : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCancelReject"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="cancelRejectedTime">The event order rejected time.</param>
        /// <param name="cancelRejectResponseTo">The event cancel reject response.</param>
        /// <param name="rejectedReason">The event order cancel rejected reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderCancelReject(
            Symbol symbol,
            EntityId orderId,
            ZonedDateTime cancelRejectedTime,
            string cancelRejectResponseTo,
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
            Debug.NotDefault(cancelRejectedTime, nameof(cancelRejectedTime));
            Debug.NotNull(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
            Debug.NotNull(rejectedReason, nameof(rejectedReason));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.CancelRejectedTime = cancelRejectedTime;
            this.CancelRejectResponseTo = cancelRejectResponseTo;
            this.RejectedReason = rejectedReason;
        }

        /// <summary>
        /// Gets the events order cancel rejected time.
        /// </summary>
        public ZonedDateTime CancelRejectedTime { get; }

        /// <summary>
        /// Gets the events order cancel reject response.
        /// </summary>
        public string CancelRejectResponseTo { get; }

        /// <summary>
        /// Gets the events order cancel rejected reason.
        /// </summary>
        public string RejectedReason { get; }
    }
}
