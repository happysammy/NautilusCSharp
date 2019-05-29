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
        public static OrderSubmitted OrderSubmittedEvent(Order order)
        {
            return new OrderSubmitted(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderRejected OrderRejectedEvent(Order order)
        {
            return new OrderRejected(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "some_rejected_reason",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderAccepted OrderAcceptedEvent(Order order)
        {
            return new OrderAccepted(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderWorking OrderWorkingEvent(Order order, Price workingPrice)
        {
            return new OrderWorking(
                order.Id,
                new OrderId("B" + order.Id),
                order.Symbol,
                order.Label,
                order.Side,
                order.Type,
                order.Quantity,
                workingPrice,
                order.TimeInForce,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderModified OrderModifiedEvent(Order order, Price newPrice)
        {
            return new OrderModified(
                order.Id,
                new OrderId("B" + order.Id),
                newPrice,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderCancelled OrderCancelledEvent(Order order)
        {
            return new OrderCancelled(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderExpired OrderExpiredEvent(Order order)
        {
            return new OrderExpired(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderPartiallyFilled OrderPartiallyFilledEvent(
            Order order,
            int filledQuantity,
            int leavesQuantity,
            Price averagePrice)
        {
            return new OrderPartiallyFilled(
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Symbol,
                order.Side,
                Quantity.Create(filledQuantity),
                Quantity.Create(leavesQuantity),
                averagePrice,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderFilled OrderFilledEvent(Order order, Price averagePrice)
        {
            return new OrderFilled(
                order.Id,
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Symbol,
                order.Side,
                order.Quantity,
                averagePrice,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
