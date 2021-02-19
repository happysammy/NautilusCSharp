// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Text;
using Nautilus.DomainModel.Enums;
using Nautilus.Serialization.MessageSerializers.Internal;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    // Required warning suppression for tests
    // (do not remove even if compiler doesn't initially complain).
#pragma warning disable 8602
#pragma warning disable 8604
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackOrderSerializerTests : TestBase
    {
        private readonly MsgPackOrderSerializer serializer;

        public MsgPackOrderSerializerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.serializer = new MsgPackOrderSerializer();
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
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(order.Quantity, unpacked.Quantity);
            Assert.Equal(order.ExpireTime, unpacked.ExpireTime);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void Deserialize_GivenMarketOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "jKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaNCdXmpT3JkZXJUeXBlpk1hcmtldKhRdWFudGl0eaYxMDAwMDClUHJpY2WkTm9uZaVMYWJlbKRVMV9FrE9yZGVyUHVycG9zZaROb25lq1RpbWVJbkZvcmNlo0RBWapFeHBpcmVUaW1lpE5vbmWmSW5pdElk2gAkZjhlMzIxNzItM2JjNi00ZGJjLTlkYjEtN2JhMDI2M2ZlMjc1qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFo=";
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
