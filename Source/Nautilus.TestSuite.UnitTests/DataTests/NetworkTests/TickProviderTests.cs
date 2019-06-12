// -------------------------------------------------------------------------------------------------
// <copyright file="TickProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.NetworkTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Network;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public sealed class TickProviderTests
    {
        private const string TEST_ADDRESS = "tcp://localhost:55522";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockMessagingAgent receiver;
        private readonly ITickRepository repository;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly TickProvider provider;

        public TickProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            var container = setupFactory.Create();
            this.loggingAdapter = setupFactory.LoggingAdapter;
            this.requestSerializer = new MsgPackRequestSerializer();
            this.responseSerializer = new MsgPackResponseSerializer();
            this.receiver = new MockMessagingAgent();
            var dataBusAdapter = DataBusFactory.Create(container);
            this.repository = new InMemoryTickStore(container, dataBusAdapter);
            this.provider = new TickProvider(
                container,
                this.repository,
                this.requestSerializer,
                this.responseSerializer,
                NetworkAddress.LocalHost,
                new NetworkPort(55522));
        }

        [Fact]
        internal void GivenTickDataRequest_WithNoTicks_ReturnsQueryFailedMessage()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();

            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUDUSD", Venue.FXCM);

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var dataRequest = new TickDataRequest(
                symbol,
                datetimeFrom,
                datetimeTo,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(dataRequest));
            var response = (QueryFailure)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.Dump(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }

        [Fact]
        internal void GivenTickDataRequest_WithTicks_TickDataResponse()
        {
            // Arrange
            this.provider.Start();
            Task.Delay(100).Wait();

            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var tick1 = new Tick(symbol, 1.00000m, 1.00000m, datetimeFrom);
            var tick2 = new Tick(symbol, 1.00010m, 1.00020m, datetimeTo);

            this.repository.Add(tick1);
            this.repository.Add(tick2);

            var requester = new RequestSocket();
            requester.Connect(TEST_ADDRESS);
            Task.Delay(100).Wait();  // Allow socket to connect

            var dataRequest = new TickDataRequest(
                symbol,
                datetimeFrom,
                datetimeTo,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            requester.SendFrame(this.requestSerializer.Serialize(dataRequest));
            var response = (TickDataResponse)this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.Dump(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(TickDataResponse), response.Type);

            // Tear Down;
            requester.Disconnect(TEST_ADDRESS);
            requester.Dispose();
            this.provider.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
