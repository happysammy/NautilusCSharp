//--------------------------------------------------------------------------------------------------
// <copyright file="OrderModified.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order has been modified with the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderModified : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderModified"/> class.
        /// </summary>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="orderIdBroker">The event broker order identifier.</param>
        /// <param name="modifiedQuantity">The event order modified quantity.</param>
        /// <param name="modifiedPrice">The event order modified price.</param>
        /// <param name="modifiedTime">The event order modification accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderModified(
            AccountId accountId,
            OrderId orderId,
            OrderIdBroker orderIdBroker,
            Quantity modifiedQuantity,
            Price modifiedPrice,
            ZonedDateTime modifiedTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderModified),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(modifiedTime, nameof(modifiedTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AccountId = accountId;
            this.OrderIdBroker = orderIdBroker;
            this.ModifiedQuantity = modifiedQuantity;
            this.ModifiedPrice = modifiedPrice;
            this.ModifiedTime = modifiedTime;
        }

        /// <summary>
        /// Gets the events account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the events broker order identifier.
        /// </summary>
        public OrderIdBroker OrderIdBroker { get; }

        /// <summary>
        /// Gets the events order modified quantity.
        /// </summary>
        public Quantity ModifiedQuantity { get; }

        /// <summary>
        /// Gets the events order modified price.
        /// </summary>
        public Price ModifiedPrice { get; }

        /// <summary>
        /// Gets the events order modified accepted time.
        /// </summary>
        public ZonedDateTime ModifiedTime { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"OrderIdBroker={this.OrderIdBroker.Value}, " +
                                             $"Quantity={this.ModifiedQuantity}, " +
                                             $"Price={this.ModifiedPrice})";
    }
}
