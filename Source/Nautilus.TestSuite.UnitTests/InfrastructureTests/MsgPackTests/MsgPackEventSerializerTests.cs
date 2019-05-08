// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackEventSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackEventSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_submitted_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var submitted = new OrderSubmitted(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(submitted);
            var unpacked = (OrderSubmitted)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(submitted, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_accepted_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var accepted = new OrderAccepted(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(accepted);
            var unpacked = (OrderAccepted)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(accepted, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_rejected_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(rejected);
            var unpacked = (OrderRejected)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(rejected, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_order_working_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var working = new OrderWorking(
                order.Symbol,
                order.Id,
                new OrderId("B123456"),
                new Label("O123456_E"),
                order.Side,
                order.Type,
                order.Quantity,
                order.Price.Value,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(working);
            var unpacked = (OrderWorking)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(working, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_order_working_with_expire_time_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder()
                .WithTimeInForce(TimeInForce.GTD)
                .WithExpireTime(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1))
                .BuildStopMarketOrder();
            var working = new OrderWorking(
                order.Symbol,
                order.Id,
                new OrderId("B123456"),
                new Label("O123456_E"),
                order.Side,
                order.Type,
                order.Quantity,
                order.Price.Value,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(working);
            var unpacked = (OrderWorking)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(working, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_cancelled_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelled = new OrderCancelled(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(cancelled);
            var unpacked = (OrderCancelled)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(cancelled, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_cancel_reject_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelReject = new OrderCancelReject(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "REJECT_RESPONSE?",
                "ORDER_NOT_FOUND",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(cancelReject);
            var unpacked = (OrderCancelReject)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(cancelReject, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_modified_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var modified = new OrderModified(
                order.Symbol,
                order.Id,
                new OrderId("B123456"),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(modified);
            var unpacked = (OrderModified)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(modified, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_order_expired_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var expired = new OrderExpired(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(expired);
            var unpacked = (OrderExpired)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(expired, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_partially_filled_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();
            var partiallyFilled = new OrderPartiallyFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("E123456"),
                new ExecutionId("P123456"),
                order.Side,
                Quantity.Create(order.Quantity / 2),
                Quantity.Create(order.Quantity / 2),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(partiallyFilled);
            var unpacked = (OrderPartiallyFilled)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(partiallyFilled, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_filled_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder()
                .WithQuantity(Quantity.Create(100000))
                .BuildStopLimitOrder();
            var filled = new OrderFilled(
                order.Symbol,
                order.Id,
                new ExecutionId("E123456"),
                new ExecutionId("P123456"),
                order.Side,
                order.Quantity,
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(filled);
            var unpacked = (OrderFilled)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(filled, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_account_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var accountEvent = new AccountEvent(
                EntityIdFactory.Account(Brokerage.FXCM, "123456"),
                Brokerage.FXCM,
                "123456",
                CurrencyCode.USD,
                Money.Create(100000, CurrencyCode.USD),
                Money.Create(100000, CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                0m,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(accountEvent);
            var unpacked = (AccountEvent)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(accountEvent, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }
    }
}
