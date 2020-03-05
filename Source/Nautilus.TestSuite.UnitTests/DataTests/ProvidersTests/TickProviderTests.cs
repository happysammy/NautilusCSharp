// -------------------------------------------------------------------------------------------------
// <copyright file="TickProviderTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.ProvidersTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network.Compression;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickProviderTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;
        private readonly IMessageBusAdapter messagingAdapter;
        private readonly ITickRepository repository;
        private readonly IDataSerializer<Tick> dataSerializer;
        private readonly ISerializer<Dictionary<string, string>> headerSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;

        public TickProviderTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.messagingAdapter = new MockMessageBusProvider(this.container).Adapter;
            this.dataSerializer = new TickDataSerializer();
            this.repository = new MockTickRepository(this.container, this.dataSerializer, DataBusFactory.Create(this.container));
            this.headerSerializer = new MsgPackDictionarySerializer();
            this.requestSerializer = new MsgPackRequestSerializer();
            this.responseSerializer = new MsgPackResponseSerializer();
            this.compressor = new CompressorBypass();
        }

        // [Fact]
        // internal void GivenTickDataRequest_WithNoTicks_ReturnsQueryFailedMessage()
        // {
        //     // Arrange
        //     var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55722));
        //
        //     var provider = new TickProvider(
        //         this.container,
        //         this.messagingAdapter,
        //         this.repository,
        //         this.dataSerializer);
        //     provider.Start().Wait();
        //
        //     var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
        //
        //     var query = new Dictionary<string, string>
        //     {
        //         { "DataType", "Tick[]" },
        //         { "Symbol", symbol.Value },
        //         { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
        //         { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
        //         { "Limit", "0" },
        //     };
        //
        //     var request = new DataRequest(
        //         query,
        //         Guid.NewGuid(),
        //         StubZonedDateTime.UnixEpoch());
        //
        //     // Act
        //     var response = (QueryFailure)requester.Send(request);
        //
        //     // Assert
        //     Assert.Equal(typeof(QueryFailure), response.Type);
        //
        //     // Tear Down
        //     requester.Stop().Wait();
        //     requester.Dispose();
        //     provider.Stop().Wait();
        // }

        // [Fact]
        // internal void GivenTickDataRequest_WithTicks_ReturnsValidTickDataResponse()
        // {
        //     // Arrange
        //     var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55723));
        //
        //     var provider = new TickProvider(
        //         this.container,
        //         this.messagingAdapter,
        //         this.repository,
        //         this.dataSerializer);
        //     provider.Start().Wait();
        //
        //     var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
        //     var datetimeTo = datetimeFrom + Duration.FromMinutes(1);
        //
        //     var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
        //     var tick1 = new Tick(symbol, Price.Create(1.00000m), Price.Create(1.00000m), Volume.One(), Volume.One(), datetimeFrom);
        //     var tick2 = new Tick(symbol, Price.Create(1.00010m), Price.Create(1.00020m), Volume.One(), Volume.One(), datetimeTo);
        //
        //     this.repository.Add(tick1);
        //     this.repository.Add(tick2);
        //
        //     var query = new Dictionary<string, string>
        //     {
        //         { "DataType", "Tick[]" },
        //         { "Symbol", symbol.Value },
        //         { "FromDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
        //         { "ToDate", new DateKey(StubZonedDateTime.UnixEpoch()).ToString() },
        //         { "Limit", "0" },
        //     };
        //
        //     var request = new DataRequest(
        //         query,
        //         Guid.NewGuid(),
        //         StubZonedDateTime.UnixEpoch());
        //
        //     // Act
        //     var response = (DataResponse)requester.Send(request);
        //
        //     var ticks = this.dataSerializer.DeserializeBlob(response.Data);
        //
        //     // Assert
        //     Assert.Equal(typeof(DataResponse), response.Type);
        //     Assert.Equal(2, ticks.Length);
        //     Assert.Equal(tick1, ticks[0]);
        //     Assert.Equal(tick2, ticks[1]);
        //
        //     // Tear Down
        //     requester.Stop().Wait();
        //     requester.Dispose();
        //     provider.Stop().Wait();
        // }
    }
}
