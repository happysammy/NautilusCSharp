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
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MsgPackResponseSerializerTests
    {
        private readonly ITestOutputHelper output;
        private readonly MsgPackResponseSerializer serializer;

        public MsgPackResponseSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.serializer = new MsgPackResponseSerializer();
        }

        [Fact]
        internal void CanSerializeAndDeserialize_MessageReceived()
        {
            // Arrange
            var messageType = nameof(MockMessage);
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
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
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
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
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
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_TickDataResponse()
        {
            // Arrange
            var tickSerializer = new Utf8TickSerializer();
            var dataSerializer = new BsonByteArrayArraySerializer();
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

            var serializedTicks = tickSerializer.Serialize(ticks);
            var data = dataSerializer.Serialize(serializedTicks);

            var response = new DataResponse(
                data,
                typeof(Tick[]).Name,
                dataSerializer.Encoding,
                new Dictionary<string, string> { { "Symbol", symbol.Value } },
                correlationId,
                id,
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.Deserialize(deserializedResponse.Data);
            var deserializedTicks = tickSerializer.Deserialize(symbol, deserializedData);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(symbol, Symbol.FromString(deserializedResponse.Metadata["Symbol"]));
            Assert.Equal(tick1, deserializedTicks[0]);
            Assert.Equal(tick2, deserializedTicks[1]);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_BarDataResponses()
        {
            // Arrange
            var barSerializer = new Utf8BarSerializer();
            var dataSerializer = new BsonByteArrayArraySerializer();
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barSpec = new BarSpecification(1, BarStructure.Minute, PriceType.Bid);
            var correlationId = Guid.NewGuid();

            var bars = new[] { StubBarProvider.Build(), StubBarProvider.Build() };
            var serializedBars = barSerializer.Serialize(bars);
            var data = dataSerializer.Serialize(serializedBars);

            var metadata = new Dictionary<string, string>
            {
                { "Symbol", symbol.Value },
                { "Specification", barSpec.ToString() },
            };

            var response = new DataResponse(
                data,
                typeof(Bar[]).Name,
                dataSerializer.Encoding,
                metadata,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.Deserialize(deserializedResponse.Data);
            var deserializedBars = barSerializer.Deserialize(deserializedData);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(symbol, Symbol.FromString(deserializedResponse.Metadata["Symbol"]));
            Assert.Equal(barSpec, BarSpecification.FromString(deserializedResponse.Metadata["Specification"]));
            Assert.Equal(bars, deserializedBars);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_InstrumentResponses()
        {
            // Arrange
            var instrumentSerializer = new BsonInstrumentSerializer();
            var dataSerializer = new BsonByteArrayArraySerializer();
            var instrument = StubInstrumentProvider.AUDUSD();
            var correlationId = Guid.NewGuid();

            Instrument[] instruments = { instrument };
            var serializedInstruments = instrumentSerializer.Serialize(instruments);
            var data = dataSerializer.Serialize(serializedInstruments);

            var response = new DataResponse(
                data,
                typeof(Instrument[]).Name,
                dataSerializer.Encoding,
                new Dictionary<string, string> { { "Symbol", instrument.Symbol.ToString() } },
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var serializedResponse = this.serializer.Serialize(response);
            var deserializedResponse = (DataResponse)this.serializer.Deserialize(serializedResponse);
            var deserializedData = dataSerializer.Deserialize(deserializedResponse.Data);
            var deserializedInstruments = instrumentSerializer.Deserialize(deserializedData);

            // Assert
            Assert.Equal(response, deserializedResponse);
            Assert.Equal(instrument, deserializedInstruments[0]);
            Assert.Equal(correlationId, deserializedResponse.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(serializedResponse));
            this.output.WriteLine(Encoding.UTF8.GetString(serializedResponse));
        }
    }
}
