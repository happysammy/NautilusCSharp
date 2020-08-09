// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Serialization.MessageSerializers;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    // Required warning suppression for tests
    // (do not remove even if compiler doesn't initially complain).
#pragma warning disable 8604
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackEventSerializerTests : TestBase
    {
        private readonly MsgPackEventSerializer serializer;

        public MsgPackEventSerializerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.serializer = new MsgPackEventSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_AccountStateEvent()
        {
            // Arrange
            var accountEvent = new AccountStateEvent(
                new AccountId("FXCM", "D123456", "SIMULATED"),
                Currency.USD,
                Money.Create(100000, Currency.USD),
                Money.Create(100000, Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                0m,
                "N",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(accountEvent);
            var unpacked = (AccountStateEvent)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal("FXCM-D123456-SIMULATED", accountEvent.AccountId.Value);
            Assert.Equal(accountEvent, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderInitializedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var initialized = new OrderInitialized(
                order.Id,
                order.Symbol,
                order.Label,
                order.OrderSide,
                order.OrderType,
                order.OrderPurpose,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                order.ExpireTime,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(initialized);
            var unpacked = (OrderInitialized)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(initialized, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderInvalidEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var invalid = new OrderInvalid(
                order.Id,
                "OrderId already exists.",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(invalid);
            var unpacked = (OrderInvalid)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(invalid, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderDeniedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var denied = new OrderDenied(
                order.Id,
                "Exceeds risk for FX.",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(denied);
            var unpacked = (OrderDenied)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(denied, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderSubmittedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var submitted = new OrderSubmitted(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(submitted);
            var unpacked = (OrderSubmitted)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(submitted, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderAcceptedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var accepted = new OrderAccepted(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Label,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(accepted);
            var unpacked = (OrderAccepted)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(accepted, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderRejectedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(rejected);
            var unpacked = (OrderRejected)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(rejected, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderWorkingEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var working = new OrderWorking(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Symbol,
                new Label("E"),
                order.OrderSide,
                order.OrderType,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(working);
            var unpacked = (OrderWorking)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(working, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderWorkingWithExpireTimeEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithTimeInForce(TimeInForce.GTD)
                .WithExpireTime(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1))
                .BuildStopMarketOrder();

            var working = new OrderWorking(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Symbol,
                new Label("E"),
                order.OrderSide,
                order.OrderType,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(working);
            var unpacked = (OrderWorking)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(working, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderCancelledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelled = new OrderCancelled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(cancelled);
            var unpacked = (OrderCancelled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(cancelled, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderCancelRejectEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelReject = new OrderCancelReject(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "REJECT_RESPONSE?",
                "ORDER_NOT_FOUND",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(cancelReject);
            var unpacked = (OrderCancelReject)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(cancelReject, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderModifiedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var modified = new OrderModified(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new OrderIdBroker("B" + order.Id.Value),
                order.Quantity,
                Price.Create(2m, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(modified);
            var unpacked = (OrderModified)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(modified, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderExpiredEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var expired = new OrderExpired(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(expired);
            var unpacked = (OrderExpired)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(expired, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderPartiallyFilledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();
            var partiallyFilled = new OrderPartiallyFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new ExecutionId("E123456"),
                new PositionIdBroker("P123456"),
                order.Symbol,
                order.OrderSide,
                Quantity.Create(order.Quantity / 2),
                Quantity.Create(order.Quantity / 2),
                Price.Create(2m, 1),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(partiallyFilled);
            var unpacked = (OrderPartiallyFilled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(partiallyFilled, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderFilledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();

            var filled = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                order.Id,
                new ExecutionId("E123456"),
                new PositionIdBroker("P123456"),
                order.Symbol,
                order.OrderSide,
                order.Quantity,
                Price.Create(2m, 1),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(filled);
            var unpacked = (OrderFilled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(filled, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
        }
    }
}
