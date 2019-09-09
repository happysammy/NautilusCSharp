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
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
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
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new PositionId("P-123456"),
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
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new PositionId("P-123456"),
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
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new PositionId("P-123456"),
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
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new OrderId("O-123456"),
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
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new OrderId("O-123456"),
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
                new AccountId("FXCM", "028999999", "SIMULATED"),
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
            var hexString = "hKRUeXBlrkFjY291bnRJbnF1aXJ5oklk2gAkNThhMTU2MjAtNzhhMS00MTBiLWJiODAtYjk5NjQzYTg4MjEwqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUQ=";
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
            var base64 = "iKRUeXBlq1N1Ym1pdE9yZGVyoklk2gAkNTNhZjhmZTgtYTlmYS00NDE3LTk4N2ItNTRlN2RhYTE0MTk4qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSqUG9zaXRpb25JZKhQLTEyMzQ1NqVPcmRlctoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJGZjY2M2Y2M2LTcyMDEtNGExZC1hZTM2LTVlNGEzOTU2ZWE5YQ==";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkZDkzN2U1MjMtY2JkZi00NmRmLWE3NTMtNGJhYTQxYjVkNTQzqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSqUG9zaXRpb25JZKhQLTEyMzQ1NqVFbnRyedoA54uiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaZNQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZaROT05FpUxhYmVspE5PTkWrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDdjNTEwMDBiLWE4MDMtNDllZi04M2IzLTA1MjJjNGQ3MmZjOKhTdG9wTG9zc9oA8IuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0yplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU0VMTKlPcmRlclR5cGWrU1RPUF9NQVJLRVSoUXVhbnRpdHnOAAGGoKVQcmljZacwLjk5OTAwpUxhYmVspE5PTkWrVGltZUluRm9yY2WjR1RDqkV4cGlyZVRpbWWkTk9ORalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBapkluaXRJZNoAJDgwMmI3YzA5LTlkY2YtNGJjYS05NGQ4LTE3NjRiNjk0ZmU1NqpUYWtlUHJvZml0oYA=";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkOThkYjliYjUtMjAwZi00MDM5LWFjMTEtMGU0ZGZlMGU5YmQ1qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSqUG9zaXRpb25JZKhQLTEyMzQ1NqVFbnRyedoA6YuiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQlVZqU9yZGVyVHlwZaVMSU1JVKhRdWFudGl0ec4AAYagpVByaWNlpzAuOTk5MDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkNmI4OWNmYjgtNGI5MS00NTczLWFhOTQtNjU2MDk1Zjc3YTFiqFN0b3BMb3Nz2gDwi6JJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTKmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaRTRUxMqU9yZGVyVHlwZatTVE9QX01BUktFVKhRdWFudGl0ec4AAYagpVByaWNlpzEuMDAwMDClTGFiZWykTk9ORatUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROT05FqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqmSW5pdElk2gAkM2NiZmUxYmItNWJhMS00ZTc0LThmODYtOThmMDE3YTU2M2RmqlRha2VQcm9maXTaAOqLoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtM6ZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlpFNFTEypT3JkZXJUeXBlpUxJTUlUqFF1YW50aXR5zgABhqClUHJpY2WnMS4wMDAxMKVMYWJlbKROT05Fq1RpbWVJbkZvcmNlo0dUQ6pFeHBpcmVUaW1lpE5PTkWpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqZJbml0SWTaACQ2ZmVlOWVmYy0wY2EzLTQ4YTEtODgwMi1mMzU2NjkxMmM1Njk=";
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
            var base64 = "iKRUeXBlq01vZGlmeU9yZGVyoklk2gAkZmNmNWI0ZDEtYzUxNS00YmNkLTg4NWEtY2U3MzNkMGNjNzM1qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSnT3JkZXJJZKhPLTEyMzQ1Nq1Nb2RpZmllZFByaWNlpzEuMDAwMDE=";
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
            var base64 = "iKRUeXBlq0NhbmNlbE9yZGVyoklk2gAkNmIwNmRhNTMtMGJjMy00YTczLWI5YzItMmUzZmUxYjQ2ZjAzqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKpTdHJhdGVneUlkqlNDQUxQRVItMDGpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSnT3JkZXJJZKhPLTEyMzQ1NqxDYW5jZWxSZWFzb26nRVhQSVJFRA==";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as CancelOrder;

            // Assert
            Assert.Equal(typeof(CancelOrder), command?.GetType());
        }
    }
}
