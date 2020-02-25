// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackResponseSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Messages.Responses;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MsgPackResponseSerializerTests : TestBase
    {
        private readonly MsgPackResponseSerializer serializer;

        public MsgPackResponseSerializerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.serializer = new MsgPackResponseSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_Connected()
        {
            // Arrange
            var correlationId = Guid.NewGuid();

            var response = new Connected(
                "Ok",
                "NautilusData.TickProvider",
                new SessionId("Trader-001", StubZonedDateTime.UnixEpoch()),
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (Connected)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            Assert.Equal("NautilusData.TickProvider", unpacked.ServerId);
            Assert.Equal("Trader-001-1970-01-01-0", unpacked.SessionId.Value);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_Disconnected()
        {
            // Arrange
            var correlationId = Guid.NewGuid();

            var response = new Disconnected(
                "Ok",
                "NautilusData.TickProvider",
                new SessionId("Trader-001", StubZonedDateTime.UnixEpoch()),
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (Disconnected)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            Assert.Equal("NautilusData.TickProvider", unpacked.ServerId);
            Assert.Equal("Trader-001-1970-01-01-0", unpacked.SessionId.Value);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_MessageReceived()
        {
            // Arrange
            var messageType = nameof(Connect);
            var correlationId = Guid.NewGuid();

            var response = new MessageReceived(
                messageType,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (MessageReceived)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            Assert.Equal(messageType, unpacked.ReceivedType);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_MessageRejected()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var message = "malformed message";

            var response = new MessageRejected(
                message,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (MessageRejected)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            Assert.Equal(message, unpacked.Message);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_QueryFailure()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var message = "data not found";

            var response = new QueryFailure(
                message,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (QueryFailure)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            Assert.Equal(message, unpacked.Message);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_TickDataResponse()
        {
            // Arrange
            var dataSerializer = new TickDataSerializer();
            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = new Tick(
                symbol,
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Volume.One(),
                Volume.One(),
                datetimeFrom);
            var tick2 = new Tick(
                symbol,
                Price.Create(1.00010m),
                Price.Create(1.00020m),
                Volume.One(),
                Volume.One(),
                datetimeTo);

            var ticks = new[] { tick1, tick2 };

            var correlationId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var metadata = new Dictionary<string, string> { { "Symbol", symbol.Value } };
            var serializedTicks = dataSerializer.Serialize(ticks);
            var data = dataSerializer.SerializeBlob(serializedTicks, metadata);

            var response = new DataResponse(
                data,
                typeof(Tick[]).Name,
                dataSerializer.BlobEncoding,
                correlationId,
                id,
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.DeserializeBlob(deserializedResponse.Data);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(tick1, deserializedData[0]);
            Assert.Equal(tick2, deserializedData[1]);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.Output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.Output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_BarDataResponses()
        {
            // Arrange
            var dataSerializer = new BarDataSerializer();
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barSpec = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);
            var correlationId = Guid.NewGuid();

            var bars = new[] { StubBarProvider.Build(), StubBarProvider.Build() };
            var serializedBars = dataSerializer.Serialize(bars);

            var metadata = new Dictionary<string, string>
            {
                { "Symbol", symbol.Value },
                { "Specification", barSpec.ToString() },
            };

            var data = dataSerializer.SerializeBlob(serializedBars, metadata);

            var response = new DataResponse(
                data,
                typeof(Bar[]).Name,
                dataSerializer.BlobEncoding,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.DeserializeBlob(deserializedResponse.Data);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(bars, deserializedData);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.Output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.Output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_InstrumentResponses()
        {
            // Arrange
            var dataSerializer = new InstrumentDataSerializer();
            var instrument = StubInstrumentProvider.AUDUSD();
            var correlationId = Guid.NewGuid();

            Instrument[] instruments = { instrument };
            var serializedInstruments = dataSerializer.Serialize(instruments);

            var metadata = new Dictionary<string, string> { { "Symbol", instrument.Symbol.ToString() } };
            var data = dataSerializer.SerializeBlob(serializedInstruments, metadata);

            var response = new DataResponse(
                data,
                typeof(Instrument[]).Name,
                dataSerializer.BlobEncoding,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.DeserializeBlob(deserializedResponse.Data);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(instrument, deserializedData[0]);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.Output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.Output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }
    }
}
