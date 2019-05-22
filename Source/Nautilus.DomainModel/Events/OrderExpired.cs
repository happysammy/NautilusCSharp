//--------------------------------------------------------------------------------------------------
// <copyright file="OrderExpired.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order had expired at the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderExpired : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpired"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="expiredTime">The event order expired time.</param>
        /// <param name="eventIdentifier">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderExpired(
            Symbol symbol,
            OrderId orderId,
            ZonedDateTime expiredTime,
            Guid eventIdentifier,
            ZonedDateTime eventTimestamp)
            : base(
                  symbol,
                  orderId,
                  eventIdentifier,
                  eventTimestamp)
        {
            Debug.NotDefault(expiredTime, nameof(expiredTime));
            Debug.NotDefault(eventIdentifier, nameof(eventIdentifier));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.ExpiredTime = expiredTime;
        }

        /// <summary>
        /// Gets the events order expired time.
        /// </summary>
        public ZonedDateTime ExpiredTime { get; }
    }
}
