//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Data.Aggregation;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarAggregatorTests
    {
        private readonly MockComponent receiver;
        private readonly BarAggregator barAggregator;
        private readonly Symbol symbol;

        public BarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            this.receiver = new MockComponent(container);
            this.receiver.RegisterHandler<BarData>(this.receiver.OnMessage);
            this.symbol = new Symbol("AUDUSD", new Venue("FXCM"));

            this.barAggregator = new BarAggregator(
                container,
                this.receiver.Endpoint,
                this.symbol);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenNoSubscriptions_ThenDoesNothing()
        {
            // Arrange
            var closeBar = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Empty(this.barAggregator.Specifications);
            Assert.Empty(this.receiver.Messages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenSubscribeBarTypeMessage_WhenNoPreviousSubscriptions_ThenSubscribes()
        {
            // Arrange
            var barSpec = new BarSpecification(1, BarStructure.Second, PriceType.Bid);
            var subscribe = new Subscribe<BarType>(
                new BarType(this.symbol, barSpec),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.barAggregator.Endpoint.SendAsync(subscribe).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.barAggregator.Specifications);
            Assert.Contains(barSpec, this.barAggregator.Specifications);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenSubscribeBarTypeMessage_WhenPreviousSubscription_ThenDoesNotSubscribeAgain()
        {
            // Arrange
            var barSpec = new BarSpecification(1, BarStructure.Second, PriceType.Bid);
            var subscribe = new Subscribe<BarType>(
                new BarType(new Symbol("AUDUSD", new Venue("FXCM")), barSpec),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.barAggregator.Endpoint.SendAsync(subscribe);
            this.barAggregator.Endpoint.SendAsync(subscribe).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.barAggregator.Specifications);
            Assert.Contains(barSpec, this.barAggregator.Specifications);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscriptionButNoTicks_ThenDoesNothing()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var closeBar = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Empty(this.receiver.Messages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenDifferentSubscription_ThenDoesNothing()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBar = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Ask),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe);
            this.barAggregator.Endpoint.SendAsync(tick);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Empty(this.receiver.Messages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscription1_ThenReturnsExpectedBar()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBar = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe);
            this.barAggregator.Endpoint.SendAsync(tick);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.receiver.Messages);
            Assert.Equal("BarData(AUDUSD.FXCM-1-SECOND-BID, 0.80000,0.80000,0.80000,0.80000,1,1970-01-01T00:00:00.000Z)", this.receiver.Messages[0].ToString());
            Assert.Empty(this.receiver.UnhandledMessages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscription2_ThenReturnsNextBar()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBar1 = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80100m, 5),
                Price.Create(0.80110m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBar2 = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe);
            this.barAggregator.Endpoint.SendAsync(tick1);
            this.barAggregator.Endpoint.SendAsync(closeBar1);
            this.barAggregator.Endpoint.SendAsync(tick2);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar2).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Equal(2, this.receiver.Messages.Count);
            Assert.Equal("BarData(AUDUSD.FXCM-1-SECOND-BID, 0.80000,0.80000,0.80000,0.80000,1,1970-01-01T00:00:00.000Z)", this.receiver.Messages[0].ToString());
            Assert.Empty(this.receiver.UnhandledMessages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMultipleSubscriptions_ThenReturnsExpectedBar()
        {
            // Arrange
            var subscribe1 = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(10, BarStructure.Second, PriceType.Bid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(10));

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80100m, 5),
                Price.Create(0.80105m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBar1 = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                this.symbol,
                Price.Create(0.80200m, 5),
                Price.Create(0.80210m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(100));

            var closeBar2 = new CloseBar(
                new BarSpecification(10, BarStructure.Second, PriceType.Bid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe1);
            this.barAggregator.Endpoint.SendAsync(subscribe2);
            this.barAggregator.Endpoint.SendAsync(tick1);
            this.barAggregator.Endpoint.SendAsync(tick2);
            this.barAggregator.Endpoint.SendAsync(closeBar1);
            this.barAggregator.Endpoint.SendAsync(tick3);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar2).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Equal(2, this.receiver.Messages.Count);
            Assert.Equal("BarData(AUDUSD.FXCM-1-SECOND-BID, 0.80000,0.80100,0.80000,0.80100,2,1970-01-01T00:00:00.000Z)", this.receiver.Messages[0].ToString());
            Assert.Equal("BarData(AUDUSD.FXCM-10-SECOND-BID, 0.80000,0.80200,0.80000,0.80200,3,1970-01-01T00:00:00.000Z)", this.receiver.Messages[1].ToString());
            Assert.Empty(this.receiver.UnhandledMessages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMidBarSubscribed_ThenReturnsNextBar()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(1, BarStructure.Second, PriceType.Mid)),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1));

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80070m, 5),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1001));

            var closeBar = new CloseBar(
                new BarSpecification(1, BarStructure.Second, PriceType.Mid),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.SendAsync(subscribe);
            this.barAggregator.Endpoint.SendAsync(tick1);
            this.barAggregator.Endpoint.SendAsync(tick2);

            // Act
            this.barAggregator.Endpoint.SendAsync(closeBar).Wait();
            this.barAggregator.Stop().Wait();
            this.receiver.Stop().Wait();

            // Assert
            Assert.Single(this.receiver.Messages);
            Assert.Equal("BarData(AUDUSD.FXCM-1-SECOND-MID, 0.800050,0.800350,0.800050,0.800350,2,1970-01-01T00:00:00.000Z)", this.receiver.Messages[0].ToString());
            Assert.Empty(this.receiver.UnhandledMessages);
            Assert.Empty(this.barAggregator.UnhandledMessages);
        }
    }
}
