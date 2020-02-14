// -------------------------------------------------------------------------------------------------
// <copyright file="TickProviderTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Providers;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.Bson;
    using Nautilus.Serialization.MessagePack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickProviderTests : IDisposable
    {
        private const string TestAddress = "tcp://localhost:55522";

        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly ITickRepository repository;
        private readonly IDataSerializer<Tick> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly TickProvider provider;

        public TickProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.dataSerializer = new TickDataSerializer();
            this.repository = new MockTickRepository(container, this.dataSerializer, DataBusFactory.Create(container));
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();

            this.provider = new TickProvider(
                container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                new NetworkPort(55522));
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
        }

        [Fact]
        internal void GivenTickDataRequest_WithNoTicks_ReturnsQueryFailedMessage()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var requester = new RequestSocket();
            requester.Connect(TestAddress);
            Task.Delay(100).Wait();  // Allow socket to connect

            var query = new Dictionary<string, string>
            {
                { "DataType", "Tick[]" },
                { "Symbol", symbol.Value },
                { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "Limit", "0" },
            };

            var dataRequest = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(dataRequest));
            var response = (QueryFailure)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down
            this.provider.Stop();
            requester.Disconnect(TestAddress);
            requester.Close();
        }

        [Fact]
        internal void GivenTickDataRequest_WithTicks_ReturnsValidTickDataResponse()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();  // Allow provider to start

            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = new Tick(symbol, Price.Create(1.00000m), Price.Create(1.00000m), Volume.One(), Volume.One(), datetimeFrom);
            var tick2 = new Tick(symbol, Price.Create(1.00010m), Price.Create(1.00020m), Volume.One(), Volume.One(), datetimeTo);

            this.repository.Add(tick1);
            this.repository.Add(tick2);

            var requester = new RequestSocket();
            requester.Connect(TestAddress);
            Task.Delay(100).Wait();  // Allow socket to connect

            var query = new Dictionary<string, string>
            {
                { "DataType", "Tick[]" },
                { "Symbol", symbol.Value },
                { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
                { "Limit", "0" },
            };

            var dataRequest = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(dataRequest));
            var response = (DataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            var ticks = this.dataSerializer.DeserializeBlob(response.Data);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(2, ticks.Length);
            Assert.Equal(tick1, ticks[0]);
            Assert.Equal(tick2, ticks[1]);

            // Tear Down
            this.provider.Stop();
            requester.Disconnect(TestAddress);
            requester.Close();
        }
    }
}
