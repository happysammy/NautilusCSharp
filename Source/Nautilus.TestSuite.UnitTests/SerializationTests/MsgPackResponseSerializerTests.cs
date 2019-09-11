// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackResponseSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Messages.Responses;
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
            var dataSerializer = new BsonTickArraySerializer();
            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = new Tick(symbol, 1.00000m, 1.00000m, datetimeFrom);
            var tick2 = new Tick(symbol, 1.00010m, 1.00020m, datetimeTo);

            var ticks = new[] { tick1, tick2 };

            var correlationId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var data = dataSerializer.Serialize(ticks);

            var response = new DataResponse(
                data,
                dataSerializer.DataEncoding,
                correlationId,
                id,
                StubZonedDateTime.UnixEpoch());

            // Act
            var serialized = this.serializer.Serialize(response);
            var deserialized = (DataResponse)this.serializer.Deserialize(serialized);
            var deserializedData = dataSerializer.Deserialize(deserialized.Data);

            // Assert
            Assert.Equal(response, deserialized);
            Assert.Equal(symbol, deserializedData[0].Symbol);
            Assert.Equal(tick1, deserializedData[0]);
            Assert.Equal(tick2, deserializedData[1]);
            Assert.Equal(correlationId, deserialized.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(serialized));
            this.output.WriteLine(Encoding.UTF8.GetString(serialized));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_BarDataResponses()
        {
            // Arrange
            var dataSerializer = new BsonBarDataFrameSerializer();
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barSpec = new BarSpecification(1, Resolution.MINUTE, QuoteType.BID);
            var correlationId = Guid.NewGuid();

            var bars = new[] { StubBarBuilder.Build(), StubBarBuilder.Build() };
            var dataFrame = new BarDataFrame(new BarType(symbol, barSpec), bars);
            var data = dataSerializer.Serialize(dataFrame);

            var response = new DataResponse(
                data,
                dataSerializer.DataEncoding,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var serialized = this.serializer.Serialize(response);
            var deserialized = (DataResponse)this.serializer.Deserialize(serialized);
            var deserializedData = dataSerializer.Deserialize(deserialized.Data);

            // Assert
            Assert.Equal(response, deserialized);
            Assert.Equal(symbol, deserializedData.BarType.Symbol);
            Assert.Equal(barSpec, deserializedData.BarType.Specification);
            Assert.Equal(bars, deserializedData.Bars);
            Assert.Equal(correlationId, deserialized.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(serialized));
            this.output.WriteLine(Encoding.UTF8.GetString(serialized));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_InstrumentResponses()
        {
            // Arrange
            var dataSerializer = new BsonInstrumentArraySerializer();
            var instrument = StubInstrumentProvider.AUDUSD();
            var correlationId = Guid.NewGuid();

            var instruments = new[] { instrument };
            var data = dataSerializer.Serialize(instruments);

            var response = new DataResponse(
                data,
                dataSerializer.DataEncoding,
                correlationId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = this.serializer.Serialize(response);
            var unpacked = (DataResponse)this.serializer.Deserialize(packed);

            // Assert
            Assert.Equal(response, unpacked);
            Assert.Equal(correlationId, unpacked.CorrelationId);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
