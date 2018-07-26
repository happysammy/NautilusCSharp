//--------------------------------------------------------------------------------------------------
// <copyright file="StubEventMessages.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The stub event messages.
    /// </summary>
    public static class StubEventMessages
    {
        /// <summary>
        /// The order filled event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="OrderFilled"/>.
        /// </returns>
        public static OrderFilled OrderFilledEvent(Order order)
        {
            return new OrderFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionId("NONE"),
                order.Side,
                order.Quantity,
                order.Price.Value,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order partially filled event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="filledQuantity">
        /// The filled quantity.
        /// </param>
        /// <param name="leavesQuantity">
        /// The leaves quantity.
        /// </param>
        /// <returns>
        /// The <see cref="OrderPartiallyFilled"/>.
        /// </returns>
        public static OrderPartiallyFilled OrderPartiallyFilledEvent(
            Order order,
            int filledQuantity,
            int leavesQuantity)
        {
            return new OrderPartiallyFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionId("NONE"),
                order.Side,
                Quantity.Create(filledQuantity),
                Quantity.Create(leavesQuantity),
                order.Price.Value,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order rejected event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="OrderRejected"/>.
        /// </returns>
        public static OrderRejected OrderRejectedEvent(Order order)
        {
            return new OrderRejected(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "some_rejected_reason",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order working event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="OrderWorking"/>.
        /// </returns>
        public static OrderWorking OrderWorkingEvent(Order order)
        {
            return new OrderWorking(
                order.Symbol,
                order.Id,
                new OrderId("some_broker_orderId"),
                order.Label,
                order.Side,
                order.Type,
                order.Quantity,
                order.Price.Value,
                order.TimeInForce,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order modified event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="newPrice">
        /// The new price.
        /// </param>
        /// <returns>
        /// The <see cref="OrderModified"/>.
        /// </returns>
        public static OrderModified OrderModifiedEvent(Order order, Price newPrice)
        {
            return new OrderModified(
                order.Symbol,
                order.Id,
                new OrderId("NONE"),
                newPrice,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order cancelled event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="OrderCancelled"/>.
        /// </returns>
        public static OrderCancelled OrderCancelledEvent(Order order)
        {
            return new OrderCancelled(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The order expired event.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="OrderExpired"/>.
        /// </returns>
        public static OrderExpired OrderExpiredEvent(Order order)
        {
            return new OrderExpired(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
