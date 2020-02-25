// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

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
            var serializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());

            var request = new Connect(
                "Trader-001",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (Connect)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);
            Assert.Equal("Trader-001", unpacked.ClientId);
        }

        [Fact]
        internal void CanSerializeAndDeserialize_DisconnectRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());

            var request = new Disconnect(
                "Trader-001",
                new SessionId("Trader-001", StubZonedDateTime.UnixEpoch()),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (Disconnect)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);
            Assert.Equal("Trader-001", unpacked.ClientId);
            Assert.Equal("Trader-001-1970-01-01-0", unpacked.SessionId.Value);
        }

        [Fact]
        internal void CanSerializeAndDeserialize_DataRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
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
