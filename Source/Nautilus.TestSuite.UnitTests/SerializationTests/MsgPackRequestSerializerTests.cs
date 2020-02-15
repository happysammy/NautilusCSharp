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
    using Nautilus.Serialization.Serializers;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackRequestSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackRequestSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
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
                { "FromDateTime", dateTime.ToIsoString() },
                { "ToDateTime", dateTime.ToIsoString() },
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
            Assert.Equal(dateTime.ToIsoString(), unpacked.Query["FromDateTime"]);
            Assert.Equal(dateTime.ToIsoString(), unpacked.Query["ToDateTime"]);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
