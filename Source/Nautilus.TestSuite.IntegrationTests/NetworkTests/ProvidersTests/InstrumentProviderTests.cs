// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests.ProvidersTests
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
    using Nautilus.Network.Configuration;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.Compression;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class InstrumentProviderTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly IComponentryContainer container;
        private readonly IInstrumentRepository repository;
        private readonly IDataSerializer<Instrument> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;

        public InstrumentProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.dataSerializer = new InstrumentDataSerializer();
            this.repository = new MockInstrumentRepository(this.dataSerializer);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
        }

        [Fact]
        internal void GivenInstrumentRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            ushort testPort = 55620;
            var testAddress = $"tcp://localhost:{testPort}";

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new CompressorBypass(),
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(testAddress);

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

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down
            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);
            requester.Disconnect(testAddress);
            requester.Dispose();
            provider.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            ushort testPort = 55621;
            var testAddress = $"tcp://localhost:{testPort}";

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new CompressorBypass(),
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(testAddress);

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

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down
            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);
            requester.Disconnect(testAddress);
            requester.Dispose();
            provider.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentRequest_WithInstrument_ReturnsValidInstrumentResponse()
        {
            // Arrange
            ushort testPort = 55622;
            var testAddress = $"tcp://localhost:{testPort}";

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new CompressorBypass(),
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();

            var requester = new RequestSocket();
            requester.Connect(testAddress);

            var instrument = StubInstrumentProvider.AUDUSD();
            this.repository.Add(instrument);

            var query = new Dictionary<string, string>
            {
                { "DataType", "Instrument[]" },
                { "Symbol", instrument.Symbol.Value },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (DataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            var data = this.dataSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument, data[0]);

            // Tear Down
            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);
            requester.Disconnect(testAddress);
            requester.Dispose();
            provider.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithInstruments_ReturnsValidInstrumentResponse()
        {
            // Arrange
            ushort testPort = 55623;
            var testAddress = $"tcp://localhost:{testPort}";

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new CompressorBypass(),
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new RequestSocket();
            requester.Connect(testAddress);
            Task.Delay(100).Wait();  // Allow socket to connect

            var instrument1 = StubInstrumentProvider.AUDUSD();
            var instrument2 = StubInstrumentProvider.EURUSD();
            this.repository.Add(instrument1);
            this.repository.Add(instrument2);

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
            var data = this.dataSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument1, data[0]);

            if (data.Length > 1)
            {
                Assert.Equal(instrument2, data[1]);
            }

            // Tear Down
            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);
            requester.Disconnect(testAddress);
            requester.Dispose();
            provider.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            provider.Dispose();
        }
    }
}
