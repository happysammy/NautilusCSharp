//--------------------------------------------------------------------------------------------------
// <copyright file="OrderModified.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderModified"/> class. Represents an event where an order
    /// had been modified at the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderModified : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderModified"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="brokerOrderId">The event broker order identifier.</param>
        /// <param name="modifiedPrice">The event order modified price.</param>
        /// <param name="acceptedTime">The event order modification accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderModified(
            Symbol symbol,
            EntityId orderId,
            EntityId brokerOrderId,
            Price modifiedPrice,
            ZonedDateTime acceptedTime,
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
            Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
            Validate.NotNull(modifiedPrice, nameof(modifiedPrice));
            Validate.NotDefault(acceptedTime, nameof(acceptedTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.BrokerOrderId = brokerOrderId;
            this.ModifiedPrice = modifiedPrice;
            this.AcceptedTime = acceptedTime;
        }

        /// <summary>
        /// Gets the events broker order identifier.
        /// </summary>
        public EntityId BrokerOrderId { get; }

        /// <summary>
        /// Gets the events order modified price.
        /// </summary>
        public Price ModifiedPrice { get; }

        /// <summary>
        /// Gets the events order modified accepted time.
        /// </summary>
        public ZonedDateTime AcceptedTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderModified"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderModified);
    }
}