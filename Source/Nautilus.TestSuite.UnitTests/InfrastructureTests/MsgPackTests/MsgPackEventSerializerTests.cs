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
    using Nautilus.DomainModel.Events;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
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
            var order = new StubOrderBuilder().BuildMarket();
            var submitted = new OrderSubmitted(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeOrderEvent(submitted);
            var unpacked = serializer.DeserializeOrderEvent(packed) as OrderSubmitted;

            // Assert
            Assert.Equal(submitted, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_accepted_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarket();
            var accepted = new OrderAccepted(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeOrderEvent(accepted);
            var unpacked = serializer.DeserializeOrderEvent(packed) as OrderAccepted;

            // Assert
            Assert.Equal(accepted, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_order_rejected_event()
        {
            // Arrange
            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarket();
            var rejected = new OrderRejected(
                order.Symbol,
                order.OrderId,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.SerializeOrderEvent(rejected);
            var unpacked = serializer.DeserializeOrderEvent(packed) as OrderRejected;

            // Assert
            Assert.Equal(rejected, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }
    }
}
