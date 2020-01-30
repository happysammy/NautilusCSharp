// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
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
        private readonly OrderSerializer serializer;

        public MsgPackOrderSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.serializer = new OrderSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_MarketOrders()
        {
            // Arrange
            var order = new StubOrderBuilder()
                .WithTimestamp(StubZonedDateTime.UnixEpoch() + Duration.FromDays(1))
                .BuildMarketOrder();

            // Act
            var packed = this.serializer.Serialize(order);
            var unpacked = this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            Assert.Equal(order.Symbol, unpacked.Symbol);
            Assert.Equal(order.OrderType, unpacked.OrderType);
            Assert.Equal(order.Price, unpacked.Price);
            Assert.Equal(order.ExecutionId, unpacked.ExecutionId);
            Assert.Equal(order.Label, unpacked.Label);
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_LimitOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = this.serializer.Serialize(order);
            var unpacked = this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            Assert.Equal(order.Symbol, unpacked.Symbol);
            Assert.Equal(order.OrderType, unpacked.OrderType);
            Assert.Equal(order.Price, unpacked.Price);
            Assert.Equal(order.ExecutionId, unpacked.ExecutionId);
            Assert.Equal(order.Label, unpacked.Label);
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopMarketOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var packed = this.serializer.Serialize(order);
            var unpacked = this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            Assert.Equal(order.Symbol, unpacked.Symbol);
            Assert.Equal(order.OrderType, unpacked.OrderType);
            Assert.Equal(order.Price, unpacked.Price);
            Assert.Equal(order.ExecutionId, unpacked.ExecutionId);
            Assert.Equal(order.Label, unpacked.Label);
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_StopLimitOrders()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = this.serializer.Serialize(order);
            var unpacked = this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            Assert.Equal(order.Symbol, unpacked.Symbol);
            Assert.Equal(order.OrderType, unpacked.OrderType);
            Assert.Equal(order.Price, unpacked.Price);
            Assert.Equal(order.ExecutionId, unpacked.ExecutionId);
            Assert.Equal(order.Label, unpacked.Label);
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_NullableOrders_GivenOrder()
        {
            // Arrange
            var order = new StubOrderBuilder().TakeProfitOrder("O-125").BuildLimitOrder();

            // Act
            var packed = this.serializer.SerializeNullable(order);
            var unpacked = this.serializer.DeserializeNullable(packed);

            // Assert
            if (unpacked is null)
            {
                Assert.True(false);
                return; // Avoid potential null dereference warning
            }

            Assert.Equal(order, unpacked);
            Assert.Equal(order.Symbol, unpacked.Symbol);
            Assert.Equal(order.OrderType, unpacked.OrderType);
            Assert.Equal(order.Price, unpacked.Price);
            Assert.Equal(order.ExecutionId, unpacked.ExecutionId);
            Assert.Equal(order.Label, unpacked.Label);
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_NullableOrders_GivenNil()
        {
            // Arrange
            // Act
            var packed = this.serializer.SerializeNullable(null);
            var unpacked = this.serializer.DeserializeNullable(packed);

            // Assert
            Assert.Null(unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void Deserialize_GivenMarketOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlpk1BUktFVKhRdWFudGl0ec4AAYagpVByaWNlpE5PTkWlTGFiZWykVTFfRaxPcmRlclB1cnBvc2WkTk9ORatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkM2IyNmI4ZjMtYmJiYy00Y2MxLWI3YjUtZjY2NWVhM2M2NzIz";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.Market, order.OrderType);
        }

        [Fact]
        internal void Deserialize_GivenLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqClUHJpY2WnMS4wMDAwMKVMYWJlbKVTMV9TTKxPcmRlclB1cnBvc2WkTk9ORatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkNjk0YzVmOTMtZTllMC00N2ZiLWExMjItNzI2Y2FmOTFjYjA0";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.Limit, order.OrderType);
        }

        [Fact]
        internal void Deserialize_GivenStopLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZKhPLTEyMzQ1NqZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0JVWalPcmRlclR5cGWqU1RPUF9MSU1JVKhRdWFudGl0ec4AAYagpVByaWNlpzEuMDAwMDClTGFiZWylUzFfU0ysT3JkZXJQdXJwb3NlpE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDU3ZGJlOGNlLWExM2UtNDI2NS04NDFlLTAwN2VjOGZmNDg3Mg==";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }
    }
}
