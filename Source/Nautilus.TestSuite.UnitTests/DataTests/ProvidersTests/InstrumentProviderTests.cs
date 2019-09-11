// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.ProvidersTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Providers;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class InstrumentProviderTests
    {
        private const string TEST_ADDRESS = "tcp://localhost:55524";

        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly IInstrumentRepository repository;
        private readonly IDataSerializer<Instrument[]> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly InstrumentProvider provider;

        public InstrumentProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.repository = new MockInstrumentRepository();
            this.dataSerializer = new BsonInstrumentArraySerializer();
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();

            this.provider = new InstrumentProvider(
                container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new NetworkPort(55524));
        }

        [Fact]
        internal void GivenInstrumentRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var instrument = StubInstrumentProvider.AUDUSD();

            var query = new Dictionary<string, string>
            {
                { "DataType", "Instrument" },
                { "Symbol", instrument.Symbol.Value },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (QueryFailure)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            this.provider.Stop();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var query = new Dictionary<string, string>
            {
                { "DataType", "Instrument[]" },
                { "Venue", "FXCM" },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (QueryFailure)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            this.provider.Stop();
        }

        [Fact]
        internal void GivenInstrumentRequest_WithInstrument_ReturnsValidInstrumentResponse()
        {
            // Arrange
            this.provider.Start();  // Allow provider to start
            Task.Delay(100).Wait();

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var instrument = StubInstrumentProvider.AUDUSD();
            this.repository.Add(instrument, StubZonedDateTime.UnixEpoch());

            var query = new Dictionary<string, string>
            {
                { "DataType", "Instrument" },
                { "Symbol", instrument.Symbol.Value },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (DataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            var data = this.dataSerializer.Deserialize(response.Data);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument, data[0]);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            this.provider.Stop();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithInstruments_ReturnsValidInstrumentResponse()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var instrument1 = StubInstrumentProvider.AUDUSD();
            var instrument2 = StubInstrumentProvider.EURUSD();
            this.repository.Add(instrument1, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument2, StubZonedDateTime.UnixEpoch());

            var query = new Dictionary<string, string>
            {
                { "DataType", "Instrument[]" },
                { "Venue", "FXCM" },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (DataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            var data = this.dataSerializer.Deserialize(response.Data);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument1, data[0]);
            Assert.Equal(instrument2, data[1]);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            this.provider.Stop();
        }
    }
}
