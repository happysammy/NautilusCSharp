// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(submitted);
            var unpacked = serializer.DeserializeEvent(packed) as OrderSubmitted;

            // Assert
            Assert.Equal(submitted, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_accepted_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var accepted = new OrderAccepted(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(accepted);
            var unpacked = serializer.DeserializeEvent(packed) as OrderAccepted;

            // Assert
            Assert.Equal(accepted, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_rejected_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(rejected);
            var unpacked = serializer.DeserializeEvent(packed) as OrderRejected;

            // Assert
            Assert.Equal(rejected, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_order_working_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var working = new OrderWorking(
                order.Symbol,
                order.OrderId,
                new EntityId("B123456"),
                new Label("O123456_E"),
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
            var packed = serializer.SerializeEvent(working);
            var unpacked = serializer.DeserializeEvent(packed) as OrderWorking;

            // Assert
            Assert.Equal(working, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
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
                order.OrderId,
                new EntityId("B123456"),
                new Label("O123456_E"),
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
            var packed = serializer.SerializeEvent(working);
            var unpacked = serializer.DeserializeEvent(packed) as OrderWorking;

            // Assert
            Assert.Equal(working, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_cancelled_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelled = new OrderCancelled(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(cancelled);
            var unpacked = serializer.DeserializeEvent(packed) as OrderCancelled;

            // Assert
            Assert.Equal(cancelled, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_cancel_reject_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var cancelReject = new OrderCancelReject(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                "REJECT_RESPONSE?",
                "ORDER_NOT_FOUND",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(cancelReject);
            var unpacked = serializer.DeserializeEvent(packed) as OrderCancelReject;

            // Assert
            Assert.Equal(cancelReject, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_order_modified_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();
            var modified = new OrderModified(
                order.Symbol,
                order.OrderId,
                new EntityId("B123456"),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(modified);
            var unpacked = serializer.DeserializeEvent(packed) as OrderModified;

            // Assert
            Assert.Equal(modified, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_order_expired_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var expired = new OrderExpired(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(expired);
            var unpacked = serializer.DeserializeEvent(packed) as OrderExpired;

            // Assert
            Assert.Equal(expired, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
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
                order.OrderId,
                new EntityId("E123456"),
                new EntityId("P123456"),
                order.OrderSide,
                Quantity.Create(order.Quantity / 2),
                Quantity.Create(order.Quantity / 2),
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(partiallyFilled);
            var unpacked = serializer.DeserializeEvent(packed) as OrderPartiallyFilled;

            // Assert
            Assert.Equal(partiallyFilled, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
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
                order.OrderId,
                new EntityId("E123456"),
                new EntityId("P123456"),
                order.OrderSide,
                order.Quantity,
                Price.Create(2, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeEvent(filled);
            var unpacked = serializer.DeserializeEvent(packed) as OrderFilled;

            // Assert
            Assert.Equal(filled, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }
    }
}
