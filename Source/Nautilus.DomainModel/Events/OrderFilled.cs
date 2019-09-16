//--------------------------------------------------------------------------------------------------
// <copyright file="OrderFilled.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order has been completely filled.
    /// </summary>
    [Immutable]
    public sealed class OrderFilled : OrderFillEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFilled"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="accountId">The event account identifier.</param>
        /// <param name="executionId">The event order execution identifier.</param>
        /// <param name="executionTicket">The event order execution ticket.</param>
        /// <param name="symbol">The event order symbol.</param>
        /// <param name="orderSide">The event order side.</param>
        /// <param name="filledQuantity">The event order filled quantity.</param>
        /// <param name="averagePrice">The event order average price.</param>
        /// <param name="executionTime">The event order execution time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderFilled(
            OrderId orderId,
            AccountId accountId,
            ExecutionId executionId,
            ExecutionTicket executionTicket,
            Symbol symbol,
            OrderSide orderSide,
            Quantity filledQuantity,
            Price averagePrice,
            ZonedDateTime executionTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                accountId,
                executionId,
                executionTicket,
                symbol,
                orderSide,
                filledQuantity,
                averagePrice,
                executionTime,
                typeof(OrderFilled),
                eventId,
                eventTimestamp)
        {
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"ExecutionId={this.ExecutionId.Value}," +
                                             $"ExecutionTicket={this.ExecutionTicket}, " +
                                             $"{this.OrderSide} {this.FilledQuantity:N0} {this.Symbol.Value} @ {this.AveragePrice})";
    }
}
