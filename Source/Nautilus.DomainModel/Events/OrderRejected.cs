//--------------------------------------------------------------------------------------------------
// <copyright file="OrderRejected.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order had been rejected by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderRejected : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRejected"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="rejectedTime">The event order rejected time.</param>
        /// <param name="rejectedReason">The event order rejected reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderRejected(
            OrderId orderId,
            AccountId accountId,
            ZonedDateTime rejectedTime,
            string rejectedReason,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderRejected),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(rejectedTime, nameof(rejectedTime));
            Debug.NotEmptyOrWhiteSpace(rejectedReason, nameof(rejectedReason));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
            this.RejectedTime = rejectedTime;
            this.RejectedReason = rejectedReason;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

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
