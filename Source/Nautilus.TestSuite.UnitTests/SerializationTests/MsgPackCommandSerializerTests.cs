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
                new AccountId("FXCM", "028999999"),
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
                new AccountId("FXCM", "028999999"),
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
                new AccountId("FXCM", "028999999"),
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
                new AccountId("FXCM", "028999999"),
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
                new AccountId("FXCM", "028999999"),
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
            var command = new AccountInquiry(
                new AccountId("FXCM", "028999999"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

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
            var hexString = "hKRUeXBlrkFjY291bnRJbnF1aXJ5oklk2gAkY2JjMTUxMjMtZDVjNy00Yzc0LWI3MmYtNWY5ZDU5MGEzZDNlqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqpQWNjb3VudElkqVVOS05PV04tMA==";
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
            var base64 = "iKRUeXBlq1N1Ym1pdE9yZGVyoklk2gAkNWJjODQwMzItZTVjNS00MTE5LWEyZjYtNDE0Yzg0MDNiNmYyqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElkqVVOS05PV04tMKpQb3NpdGlvbklkpjEyMzQ1NqVPcmRlctoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJGVjOTVlYTI5LTdkZGMtNDlkNC1iZmJmLWZmZGI4YmM3MmUyMA==";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkYjQ1YjUyNDQtNWQ3Ny00ZDU5LTgyM2EtNGI5YzU5MzRmZGJmqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElkqVVOS05PV04tMKpQb3NpdGlvbklkpjEyMzQ1NqVFbnRyedoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDk0N2UyOWVjLTdiOGQtNGNhZS1hZjY1LTVmZTVkMTY1NzhmZahTdG9wTG9zc9oA8IuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0yplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU0VMTKlPcmRlclR5cGWrU1RPUF9NQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZacwLjk5OTAwpUxhYmVspE5PTkWrVGltZUluRm9yY2WjR1RDqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDFhMGMwMTFmLWJhZGItNDcyZC1hMTVkLWQ3NjIzN2I5N2Y4ZKpUYWtlUHJvZml0oYA=";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkNDkzNWM4NGMtMzg4MC00MmE2LTk1NjctMTIzNzQwYTAxMzhlqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElkqVVOS05PV04tMKpQb3NpdGlvbklkpjEyMzQ1NqVFbnRyedoA6YuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaVMSU1JVKhRdWFudGl0ec4AAYagpVByaWNlpzAuOTk5MDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkYjA2MGU3MTQtZDkzMC00NGVmLTkyY2ItOGJmMjA3YWIzNzQzqFN0b3BMb3Nz2gDwi6JJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTKmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaRTRUxMqU9yZGVyVHlwZatTVE9QX01BUktFVKhRdWFudGl0ec4AAYagpVByaWNlpzEuMDAwMDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkY2FjMmE1N2YtMTY3Yy00MTg2LTljNjktNTc5NjdhYjVlNjhhqlRha2VQcm9maXTaAOqLoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtM6ZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlpFNFTEypT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqClUHJpY2WnMS4wMDAxMKVMYWJlbKROT05Fq1RpbWVJbkZvcmNlo0dUQ6pFeHBpcmVUaW1lpE5PTkWpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqZJbml0SWTaACRjN2U0OGE4Ni1mY2I5LTRlNDEtOWYwNi0zY2JiMDI2N2FhM2I=";
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
            var base64 = "iKRUeXBlq01vZGlmeU9yZGVyoklk2gAkMjQ2YzRjODUtYzFlOC00YTU3LTlmZGQtY2I3MGVkZmQyMjFhqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElkqVVOS05PV04tMKdPcmRlcklkqE8tMTIzNDU2rU1vZGlmaWVkUHJpY2WnMS4wMDAwMQ==";
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
            var base64 = "iKRUeXBlq0NhbmNlbE9yZGVyoklk2gAkZGRjNGJlYTAtOGQ3YS00MmY0LThjYTktOWZhMDkxYWRiZThhqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElkqVVOS05PV04tMKdPcmRlcklkqE8tMTIzNDU2rENhbmNlbFJlYXNvbqdFWFBJUkVE";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as CancelOrder;

            // Assert
            Assert.Equal(typeof(CancelOrder), command?.GetType());
        }
    }
}
