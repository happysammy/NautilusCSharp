// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Serialization.Internal;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
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
            var order = new StubOrderBuilder()
                .WithTimestamp(StubZonedDateTime.UnixEpoch() + Duration.FromDays(1))
                .BuildMarketOrder();

            // Act
            var packed = OrderSerializer.Serialize(order);
            var unpacked = OrderSerializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_LimitOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = OrderSerializer.Serialize(order);
            var unpacked = OrderSerializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopMarketOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var packed = OrderSerializer.Serialize(order);
            var unpacked = OrderSerializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopLimitOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = OrderSerializer.Serialize(order);
            var unpacked = OrderSerializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_NullableOrders_GivenOrder()
        {
            // Arrange
            var takeProfit = new StubOrderBuilder().TakeProfitOrder("O-125").BuildLimitOrder();

            // Act
            var packed = OrderSerializer.SerializeNullable(takeProfit);
            var unpacked = OrderSerializer.DeserializeNullable(packed);

            // Assert
            Assert.Equal(takeProfit, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_NullableOrders_GivenNil()
        {
            // Arrange
            // Act
            var packed = OrderSerializer.SerializeNullable(null);
            var unpacked = OrderSerializer.DeserializeNullable(packed);

            // Assert
            Assert.Null(unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

//        [Fact]
//        internal void Deserialize_GivenMarketOrder_FromPythonMsgPack_ReturnsExpectedOrder()
//        {
//            // Arrange
//            var base64 = "i6JJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlpk1BUktFVKhRdWFudGl0ec4AAYagpVByaWNlpE5PTkWlTGFiZWykVTFfRatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkYjdhOWRkN2UtMmFkOC00ZDYzLTg1ZGItNGUzMzMwM2U3ODA2";
//            var orderBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var order = OrderSerializer.Deserialize(orderBytes);
//
//            // Assert
//            Assert.Equal(OrderType.MARKET, order.Type);
//        }
//
//        [Fact]
//        internal void Deserialize_GivenLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
//        {
//            // Arrange
//            var base64 = "i6JJZKdPMTIzNDU2plN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaVMSU1JVKhRdWFudGl0ec4AAYagpVByaWNlpzEuMDAwMDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNHVESqRXhwaXJlVGltZbgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqZJbml0SWTaACQ1YzRiZTllNC0yYTQxLTRjZjQtYmRkMS04OTE0ODM0Mjc5ZDg=";
//            var orderBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var order = OrderSerializer.Deserialize(orderBytes);
//
//            // Assert
//            Assert.Equal(OrderType.LIMIT, order.Type);
//        }
//
//        [Fact]
//        internal void Deserialize_GivenStopLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
//        {
//            // Arrange
//            var base64 = "i6JJZKdPMTIzNDU2plN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZapTVE9QX0xJTUlUqFF1YW50aXR5zgABhqClUHJpY2WnMS4wMDAwMKVMYWJlbKVTMV9TTKtUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkNTA5ZmUxNTctNjIyNi00N2MyLWEzOGYtNjQ4MzFkNjlkMmIy";
//            var orderBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var order = OrderSerializer.Deserialize(orderBytes);
//
//            // Assert
//            Assert.Equal(OrderType.STOP_LIMIT, order.Type);
//        }
    }
}
