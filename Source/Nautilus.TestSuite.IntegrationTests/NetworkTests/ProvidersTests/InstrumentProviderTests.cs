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
    using Nautilus.Network.Compression;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class InstrumentProviderTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;
        private readonly IInstrumentRepository repository;
        private readonly IDataSerializer<Instrument> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;

        public InstrumentProviderTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.dataSerializer = new InstrumentDataSerializer();
            this.repository = new MockInstrumentRepository(this.dataSerializer);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
            this.compressor = new CompressorBypass();
        }

        [Fact]
        internal void GivenInstrumentRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55620));

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start().Wait();

            var requester = new TestDealer(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

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
            var response = (QueryFailure)requester.Send(request);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down
            requester.Stop().Wait();
            requester.Dispose();
            provider.Stop().Wait();
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithNoInstruments_ReturnsQueryFailedMessage()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55621));

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new CompressorBypass(),
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var requester = new TestDealer(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

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
            var response = (QueryFailure)requester.Send(request);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down
            requester.Stop().Wait();
            requester.Dispose();
            provider.Stop().Wait();
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentRequest_WithInstrument_ReturnsValidInstrumentResponse()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55622));

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start().Wait();

            var requester = new TestDealer(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

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
            var response = (DataResponse)requester.Send(request);
            var data = this.dataSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument, data[0]);

            // Tear Down
            requester.Stop().Wait();
            requester.Dispose();
            provider.Stop().Wait();
            provider.Dispose();
        }

        [Fact]
        internal void GivenInstrumentsRequest_WithInstruments_ReturnsValidInstrumentResponse()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55623));

            var provider = new InstrumentProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start().Wait();

            var requester = new TestDealer(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

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
            var response = (DataResponse)requester.Send(request);
            var data = this.dataSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(instrument1, data[0]);

            if (data.Length > 1)
            {
                Assert.Equal(instrument2, data[1]);
            }

            // Tear Down
            requester.Stop().Wait();
            requester.Dispose();
            provider.Stop().Wait();
            provider.Dispose();
        }
    }
}
