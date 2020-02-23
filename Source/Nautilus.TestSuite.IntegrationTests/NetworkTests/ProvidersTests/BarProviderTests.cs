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
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Data.Providers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Network.Compression;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NetMQ;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarProviderTests : IDisposable
    {
        private readonly IComponentryContainer container;
        private readonly IBarRepository repository;
        private readonly IDataSerializer<Bar> dataSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;

        public BarProviderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.dataSerializer = new BarDataSerializer();
            this.repository = new MockBarRepository(this.dataSerializer);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
            this.compressor = new CompressorBypass();
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
        }

        [Fact]
        internal void GivenBarDataRequest_WithNoBars_ReturnsQueryFailedMessage()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55523));

            var provider = new BarProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start().Wait();

            var requester = new MockRequester(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

            var barType = StubBarType.AUDUSD_OneMinuteAsk();

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
        internal void GivenBarDataRequest_WithBars_ReturnsValidBarDataResponse()
        {
            // Arrange
            var testAddress = new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(55524));

            var provider = new BarProvider(
                this.container,
                this.repository,
                this.dataSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress.Port);
            provider.Start().Wait();

            var requester = new MockRequester(
                this.container,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            requester.Start().Wait();

            var barType = StubBarType.AUDUSD_OneMinuteAsk();
            var bar1 = StubBarProvider.Build();
            var bar2 = StubBarProvider.Build();

            this.repository.Add(barType, bar1);
            this.repository.Add(barType, bar2);

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
            var response = (DataResponse)requester.Send(request);
            var bars = this.dataSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(2, bars.Length);
            Assert.Equal(bar1, bars[0]);
            Assert.Equal(bar2, bars[1]);

            // Tear Down
            requester.Stop().Wait();
            requester.Dispose();
            provider.Stop().Wait();
            provider.Dispose();
        }
    }
}
