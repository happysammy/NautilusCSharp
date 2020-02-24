// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackCommandSerializerTests : TestBase
    {
        private readonly MsgPackCommandSerializer serializer;

        public MsgPackCommandSerializerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.serializer = new MsgPackCommandSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_AccountInquiryCommands()
        {
            // Arrange
            var command = new AccountInquiry(
                TraderId.FromString("TESTER-000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (AccountInquiry)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(order, unpacked.Order);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(atomicOrder, unpacked.AtomicOrder);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
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
            Assert.Equal(atomicOrder, unpacked.AtomicOrder);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CancelOrderCommands()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new CancelOrder(
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new OrderId("O-123456"),
                "EXPIRED",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (CancelOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ModifyOrderCommands()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new ModifyOrder(
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new OrderId("O-123456"),
                order.Quantity,
                Price.Create(1.50000m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (ModifyOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void Deserialize_AccountInquiry_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "haRUeXBlrkFjY291bnRJbnF1aXJ5oklk2gAkOTJkZWZjNTMtM2M4YS00NWFkLThjMjItZDdiNmUzYmJiNzJlqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKlBY2NvdW50SWS2TkFVVElMVVMtMDAwLVNJTVVMQVRFRA==";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as AccountInquiry;

            // Assert
            Assert.Equal(typeof(AccountInquiry), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenSubmitOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "iKRUeXBlq1N1Ym1pdE9yZGVyoklk2gAkNDVmYTg5MjgtODJmYi00ZGMxLTkyNWMtZjVmNWE2NzVjYjEzqVRpbWVzdGFtcLgxOTcwLTAxLTAxIDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKlBY2NvdW50SWS2TkFVVElMVVMtMDAwLVNJTVVMQVRFRKpTdHJhdGVneUlkqlNDQUxQRVItMDGqUG9zaXRpb25JZKhQLTEyMzQ1NqVPcmRlctoA+4yiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQnV5qU9yZGVyVHlwZaZNYXJrZXSoUXVhbnRpdHmmMTAwMDAwpVByaWNlpE5PTkWlTGFiZWykTk9ORaxPcmRlclB1cnBvc2WkTm9uZatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROT05FpkluaXRJZNoAJDQ5NmE3NmZlLTNlYTUtNDZlMS05M2VkLTdhYmUzNDcyYjFjYqlUaW1lc3RhbXC4MTk3MC0wMS0wMSAwMDowMDowMC4wMDBa";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkOGU1MTJlNTQtNGMwYS00ZDQ4LTgyYTYtOGM0NThmY2I2MDI3qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKlBY2NvdW50SWS2TkFVVElMVVMtMDAwLVNJTVVMQVRFRKpTdHJhdGVneUlkqlNDQUxQRVItMDGqUG9zaXRpb25JZKhQLTEyMzQ1NqVFbnRyedoA/IyiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQnV5qU9yZGVyVHlwZaZNYXJrZXSoUXVhbnRpdHmmMTAwMDAwpVByaWNlpE5vbmWlTGFiZWykTm9uZaxPcmRlclB1cnBvc2WlRW50cnmrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTm9uZaZJbml0SWTaACQwN2RkNDFiMS01NGM2LTRkOTYtYmY3Ny1lNGI1NDA0YzgwNGOpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqhTdG9wTG9zc9oBAYyiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0yplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU2VsbKlPcmRlclR5cGWkU3RvcKhRdWFudGl0eaYxMDAwMDClUHJpY2WnMC45OTkwMKVMYWJlbKROb25lrE9yZGVyUHVycG9zZahTdG9wTG9zc6tUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROb25lpkluaXRJZNoAJDQyNDBhMWQwLTg3ZTQtNDlkNi1hYTRlLTJkNWE1YjEyMTUyNalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBaqlRha2VQcm9maXShgA==";
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
            var base64 = "iqRUeXBlsVN1Ym1pdEF0b21pY09yZGVyoklk2gAkOTZlYmJmNjUtZTA1Zi00ZmYxLTlkZjMtMDE5YTNhNWRjNWRjqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKlBY2NvdW50SWS2TkFVVElMVVMtMDAwLVNJTVVMQVRFRKpTdHJhdGVneUlkqlNDQUxQRVItMDGqUG9zaXRpb25JZKhQLTEyMzQ1NqVFbnRyedoA/oyiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0xplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWjQnV5qU9yZGVyVHlwZaVMaW1pdKhRdWFudGl0eaYxMDAwMDClUHJpY2WnMC45OTkwMKVMYWJlbKROb25lrE9yZGVyUHVycG9zZaVFbnRyeatUaW1lSW5Gb3JjZaNEQVmqRXhwaXJlVGltZaROb25lpkluaXRJZNoAJGVlZmY0OGNjLTZkY2EtNDdmMS1iODAxLTgwODVlZGJhMTM3N6lUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBaqFN0b3BMb3Nz2gEBjKJJZLtPLTE5NzAwMTAxLTAwMDAwMC0wMDEtMDAxLTKmU3ltYm9sq0FVRFVTRC5GWENNqU9yZGVyU2lkZaRTZWxsqU9yZGVyVHlwZaRTdG9wqFF1YW50aXR5pjEwMDAwMKVQcmljZacxLjAwMDAwpUxhYmVspE5vbmWsT3JkZXJQdXJwb3NlqFN0b3BMb3Nzq1RpbWVJbkZvcmNlo0dUQ6pFeHBpcmVUaW1lpE5vbmWmSW5pdElk2gAkYzU3NmIyZmMtYjk3ZC00NmM5LTkzZWYtZDc4YWE1MjNhYmUxqVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqqVGFrZVByb2ZpdNoBBIyiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0zplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU2VsbKlPcmRlclR5cGWlTGltaXSoUXVhbnRpdHmmMTAwMDAwpVByaWNlpzEuMDAwMTClTGFiZWykTm9uZaxPcmRlclB1cnBvc2WqVGFrZVByb2ZpdKtUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROb25lpkluaXRJZNoAJDIwODlhOWJiLTExODctNDI2Mi05MWZiLWQ5ZjcyNjNkYjE0M6lUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBa";
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
            var base64 = "iKRUeXBlq01vZGlmeU9yZGVyoklk2gAkZjkyMzNkZGEtZGQwYi00Y2UzLWE1YTctODA0OTFhNTIwNDdlqVRpbWVzdGFtcLgxOTcwLTAxLTAxIDAwOjAwOjAwLjAwMFqoVHJhZGVySWSqVEVTVEVSLTAwMKlBY2NvdW50SWS2TkFVVElMVVMtMDAwLVNJTVVMQVRFRKdPcmRlcklkqE8tMTIzNDU2sE1vZGlmaWVkUXVhbnRpdHmmMTAwMDAwrU1vZGlmaWVkUHJpY2WnMS4wMDAwMQ==";
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
