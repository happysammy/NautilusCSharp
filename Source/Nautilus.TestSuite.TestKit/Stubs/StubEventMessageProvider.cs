//--------------------------------------------------------------------------------------------------
// <copyright file="StubEventMessageProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Stubs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubEventMessageProvider
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
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderRejected OrderRejectedEvent(Order order)
        {
            return new OrderRejected(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "some_rejected_reason",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderAccepted OrderAcceptedEvent(Order order)
        {
            return new OrderAccepted(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Label,
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
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
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
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Quantity,
                newPrice,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderCancelled OrderCancelledEvent(Order order)
        {
            return new OrderCancelled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderCancelReject OrderCancelRejectEvent(Order order)
        {
            return new OrderCancelReject(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "None",
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderExpired OrderExpiredEvent(Order order)
        {
            return new OrderExpired(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        public static OrderPartiallyFilled OrderPartiallyFilledEvent(
            Order order,
            Quantity filledQuantity,
            Quantity leavesQuantity,
            Price? averagePrice = null)
        {
            if (averagePrice is null)
            {
                averagePrice = Price.Create(1.00000m);
            }

            return new OrderPartiallyFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new ExecutionId("None"),
                new PositionIdBroker("None"),
                order.Symbol,
                order.OrderSide,
                filledQuantity,
                leavesQuantity,
                averagePrice,
                Currency.USD,
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
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new ExecutionId("None"),
                new PositionIdBroker("None"),
                order.Symbol,
                order.OrderSide,
                order.Quantity,
                averagePrice,
                Currency.USD,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
