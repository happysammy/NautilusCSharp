// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Execution.Identifiers;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackCommandSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackCommandSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitOrderCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();

            var command = new SubmitOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(order, unpacked?.Order);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitAtomicOrderCommands_WithNoTakeProfit()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var entry = new StubOrderBuilder().BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().BuildStopMarketOrder();
            var atomicOrder = new AtomicOrder(entry, stopLoss);

            var command = new SubmitAtomicOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                atomicOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(atomicOrder, unpacked?.AtomicOrder);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitAtomicOrderCommands_WithTakeProfit()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var entry = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().StopLossOrder("O-123457").BuildStopMarketOrder();
            var takeProfit = new StubOrderBuilder().TakeProfitOrder("O-123458").BuildLimitOrder();
            var atomicOrder = new AtomicOrder(entry, stopLoss, takeProfit);

            var command = new SubmitAtomicOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                atomicOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(atomicOrder, unpacked?.AtomicOrder);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CancelOrderCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new CancelOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new OrderId("123456"),
                "EXPIRED",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as CancelOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ModifyOrderCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new ModifyOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new OrderId("123456"),
                Price.Create(1.50000m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as ModifyOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CollateralInquiryCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var command = new CollateralInquiry(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as CollateralInquiry;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

//        [Fact]
//        internal void Deserialize_CollateralInquiry_FromPythonMsgPack_ReturnsExpectedCommand()
//        {
//            // Arrange
//            var serializer = new MsgPackCommandSerializer();
//            var hexString = "hKRUeXBlp0NvbW1hbmSpQ29tbWFuZElk2gAkNmExNmVjYTMtNWVjOC00MGNjLTg0YWQtZDQyNjdkZWI0NWQ0sENvbW1hbmRUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBap0NvbW1hbmSxQ29sbGF0ZXJhbElucXVpcnk=";
//
//            var commandBytes = Convert.FromBase64String(hexString);
//
//            // Act
//            var command = serializer.Deserialize(commandBytes) as CollateralInquiry;
//
//            // Assert
//            Assert.Equal(typeof(CollateralInquiry), command?.GetType());
//        }
//
//        [Fact]
//        internal void Deserialize_GivenSubmitOrder_FromPythonMsgPack_ReturnsExpectedCommand()
//        {
//            // Arrange
//            var serializer = new MsgPackCommandSerializer();
//            var base64 =
//                "iKRUeXBlp0NvbW1hbmSpQ29tbWFuZElk2gAkY2RlZmFmYjUtY2Q5Yy00MTExLWEyN2EtZTM4ZjBjYzlhNDYzsENvbW1hbmRUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBap0NvbW1hbmSrU3VibWl0T3JkZXKoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMapQb3NpdGlvbklkpjEyMzQ1NqVPcmRlctoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJGExZmVhNGU5LTkyOGEtNDIzNy05NDM0LWQzODMyOGE4YjY1MA==";
//
//            var commandBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var command = serializer.Deserialize(commandBytes) as SubmitOrder;
//
//            // Assert
//            Assert.Equal(typeof(SubmitOrder), command?.GetType());
//        }
//
//        [Fact]
//        internal void Deserialize_GivenModifyOrder_FromPythonMsgPack_ReturnsExpectedCommand()
//        {
//            // Arrange
//            var serializer = new MsgPackCommandSerializer();
//            var base64 = "iKRUeXBlp0NvbW1hbmSpQ29tbWFuZElk2gAkZDFiM2MzYTYtY2UwYS00M2E5LTliOTctMzIwY2M1MjI1YWNisENvbW1hbmRUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBap0NvbW1hbmSrTW9kaWZ5T3JkZXKoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMadPcmRlcklkqE8tMTIzNDU2rU1vZGlmaWVkUHJpY2WnMS4wMDAwMQ==";
//
//            var commandBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var command = serializer.Deserialize(commandBytes) as ModifyOrder;
//
//            // Assert
//            Assert.Equal(typeof(ModifyOrder), command?.GetType());
//        }
//
//        [Fact]
//        internal void Deserialize_GivenCancelOrder_FromPythonMsgPack_ReturnsExpectedCommand()
//        {
//            // Arrange
//            var serializer = new MsgPackCommandSerializer();
//            var base64 = "iKRUeXBlp0NvbW1hbmSpQ29tbWFuZElk2gAkZWY5Nzg3NGMtN2YzNC00MWMxLWJhOTYtOWY2ZjM5NTQ3MmM3sENvbW1hbmRUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBap0NvbW1hbmSrQ2FuY2VsT3JkZXKoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMadPcmRlcklkqE8tMTIzNDU2rENhbmNlbFJlYXNvbqdFWFBJUkVE";
//
//            var commandBytes = Convert.FromBase64String(base64);
//
//            // Act
//            var command = serializer.Deserialize(commandBytes) as CancelOrder;
//
//            // Assert
//            Assert.Equal(typeof(CancelOrder), command?.GetType());
//        }
    }
}
