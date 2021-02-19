// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Commands;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Serialization.MessageSerializers;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
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
        internal void CanSerializeAndDeserialize_SubmitBracketOrderCommands_WithNoTakeProfit()
        {
            // Arrange
            var entry = new StubOrderBuilder().BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().BuildStopMarketOrder();
            var bracketOrder = new BracketOrder(entry, stopLoss);

            var command = new SubmitBracketOrder(
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new PositionId("P-123456"),
                bracketOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (SubmitBracketOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(bracketOrder, unpacked.BracketOrder);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitBracketOrderCommands_WithTakeProfit()
        {
            // Arrange
            var entry = new StubOrderBuilder().EntryOrder("O-123456").BuildMarketOrder();
            var stopLoss = new StubOrderBuilder().StopLossOrder("O-123457").BuildStopMarketOrder();
            var takeProfit = new StubOrderBuilder().TakeProfitOrder("O-123458").BuildLimitOrder();
            var bracketOrder = new BracketOrder(entry, stopLoss, takeProfit);

            var command = new SubmitBracketOrder(
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new StrategyId("EMACross", "001"),
                new PositionId("P-123456"),
                bracketOrder,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(command);
            var unpacked = (SubmitBracketOrder)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(command, unpacked);
            Assert.Equal(bracketOrder, unpacked.BracketOrder);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CancelOrderCommands()
        {
            // Arrange
            var command = new CancelOrder(
                new TraderId("TESTER", "000"),
                new AccountId("FXCM", "028999999", "SIMULATED"),
                new OrderId("O-123456"),
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
        internal void Deserialize_GivenSubmitBracketOrderWithNoTakeProfit_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "iqRUeXBlslN1Ym1pdEJyYWNrZXRPcmRlcqJJZNoAJDU1YjAxNjljLTI4OGUtNDZiMC1iODA5LTM2OTJmNWQ0YjVlM6lUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBaqFRyYWRlcklkqlRFU1RFUi0wMDCpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSqU3RyYXRlZ3lJZKpTQ0FMUEVSLTAxqlBvc2l0aW9uSWSoUC0xMjM0NTalRW50cnnaAPyMoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtMaZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0J1ealPcmRlclR5cGWmTWFya2V0qFF1YW50aXR5pjEwMDAwMKVQcmljZaROb25lpUxhYmVspE5vbmWsT3JkZXJQdXJwb3NlpUVudHJ5q1RpbWVJbkZvcmNlo0RBWapFeHBpcmVUaW1lpE5vbmWmSW5pdElk2gAkNDFhZjM5MTctNzdlZC00ZDg0LThiNWUtYzY4YjNmMDgyYjk5qVRpbWVzdGFtcLgxOTcwLTAxLTAxVDAwOjAwOjAwLjAwMFqoU3RvcExvc3PaAQGMoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtMqZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlpFNlbGypT3JkZXJUeXBlpFN0b3CoUXVhbnRpdHmmMTAwMDAwpVByaWNlpzAuOTk5MDClTGFiZWykTm9uZaxPcmRlclB1cnBvc2WoU3RvcExvc3OrVGltZUluRm9yY2WjR1RDqkV4cGlyZVRpbWWkTm9uZaZJbml0SWTaACRmYTY4OWJjMy1jNjg0LTRlOGUtODc2Ny01YTY0N2IyNzZlNDapVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqpUYWtlUHJvZml0oYA=";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as SubmitBracketOrder;

            // Assert
            Assert.Equal(typeof(SubmitBracketOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenSubmitBracketOrderWithTakeProfit_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var base64 = "iqRUeXBlslN1Ym1pdEJyYWNrZXRPcmRlcqJJZNoAJDdkNmNjMTc2LTIzYWUtNGUxYy1iMDY2LWRiYjU2YzM5ZDc5OalUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBaqFRyYWRlcklkqlRFU1RFUi0wMDCpQWNjb3VudElktk5BVVRJTFVTLTAwMC1TSU1VTEFURUSqU3RyYXRlZ3lJZKpTQ0FMUEVSLTAxqlBvc2l0aW9uSWSoUC0xMjM0NTalRW50cnnaAP6Moklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtMaZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlo0J1ealPcmRlclR5cGWlTGltaXSoUXVhbnRpdHmmMTAwMDAwpVByaWNlpzAuOTk5MDClTGFiZWykTm9uZaxPcmRlclB1cnBvc2WlRW50cnmrVGltZUluRm9yY2WjREFZqkV4cGlyZVRpbWWkTm9uZaZJbml0SWTaACQ3OTA4NDY3NC03NDAyLTQ5MDctOTg5ZS1mNTJkZjQxMzEwZjKpVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWqhTdG9wTG9zc9oBAYyiSWS7Ty0xOTcwMDEwMS0wMDAwMDAtMDAxLTAwMS0yplN5bWJvbKtBVURVU0QuRlhDTalPcmRlclNpZGWkU2VsbKlPcmRlclR5cGWkU3RvcKhRdWFudGl0eaYxMDAwMDClUHJpY2WnMS4wMDAwMKVMYWJlbKROb25lrE9yZGVyUHVycG9zZahTdG9wTG9zc6tUaW1lSW5Gb3JjZaNHVEOqRXhwaXJlVGltZaROb25lpkluaXRJZNoAJGVmZWUzYTRkLWNhZWItNDkzNy1iMTRmLTcyODk2YjcyZTJhZqlUaW1lc3RhbXC4MTk3MC0wMS0wMVQwMDowMDowMC4wMDBaqlRha2VQcm9maXTaAQSMoklku08tMTk3MDAxMDEtMDAwMDAwLTAwMS0wMDEtM6ZTeW1ib2yrQVVEVVNELkZYQ02pT3JkZXJTaWRlpFNlbGypT3JkZXJUeXBlpUxpbWl0qFF1YW50aXR5pjEwMDAwMKVQcmljZacxLjAwMDEwpUxhYmVspE5vbmWsT3JkZXJQdXJwb3NlqlRha2VQcm9maXSrVGltZUluRm9yY2WjR1RDqkV4cGlyZVRpbWWkTm9uZaZJbml0SWTaACQ5OTkxYzg1NS0yNjk4LTRkODUtYjY4MS1lZjkyY2RhODEzNzepVGltZXN0YW1wuDE5NzAtMDEtMDFUMDA6MDA6MDAuMDAwWg==";
            var commandBytes = Convert.FromBase64String(base64);

            // Act
            var command = this.serializer.Deserialize(commandBytes) as SubmitBracketOrder;

            // Assert
            Assert.Equal(typeof(SubmitBracketOrder), command?.GetType());
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
