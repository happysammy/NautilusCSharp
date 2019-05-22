// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackOrderSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackOrderSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void CanSerializeAndDeserialize_MarketOrders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_LimitOrders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopMarketOrders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopLimitOrders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void Deserialize_GivenMarketOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var base64 = "iqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGpT3JkZXJTaWRlo0JVWalPcmRlclR5cGWmTUFSS0VUqFF1YW50aXR5zgABhqCpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqVQcmljZaROT05FpUxhYmVspFUxX0WrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORQ==";

            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.MARKET, order.Type);
        }

        [Fact]
        internal void Deserialize_GivenLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var hexString = "8aa653796d626f6cab4155445553442e4658434da74f726465724964bb4f2d31393730303130312d3030303030302d3030312d3030312d31a94f7264657253696465a3425559a94f7264657254797065a54c494d4954a85175616e74697479ce000186a0a954696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aa55072696365a7312e3030303030a54c6162656ca553315f534cab54696d65496e466f726365a3444159aa45787069726554696d65a44e4f4e45";

            var orderBytes = Convert.FromBase64String(hexString);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.LIMIT, order.Type);
        }

        [Fact]
        internal void Deserialize_GivenStopLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var hexString = "8aa653796d626f6cab4155445553442e4658434da74f726465724964a74f313233343536a94f7264657253696465a3425559a94f7264657254797065aa53544f505f4c494d4954a85175616e74697479ce000186a0a954696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aa55072696365a7312e3030303030a54c6162656ca553315f534cab54696d65496e466f726365a3444159aa45787069726554696d65a44e4f4e45";

            var orderBytes = Convert.FromBase64String(hexString);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.STOP_LIMIT, order.Type);
        }
    }
}
