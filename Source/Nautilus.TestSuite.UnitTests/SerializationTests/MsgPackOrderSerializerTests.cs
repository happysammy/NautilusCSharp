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
    using Nautilus.Serialization.MessageSerializers.Internal;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackOrderSerializerTests
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
            var base64 = "jKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCdXmpT3JkZXJUeXBlpk1hcmtldKhRdWFudGl0eaYxMDAwMDClUHJpY2WkTk9ORaVMYWJlbKRVMV9FrE9yZGVyUHVycG9zZaROb25lq1RpbWVJbkZvcmNlo0RBWapFeHBpcmVUaW1lpE5PTkWmSW5pdElk2gAkMWUzNzMzMDUtMWJhNy00NzUwLWFmNDYtNWQ2M2M2OWI2MzQ4qVRpbWVzdGFtcLgxOTcwLTAxLTAxIDAwOjAwOjAwLjAwMFo=";
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
            var base64 = "jKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCdXmpT3JkZXJUeXBlpUxpbWl0qFF1YW50aXR5pjEwMDAwMKVQcmljZacxLjAwMDAwpUxhYmVspVMxX1NMrE9yZGVyUHVycG9zZaROb25lq1RpbWVJbkZvcmNlo0RBWapFeHBpcmVUaW1lpE5vbmWmSW5pdElk2gAkZTgzODBiNmUtZmI3Ni00MTUxLWI4M2MtMTZhYjNhYjcxODhmqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFo=";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.Limit, order.OrderType);
        }

        [Fact]
        internal void Deserialize_GivenLimitOrderWithExpireTime_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZKhPLTEyMzQ1NqZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0J1ealPcmRlclR5cGWlTGltaXSoUXVhbnRpdHmmMTAwMDAwpVByaWNlpzEuMDAwMDClTGFiZWykTk9ORaxPcmRlclB1cnBvc2WkTm9uZatUaW1lSW5Gb3JjZaNHVESqRXhwaXJlVGltZbgxOTcwLTAxLTAxIDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkNWNmYWM0N2YtNjIyOC00MThhLWJlMDctNmRiYmEwM2QwOWY2qVRpbWVzdGFtcLgxOTcwLTAxLTAxIDAwOjAwOjAwLjAwMFo=";
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
            var base64 = "jKJJZKhPLTEyMzQ1NqZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0J1ealPcmRlclR5cGWpU3RvcExpbWl0qFF1YW50aXR5pjEwMDAwMKVQcmljZacxLjAwMDAwpUxhYmVspVMxX1NMrE9yZGVyUHVycG9zZaROb25lq1RpbWVJbkZvcmNlo0RBWapFeHBpcmVUaW1lpE5vbmWmSW5pdElk2gAkOTI5ZmY0YzYtNmU5MC00ZWIxLTk5ZWItMzBhZjEzZDdkZDZjqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFo=";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }

        [Fact]
        internal void Deserialize_GivenStopLimitOrderWithExpireTime_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZKhPLTEyMzQ1NqZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0J1ealPcmRlclR5cGWpU3RvcExpbWl0qFF1YW50aXR5pjEwMDAwMKVQcmljZacxLjAwMDAwpUxhYmVspE5PTkWsT3JkZXJQdXJwb3NlpE5vbmWrVGltZUluRm9yY2WjR1REqkV4cGlyZVRpbWW4MTk3MC0wMS0wMSAwMDowMDowMC4wMDBapkluaXRJZNoAJGM4Yjc5MWY2LWJlMDAtNDA4My04MWRhLWNjYzZiYjY0M2MzMqlUaW1lc3RhbXC4MTk3MC0wMS0wMSAwMDowMDowMC4wMDBa";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = this.serializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }
    }
}
