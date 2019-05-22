//--------------------------------------------------------------------------------------------------
// <copyright file="OrderSubmitted.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been submitted by the system to the broker.
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
        /// <param name="eventIdentifier">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderSubmitted(
            Symbol symbol,
            OrderId orderId,
            ZonedDateTime submittedTime,
            Guid eventIdentifier,
            ZonedDateTime eventTimestamp)
            : base(
                  symbol,
                  orderId,
                  eventIdentifier,
                  eventTimestamp)
        {
            Debug.NotDefault(submittedTime, nameof(submittedTime));
            Debug.NotDefault(eventIdentifier, nameof(eventIdentifier));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.SubmittedTime = submittedTime;
        }

        /// <summary>
        /// Gets the events order submitted time.
        /// </summary>
        public ZonedDateTime SubmittedTime { get; }
    }
}
