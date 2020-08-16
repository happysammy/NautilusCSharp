// -------------------------------------------------------------------------------------------------
// <copyright file="TickProviderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Common.Data;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Extensions;
using Nautilus.Data.Interfaces;
using Nautilus.Data.Messages.Requests;
using Nautilus.Data.Messages.Responses;
using Nautilus.Data.Providers;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Network.Messages;
using Nautilus.Serialization.DataSerializers;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Mocks;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.DataTests.ProvidersTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickProviderTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;
        private readonly IMessageBusAdapter messagingAdapter;
        private readonly ITickRepository repository;
        private readonly QuoteTickSerializer tickSerializer;

        public TickProviderTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.messagingAdapter = new MockMessageBusProvider(this.container).Adapter;
            this.tickSerializer = new QuoteTickSerializer();
            this.repository = new MockMarketDataRepository(
                this.container,
                this.tickSerializer,
                new TradeTickSerializer(),
                new BarSerializer(),
                DataBusFactory.Create(this.container));
        }

        [Fact]
        internal void GivenTickDataRequest_WithNoTicks_ReturnsQueryFailedMessage()
        {
            // Arrange
            var provider = new TickProvider(
                this.container,
                this.messagingAdapter,
                this.repository,
                this.tickSerializer,
                new TradeTickSerializer());
            provider.Start().Wait();

            var symbol = new Symbol("AUD/USD", new Venue("FXCM"));

            var query = new Dictionary<string, string>
            {
                { "DataType", "Tick[]" },
                { "Symbol", symbol.Value },
                { "FromDateTime", StubZonedDateTime.UnixEpoch().ToIso8601String() },
                { "ToDateTime", StubZonedDateTime.UnixEpoch().ToIso8601String() },
                { "Limit", "0" },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var response = (QueryFailure)provider.FindData(request);

            // Assert
            Assert.Equal(typeof(QueryFailure), response.Type);
        }

        [Fact]
        internal void GivenTickDataRequest_WithTicks_ReturnsValidTickDataResponse()
        {
            // Arrange
            var provider = new TickProvider(
                this.container,
                this.messagingAdapter,
                this.repository,
                this.tickSerializer,
                new TradeTickSerializer());
            provider.Start().Wait();

            var datetimeFrom = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var datetimeTo = datetimeFrom + Duration.FromMinutes(1);

            var symbol = new Symbol("AUD/USD", new Venue("FXCM"));
            var tick1 = new QuoteTick(symbol, Price.Create(1.00000m), Price.Create(1.00000m), Quantity.One(), Quantity.One(), datetimeFrom);
            var tick2 = new QuoteTick(symbol, Price.Create(1.00010m), Price.Create(1.00020m), Quantity.One(), Quantity.One(), datetimeTo);

            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);

            var query = new Dictionary<string, string>
            {
                { "DataType", "Tick[]" },
                { "Symbol", symbol.Value },
                { "FromDateTime", StubZonedDateTime.UnixEpoch().ToIso8601String() },
                { "ToDateTime", StubZonedDateTime.UnixEpoch().ToIso8601String() },
                { "Limit", "0" },
            };

            var request = new DataRequest(
                query,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var response = (DataResponse)provider.FindData(request);

            var ticks = this.tickSerializer.DeserializeBlob(response.Data);

            // Assert
            Assert.Equal(typeof(DataResponse), response.Type);
            Assert.Equal(2, ticks.Length);
            Assert.Equal(tick1, ticks[0]);
            Assert.Equal(tick2, ticks[1]);
        }
    }
}
