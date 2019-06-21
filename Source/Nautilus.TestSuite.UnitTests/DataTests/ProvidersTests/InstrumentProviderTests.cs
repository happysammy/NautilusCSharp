// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.ProvidersTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Providers;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
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
        private readonly ISerializer<Instrument> serializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly InstrumentProvider provider;

        public InstrumentProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.serializer = new MsgPackInstrumentSerializer();
            this.requestSerializer = new MsgPackRequestSerializer();
            this.responseSerializer = new MsgPackResponseSerializer();
            this.repository = new MockInstrumentRepository();
            this.provider = new InstrumentProvider(
                container,
                this.repository,
                this.serializer,
                this.requestSerializer,
                this.responseSerializer,
                NetworkAddress.LocalHost,
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

            var instrument = StubInstrumentFactory.AUDUSD();

            var request = new InstrumentRequest(
                instrument.Symbol,
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
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
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

            var request = new InstrumentsRequest(
                Venue.FXCM,
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
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
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

            var instrument = StubInstrumentFactory.AUDUSD();
            this.repository.Add(instrument, StubZonedDateTime.UnixEpoch());

            var request = new InstrumentRequest(
                instrument.Symbol,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (InstrumentResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(InstrumentResponse), response.Type);
            Assert.Equal(instrument, this.serializer.Deserialize(response.Instruments[0]));

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
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

            var instrument1 = StubInstrumentFactory.AUDUSD();
            var instrument2 = StubInstrumentFactory.EURUSD();
            this.repository.Add(instrument1, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument2, StubZonedDateTime.UnixEpoch());

            var request = new InstrumentsRequest(
                Venue.FXCM,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (InstrumentResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(InstrumentResponse), response.Type);
            Assert.Equal(instrument1, this.serializer.Deserialize(response.Instruments[0]));
            Assert.Equal(instrument2, this.serializer.Deserialize(response.Instruments[1]));

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
