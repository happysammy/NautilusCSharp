// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
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
        internal void Test_can_serialize_and_deserialize_market_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_limit_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_market_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_stop_limit_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Hex.ToHexString(packed));
        }

        [Fact]
        internal void Test_can_deserialize_market_order_from_python_msgpack()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var hexString = "8aa673796d626f6cab4155445553442e4658434da86f726465725f6964a74f313233" +
                            "343536a56c6162656cac5343414c50455230315f534caa6f726465725f73696465a3" +
                            "425559aa6f726465725f74797065a64d41524b4554a87175616e74697479ce000186" +
                            "a0a974696d657374616d70b8313937302d30312d30315430303a30303a30302e3030" +
                            "305aa57072696365a44e4f4e45ad74696d655f696e5f666f726365a3444159ab6578" +
                            "706972655f74696d65a44e4f4e45";

            var orderBytes = Hex.FromHexString(hexString);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.MARKET, order.Type);
        }

        [Fact]
        internal void Test_can_deserialize_limit_order_from_python_msgpack()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var hexString = "8aa673796d626f6cab4155445553442e4658434da86f726465725f6964a74f313233" +
                            "343536a56c6162656cac5343414c50455230315f534caa6f726465725f73696465a3" +
                            "425559aa6f726465725f74797065a54c494d4954a87175616e74697479ce000186a0" +
                            "a974696d657374616d70b8313937302d30312d30315430303a30303a30302e303030" +
                            "5aa57072696365a7312e3030303030ad74696d655f696e5f666f726365a3444159ab" +
                            "6578706972655f74696d65a44e4f4e45";

            var orderBytes = Hex.FromHexString(hexString);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.LIMIT, order.Type);
        }

        [Fact]
        internal void Test_can_deserialize_stop_limit_order_from_python_msgpack()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var hexString = "8aa673796d626f6cab4155445553442e4658434da86f726465725f6964a74f313233" +
                            "343536a56c6162656cac5343414c50455230315f534caa6f726465725f73696465a3" +
                            "425559aa6f726465725f74797065aa53544f505f4c494d4954a87175616e74697479" +
                            "ce000186a0a974696d657374616d70b8313937302d30312d30315430303a30303a30" +
                            "302e3030305aa57072696365a7312e3030303030ad74696d655f696e5f666f726365" +
                            "a3444159ab6578706972655f74696d65a44e4f4e45";

            var orderBytes = Hex.FromHexString(hexString);

            // Act
            var order = serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.STOP_LIMIT, order.Type);
        }
    }
}
