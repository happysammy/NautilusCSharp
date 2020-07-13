// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nautilus.Core.Extensions;
using Nautilus.Data.Messages.Requests;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Network.Identifiers;
using Nautilus.Network.Messages;
using Nautilus.Serialization.MessageSerializers;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackRequestSerializerTests : TestBase
    {
        public MsgPackRequestSerializerTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ConnectRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new Connect(
                new ClientId("Trader-001"),
                "None",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (Connect)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);
            Assert.Equal("Trader-001", unpacked.ClientId.Value);
        }

        [Fact]
        internal void CanSerializeAndDeserialize_DisconnectRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new Disconnect(
                new ClientId("Trader-001"),
                SessionId.Create(new ClientId("Trader-001"), StubZonedDateTime.UnixEpoch(), "None"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (Disconnect)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);
            Assert.Equal(new ClientId("Trader-001"), unpacked.ClientId);
            Assert.Equal("Trader-001-e5db3dad8222a27e5d2991d11ad65f0f74668a4cfb629e97aa6920a73a012f87", unpacked.SessionId.Value);
        }

        [Fact]
        internal void CanSerializeAndDeserialize_DataRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var symbol = new Symbol("AUD/USD", new Venue("FXCM"));
            var dateTime = StubZonedDateTime.UnixEpoch();
            var query = new Dictionary<string, string>
            {
                { "DataType", "Tick[]" },
                { "Symbol", symbol.ToString() },
                { "FromDateTime", dateTime.ToIso8601String() },
                { "ToDateTime", dateTime.ToIso8601String() },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (DataRequest)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);
            Assert.Equal("Tick[]", unpacked.Query["DataType"]);
            Assert.Equal(symbol.ToString(), unpacked.Query["Symbol"]);
            Assert.Equal(dateTime.ToIso8601String(), unpacked.Query["FromDateTime"]);
            Assert.Equal(dateTime.ToIso8601String(), unpacked.Query["ToDateTime"]);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
