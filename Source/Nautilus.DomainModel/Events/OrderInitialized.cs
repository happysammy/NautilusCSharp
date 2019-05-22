//--------------------------------------------------------------------------------------------------
// <copyright file="OrderInitialized.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order has been initialized by the system.
    /// </summary>
    [Immutable]
    public sealed class OrderInitialized : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderInitialized"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="eventIdentifier">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderInitialized(
            Symbol symbol,
            OrderId orderId,
            Guid eventIdentifier,
            ZonedDateTime eventTimestamp)
            : base(
                  symbol,
                  orderId,
                  eventIdentifier,
                  eventTimestamp)
        {
            Debug.NotDefault(eventIdentifier, nameof(eventIdentifier));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));
        }
    }
}
