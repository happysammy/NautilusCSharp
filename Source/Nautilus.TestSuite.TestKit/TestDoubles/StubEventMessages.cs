//--------------------------------------------------------------------------------------------------
// <copyright file="StubEventMessages.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubEventMessages
    {
        public static OrderPartiallyFilled OrderPartiallyFilledEvent(
            Order order,
            int filledQuantity,
            int leavesQuantity)
        {
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            return new OrderPartiallyFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Side,
                Quantity.Create(filledQuantity),
                Quantity.Create(leavesQuantity),
                order.Price,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderFilled OrderFilledEvent(Order order)
        {
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            return new OrderFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Side,
                order.Quantity,
                order.Price,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

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

        public static OrderWorking OrderWorkingEvent(Order order)
        {
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            return new OrderWorking(
                order.Symbol,
                order.Id,
                new OrderId("some_broker_orderId"),
                order.Label,
                order.Side,
                order.Type,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

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

        public static OrderCancelled OrderCancelledEvent(Order order)
        {
            return new OrderCancelled(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

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
