// -------------------------------------------------------------------------------------------------
// <copyright file="BarProviderTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Providers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.Bson;
    using Nautilus.Serialization.MessagePack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarProviderTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly IComponentryContainer container;
        private readonly IBarRepository repository;
        private readonly IDataSerializer<Bar> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;

        public BarProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.dataSerializer = new BarDataSerializer();
            this.repository = new MockBarRepository(this.dataSerializer);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
            Task.Delay(100); // Allow cleanup
        }

        [Fact]
        internal void GivenBarDataRequest_WithNoBars_ReturnsQueryFailedMessage()
        {
            // Arrange
            ushort testPort = 55523;
            var testAddress = $"tcp://localhost:{55523}";

            var provider = new BarProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var barType = StubBarType.AUDUSD_OneMinuteAsk();

            var requester = new RequestSocket();
            requester.Connect(testAddress);

            Task.Delay(100).Wait();  // Allow socket to connect

            var query = new Dictionary<string, string>
            {
                { "DataType", "Bar[]" },
                { "Symbol", barType.Symbol.Value },
                { "Specification", barType.Specification.ToString() },
                { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "Limit", "0" },
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
        }

        [Fact]
        internal void GivenBarDataRequest_WithBars_ReturnsValidBarDataResponse()
        {
            // Arrange
            ushort testPort = 55524;
            var testAddress = $"tcp://localhost:{testPort}";

            var provider = new BarProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                EncryptionConfig.None(),
                new NetworkPort(testPort));
            provider.Start();
            Task.Delay(100).Wait();

            var barType = StubBarType.AUDUSD_OneMinuteAsk();
            var bar1 = StubBarProvider.Build();
            var bar2 = StubBarProvider.Build();

            this.repository.Add(barType, bar1);
            this.repository.Add(barType, bar2);

            var requester = new RequestSocket();
            requester.Connect(testAddress);

            Task.Delay(100).Wait();  // Allow socket to connect

            var query = new Dictionary<string, string>
            {
                { "DataType", "Bar[]" },
                { "Symbol", barType.Symbol.Value },
                { "Specification", barType.Specification.ToString() },
                { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "Limit", "0" },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(request));
            var response = (DataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            var bars = this.dataSerializer.DeserializeBlob(response.Data);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(2, bars.Length);
            Assert.Equal(bar1, bars[0]);
            Assert.Equal(bar2, bars[1]);
        }
    }
}
