// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackEventSerializerTests
    {
        private readonly ITestOutputHelper output;
        private readonly MsgPackEventSerializer serializer;

        public MsgPackEventSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.serializer = new MsgPackEventSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_AccountEvent()
        {
            // Arrange
            var accountEvent = new AccountEvent(
                Brokerage.FXCM,
                "123456",
                Currency.USD,
                Money.Create(100000, Currency.USD),
                Money.Create(100000, Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                Money.Zero(Currency.USD),
                0m,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(accountEvent);
            var unpacked = (AccountEvent)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(accountEvent, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
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
                order.Side,
                order.Type,
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderSubmittedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var submitted = new OrderSubmitted(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(submitted);
            var unpacked = (OrderSubmitted)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(submitted, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderAcceptedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var accepted = new OrderAccepted(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(accepted);
            var unpacked = (OrderAccepted)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(accepted, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderRejectedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderWorkingEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            var working = new OrderWorking(
                order.Id,
                new OrderId("B" + order.Id),
                order.Symbol,
                new Label("O123456_E"),
                order.Side,
                order.Type,
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderWorkingWithExpireTimeEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithTimeInForce(TimeInForce.GTD)
                .WithExpireTime(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1))
                .BuildStopMarketOrder();
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            var working = new OrderWorking(
                order.Id,
                new OrderId("B" + order.Id),
                order.Symbol,
                new Label("O123456_E"),
                order.Side,
                order.Type,
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderCancelledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelled = new OrderCancelled(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(cancelled);
            var unpacked = (OrderCancelled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(cancelled, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderCancelRejectEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelReject = new OrderCancelReject(
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderModifiedEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var modified = new OrderModified(
                order.Id,
                new OrderId("B" + order.Id),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(modified);
            var unpacked = (OrderModified)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(modified, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderExpiredEvents()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var expired = new OrderExpired(
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(expired);
            var unpacked = (OrderExpired)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(expired, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderPartiallyFilledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();
            var partiallyFilled = new OrderPartiallyFilled(
                order.Id,
                new ExecutionId("E123456"),
                new ExecutionTicket("P123456"),
                order.Symbol,
                order.Side,
                Quantity.Create(order.Quantity / 2),
                Quantity.Create(order.Quantity / 2),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(partiallyFilled);
            var unpacked = (OrderPartiallyFilled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(partiallyFilled, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_OrderFilledEvents()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();
            var filled = new OrderFilled(
                order.Id,
                new ExecutionId("E123456"),
                new ExecutionTicket("P123456"),
                order.Symbol,
                order.Side,
                order.Quantity,
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(filled);
            var unpacked = (OrderFilled)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(filled, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }
    }
}
