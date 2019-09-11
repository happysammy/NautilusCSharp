//--------------------------------------------------------------------------------------------------
// <copyright file="StubEventMessages.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubEventMessages
    {
        public static AccountStateEvent AccountStateEvent(string accountId = "FXCM-123456789-SIMULATED")
        {
            return new AccountStateEvent(
                AccountId.FromString(accountId),
                Currency.USD,
                Money.Create(100000, Currency.USD),
                Money.Create(100000, Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderSubmitted OrderSubmittedEvent(Order order)
        {
            return new OrderSubmitted(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderRejected OrderRejectedEvent(Order order)
        {
            return new OrderRejected(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                "some_rejected_reason",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderAccepted OrderAcceptedEvent(Order order)
        {
            return new OrderAccepted(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderWorking OrderWorkingEvent(Order order, Price? workingPrice = null)
        {
            if (workingPrice is null)
            {
                workingPrice = Price.Create(1.00000m);
            }

            return new OrderWorking(
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Symbol,
                order.Label,
                order.OrderSide,
                order.OrderType,
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
                new OrderIdBroker("B" + order.Id.Value),
                AccountId.FromString("FXCM-02851908-DEMO"),
                newPrice,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderCancelled OrderCancelledEvent(Order order)
        {
            return new OrderCancelled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderCancelReject OrderCancelRejectEvent(Order order)
        {
            return new OrderCancelReject(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                "NONE",
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderExpired OrderExpiredEvent(Order order)
        {
            return new OrderExpired(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderPartiallyFilled OrderPartiallyFilledEvent(
            Order order,
            int filledQuantity,
            int leavesQuantity,
            Price? averagePrice = null)
        {
            if (averagePrice is null)
            {
                averagePrice = Price.Create(1.00000m);
            }

            return new OrderPartiallyFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Symbol,
                order.OrderSide,
                Quantity.Create(filledQuantity),
                Quantity.Create(leavesQuantity),
                averagePrice,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderFilled OrderFilledEvent(Order order, Price? averagePrice = null)
        {
            if (averagePrice is null)
            {
                averagePrice = Price.Create(1.00000m);
            }

            return new OrderFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("NONE"),
                new ExecutionTicket("NONE"),
                order.Symbol,
                order.OrderSide,
                order.Quantity,
                averagePrice,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
