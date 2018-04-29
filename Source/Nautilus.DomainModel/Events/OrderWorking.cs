//--------------------------------------------------------------------------------------------------
// <copyright file="OrderWorking.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="OrderWorking"/> class. Represents an event where an order
    /// had been working in the market at the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderWorking : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderWorking"/> class.
        /// </summary>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="brokerOrderId">The event broker order identifier.</param>
        /// <param name="label">The event order label. </param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="orderType">The event order type.</param>
        /// <param name="quantity">The event order quantity.</param>
        /// <param name="price">The event order price.</param>
        /// <param name="timeInForce">The event order time in force.</param>
        /// <param name="expireTime">The event order expire time.</param>
        /// <param name="workingTime">The event order accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public OrderWorking(
            Symbol symbol,
            EntityId orderId,
            EntityId brokerOrderId,
            Label label,
            OrderSide orderSide,
            OrderType orderType,
            Quantity quantity,
            Price price,
            TimeInForce timeInForce,
            Option<ZonedDateTime?> expireTime,
            ZonedDateTime workingTime,
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
            Validate.NotNull(label, nameof(label));
            Validate.NotDefault(orderSide, nameof(orderSide));
            Validate.NotDefault(orderType, nameof(orderType));
            Validate.NotNull(quantity, nameof(quantity));
            Validate.NotNull(price, nameof(price));
            Validate.NotDefault(timeInForce, nameof(timeInForce));
            Validate.NotNull(expireTime, nameof(expireTime));
            Validate.NotDefault(workingTime, nameof(workingTime));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.BrokerOrderId = brokerOrderId;
            this.Label = label;
            this.OrderSide = orderSide;
            this.OrderType = orderType;
            this.Quantity = quantity;
            this.Price = price;
            this.TimeInForce = timeInForce;
            this.ExpireTime = expireTime;
            this.WorkingTime = workingTime;
        }

        /// <summary>
        /// Gets the events broker order identifier.
        /// </summary>
        public EntityId BrokerOrderId { get; }

        /// <summary>
        /// Gets the events order label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the events order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the events order type.
        /// </summary>
        public OrderType OrderType { get; }

        /// <summary>
        /// Gets the events order quantity.
        /// </summary>
        public Quantity Quantity { get; }

        /// <summary>
        /// Gets the events order price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the events order time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; }

        /// <summary>
        /// Gets the events order expire time (optional).
        /// </summary>
        public Option<ZonedDateTime?> ExpireTime { get; }

        /// <summary>
        /// Gets the events order working time.
        /// </summary>
        public ZonedDateTime WorkingTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="OrderWorking"/> event.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(OrderWorking);
    }
}