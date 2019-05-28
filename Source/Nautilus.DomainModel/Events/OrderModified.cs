//--------------------------------------------------------------------------------------------------
// <copyright file="OrderModified.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had been modified with the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderModified : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderModified"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="orderIdBroker">The event broker order identifier.</param>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="modifiedPrice">The event order modified price.</param>
        /// <param name="modifiedTime">The event order modification accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderModified(
            OrderId orderId,
            OrderId orderIdBroker,
            Symbol symbol,
            Price modifiedPrice,
            ZonedDateTime modifiedTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                symbol,
                typeof(OrderModified),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(modifiedTime, nameof(modifiedTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.OrderIdBroker = orderIdBroker;
            this.ModifiedPrice = modifiedPrice;
            this.ModifiedTime = modifiedTime;
        }

        /// <summary>
        /// Gets the events broker order identifier.
        /// </summary>
        public OrderId OrderIdBroker { get; }

        /// <summary>
        /// Gets the events order modified price.
        /// </summary>
        public Price ModifiedPrice { get; }

        /// <summary>
        /// Gets the events order modified accepted time.
        /// </summary>
        public ZonedDateTime ModifiedTime { get; }
    }
}
