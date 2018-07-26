//--------------------------------------------------------------------------------------------------
// <copyright file="OrderInitialized.cs" company="Nautech Systems Pty Ltd">
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
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderInitialized(
            Symbol symbol,
            OrderId orderId,
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
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));
        }
    }
}
