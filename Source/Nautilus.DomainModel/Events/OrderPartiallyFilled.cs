//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPartiallyFilled.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order has been partially filled.
    /// </summary>
    [Immutable]
    public sealed class OrderPartiallyFilled : OrderFillEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPartiallyFilled"/> class.
        /// </summary>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="positionIdBroker">The event order execution ticket.</param>
        /// <param name="symbol">The event order symbol.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="leavesQuantity">The event leaves quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="currency">The event order transaction currency.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderPartiallyFilled(
            AccountId accountId,
            OrderId orderId,
            ExecutionId executionId,
            PositionIdBroker positionIdBroker,
            Symbol symbol,
            OrderSide orderSide,
            Quantity filledQuantity,
            Quantity leavesQuantity,
            Price averagePrice,
            Currency currency,
            ZonedDateTime executionTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                accountId,
                orderId,
                executionId,
                positionIdBroker,
                symbol,
                orderSide,
                filledQuantity,
                averagePrice,
                currency,
                executionTime,
                typeof(OrderPartiallyFilled),
                eventId,
                eventTimestamp)
        {
            this.LeavesQuantity = leavesQuantity;
        }

        /// <summary>
        /// Gets the events leaves quantity.
        /// </summary>
        public Quantity LeavesQuantity { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"ExecutionId={this.ExecutionId.Value}, " +
                                             $"PositionIdBroker={this.PositionIdBroker.Value}, " +
                                             $"{this.OrderSide} " +
                                             $"{this.FilledQuantity.ToStringFormatted()} " +
                                             $"{this.Symbol.Value} @ " +
                                             $"{this.AveragePrice} {this.Currency}, " +
                                             $"LeavesQty={this.LeavesQuantity.ToStringFormatted()})";
    }
}
