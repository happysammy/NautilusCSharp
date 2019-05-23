// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Execution.Identifiers;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.MsgPack;
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
                order,
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(order, unpacked?.Order);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitAtomicOrderCommands_WithNoTakeProfit()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var entry = new StubOrderBuilder().BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().BuildStopMarketOrder();
            var takeProfit = OptionRef<Order>.None();
            var atomicOrder = new AtomicOrder(entry, stopLoss, takeProfit);

            var command = new SubmitAtomicOrder(
                atomicOrder,
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(atomicOrder, unpacked?.AtomicOrder);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitAtomicOrderCommands_WithTakeProfit()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var entry = new StubOrderBuilder().BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().BuildStopMarketOrder();
            var takeProfit = OptionRef<Order>.None();
            var atomicOrder = new AtomicOrder(entry, stopLoss, takeProfit);

            var command = new SubmitAtomicOrder(
                atomicOrder,
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(atomicOrder, unpacked?.AtomicOrder);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CancelOrderCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new CancelOrder(
                order,
                "EXPIRED",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as CancelOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ModifyOrderCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new ModifyOrder(
                order,
                Price.Create(1.50000m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as ModifyOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
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
        }

        [Fact]
        internal void Deserialize_GivenSubmitOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var base64 =
                "iKlDb21tYW5kSWTaACQ3NDQ1YmMxOS02MGIxLTQ0NjMtYjI2Ny0wMmUzZmNmNGI4NTiwQ29tbWFuZFRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqrQ29tbWFuZFR5cGWsT3JkZXJDb21tYW5kpU9yZGVy2gC+iqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTGpT3JkZXJTaWRlo0JVWalPcmRlclR5cGWmTUFSS0VUqFF1YW50aXR5zgABhqCpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORaxPcmRlckNvbW1hbmSrU3VibWl0T3JkZXKoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMapQb3NpdGlvbklkpjEyMzQ1Ng==";

            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = serializer.Deserialize(commandBytes) as SubmitOrder;

            // Assert
            Assert.Equal(typeof(SubmitOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenCancelOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var base64 = "hqlDb21tYW5kSWTaACQzZTRkYzhkZS0wZGMyLTRiNDItOWU4MC00ZWFmY2NkODYzZDewQ29tbWFuZFRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqrQ29tbWFuZFR5cGWsT3JkZXJDb21tYW5kpU9yZGVy2gDBiqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZKdPMTIzNDU2qU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqCpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqVQcmljZacxLjAwMDAwpUxhYmVspVMxX1NMq1RpbWVJbkZvcmNlo0dURKpFeHBpcmVUaW1luDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqxPcmRlckNvbW1hbmSrQ2FuY2VsT3JkZXKsQ2FuY2VsUmVhc29up0VYUElSRUQ=";

            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = serializer.Deserialize(commandBytes) as CancelOrder;

            // Assert
            Assert.Equal(typeof(CancelOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenModifyOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var base64 = "hqlDb21tYW5kSWTaACQwY2QyZTk0Mi1jY2JhLTQxYjYtOTlhYi03MTkzYmU3ODIyYWWwQ29tbWFuZFRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqrQ29tbWFuZFR5cGWsT3JkZXJDb21tYW5kpU9yZGVy2gDBiqZTeW1ib2yrQVVEVVNELkZYQ02nT3JkZXJJZKdPMTIzNDU2qU9yZGVyU2lkZaNCVVmpT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqCpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqVQcmljZacxLjAwMDAwpUxhYmVspVMxX1NMq1RpbWVJbkZvcmNlo0dURKpFeHBpcmVUaW1luDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqxPcmRlckNvbW1hbmSrTW9kaWZ5T3JkZXKtTW9kaWZpZWRQcmljZacxLjAwMDAx";

            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = serializer.Deserialize(commandBytes) as ModifyOrder;

            // Assert
            Assert.Equal(typeof(ModifyOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_CollateralInquiry_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "g6lDb21tYW5kSWTaACQ5YWQyYzRlZi0xYTQ1LTQzMjUtYjRiYi0xYWJkN2Q5ZTJjMjiwQ29tbWFuZFRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqrQ29tbWFuZFR5cGWxQ29sbGF0ZXJhbElucXVpcnk=";

            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as CollateralInquiry;

            // Assert
            Assert.Equal(typeof(CollateralInquiry), command?.GetType());
        }
    }
}
