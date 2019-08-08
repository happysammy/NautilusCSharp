// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
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
        private readonly MsgPackCommandSerializer serializer;

        public MsgPackCommandSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.serializer = new MsgPackCommandSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitOrderCommands()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();

            var command = new SubmitOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new PositionId("001"),
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (SubmitOrder)this.serializer.Deserialize(packed);

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
            var packed = this.serializer.Serialize(command);
            var unpacked = (SubmitAtomicOrder)this.serializer.Deserialize(packed);

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
            var packed = this.serializer.Serialize(command);
            var unpacked = (SubmitAtomicOrder)this.serializer.Deserialize(packed);

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
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new CancelOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new OrderId("123456"),
                "EXPIRED",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (CancelOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ModifyOrderCommands()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new ModifyOrder(
                new TraderId("000"),
                new StrategyId("001"),
                new OrderId("123456"),
                Price.Create(1.50000m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (ModifyOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CollateralInquiryCommands()
        {
            // Arrange
            var command = new AccountInquiry(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (AccountInquiry)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void Deserialize_AccountInquiry_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var hexString = "g6RUeXBlrkFjY291bnRJbnF1aXJ5oklk2gAkNjJlNGNkZDktOThiOC00MGQ2LThmODctNjIyMjQxNjg2MGIxqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFo=";
            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as AccountInquiry;

            // Assert
            Assert.Equal(typeof(AccountInquiry), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenSubmitOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "h6RUeXBlq1N1Ym1pdE9yZGVyoklk2gAkNTM0NmUzNjUtYjZmZi00NTY3LTgyNDAtMDdjNDE5ZmIwM2RhqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMapQb3NpdGlvbklkpjEyMzQ1NqVPcmRlctoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJGE4NTM3MDQwLTQzNzQtNDU3Yi04MGJhLWUwZTA0MDRmYzM1Yg==";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as SubmitOrder;

            // Assert
            Assert.Equal(typeof(SubmitOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenSubmitAtomicOrderWithNoTakeProfit_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "iaRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkNTE4ZWEwNzQtZWNlNy00ZmFiLTlmZGYtZDBiOWNmMmI4YWIwqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMapQb3NpdGlvbklkpjEyMzQ1NqVFbnRyedoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDdhODNlZDI5LWZjMjUtNDFlNS1hY2Y3LTc4OTI5ODM4NjUyY6hTdG9wTG9zc9oA8IuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0yplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU0VMTKlPcmRlclR5cGWrU1RPUF9NQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZacwLjk5OTAwpUxhYmVspE5PTkWrVGltZUluRm9yY2WjR1RDqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDhiNmQ5MDcwLTFlYWMtNGZkOC1hODZjLTY3ODI2MmY3NTcyMqpUYWtlUHJvZml0oYA=";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(typeof(SubmitAtomicOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenSubmitAtomicOrderWithTakeProfit_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "iaRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkMzU2OGM1MjMtNGM5OS00ZmQ1LWFkZmItNWMxY2Y2ZGVlMGY3qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMapQb3NpdGlvbklkpjEyMzQ1NqVFbnRyedoA6YuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaVMSU1JVKhRdWFudGl0ec4AAYagpVByaWNlpzAuOTk5MDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkZDZhZDQ2ZDEtODY0Yy00NDc3LTg4OGMtNzIyZTNhOWNkMmJhqFN0b3BMb3Nz2gDwi6JJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTKmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaRTRUxMqU9yZGVyVHlwZatTVE9QX01BUktFVKhRdWFudGl0ec4AAYagpVByaWNlpzEuMDAwMDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkYTUxZjZmZDctZWQ3Yy00NzIxLWI5M2QtNGM1MDQyNmEyY2RkqlRha2VQcm9maXTaAOqLoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtM6ZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlpFNFTEypT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqClUHJpY2WnMS4wMDAxMKVMYWJlbKROT05Fq1RpbWVJbkZvcmNlo0dUQ6pFeHBpcmVUaW1lpE5PTkWpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqZJbml0SWTaACRmNzEwMjgwMC02NjRjLTQ0YmUtOGQwZS01M2NiNjkwMjRhNTQ=";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as SubmitAtomicOrder;

            // Assert
            Assert.Equal(typeof(SubmitAtomicOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenModifyOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "h6RUeXBlq01vZGlmeU9yZGVyoklk2gAkNmY4N2ZjMWQtYjQ1Yy00MGEwLWEzNTEtY2MwYWJjZTMzYTY0qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMadPcmRlcklkqE8tMTIzNDU2rU1vZGlmaWVkUHJpY2WnMS4wMDAwMQ==";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as ModifyOrder;

            // Assert
            Assert.Equal(typeof(ModifyOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenCancelOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "h6RUeXBlq0NhbmNlbE9yZGVyoklk2gAkMjcwMTM5OGQtMzZiOS00ZmU1LThjZDgtY2U3MTM3OTVmMGQzqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVHJhZGVyLTAwMapTdHJhdGVneUlkqVNDQUxQRVIwMadPcmRlcklkqE8tMTIzNDU2rENhbmNlbFJlYXNvbqdFWFBJUkVE";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as CancelOrder;

            // Assert
            Assert.Equal(typeof(CancelOrder), command?.GetType());
        }
    }
}
