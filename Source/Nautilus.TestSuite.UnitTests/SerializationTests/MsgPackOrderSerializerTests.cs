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
    using Nautilus.Serialization;
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
            var packed = MsgPackOrderSerializer.Serialize(order);
            var unpacked = MsgPackOrderSerializer.Deserialize(packed);

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
            var packed = MsgPackOrderSerializer.Serialize(order);
            var unpacked = MsgPackOrderSerializer.Deserialize(packed);

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
            var packed = MsgPackOrderSerializer.Serialize(order);
            var unpacked = MsgPackOrderSerializer.Deserialize(packed);

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
            var packed = MsgPackOrderSerializer.Serialize(order);
            var unpacked = MsgPackOrderSerializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void Deserialize_GivenMarketOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "iqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGpT3JkZXJTaWRlo0JVWalPcmRlclR5cGWmTUFSS0VUqFF1YW50aXR5zgABhqCpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqVQcmljZaROT05FpUxhYmVspFUxX0WrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORQ==";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = MsgPackOrderSerializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.MARKET, order.Type);
        }

        [Fact]
        internal void Deserialize_GivenLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "iqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGpT3JkZXJTaWRlo0JVWalPcmRlclR5cGWlTElNSVSoUXVhbnRpdHnOAAGGoKlUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapVByaWNlpzEuMDAwMDClTGFiZWylUzFfU0yrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORQ==";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = MsgPackOrderSerializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.LIMIT, order.Type);
        }

        [Fact]
        internal void Deserialize_GivenStopLimitOrder_FromPythonMsgPack_ReturnsExpectedOrder()
        {
            // Arrange
            var base64 = "iqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZKdPMTIzNDU2qU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlqlNUT1BfTElNSVSoUXVhbnRpdHnOAAGGoKlUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapVByaWNlpzEuMDAwMDClTGFiZWylUzFfU0yrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORQ==";
            var orderBytes = Convert.FromBase64String(base64);

            // Act
            var order = MsgPackOrderSerializer.Deserialize(orderBytes);

            // Assert
            Assert.Equal(OrderType.STOP_LIMIT, order.Type);
        }
    }
}
