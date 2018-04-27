// -------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelReject.cs" company="Nautech Systems Pty Ltd.">
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
    /// The immutable sealed <see cref="OrderCancelReject"/> class. Represents an event where a 
    /// request to cancel an order had been rejected by the broker.
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
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(orderId, nameof(orderId));
            Validate.NotDefault(cancelRejectedTime, nameof(cancelRejectedTime));
            Validate.NotNull(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
            Validate.NotNull(rejectedReason, nameof(rejectedReason));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

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

        /// <summary>
        /// Returns a string representation of the <see cref="OrderCancelReject"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderCancelReject);
    }
}