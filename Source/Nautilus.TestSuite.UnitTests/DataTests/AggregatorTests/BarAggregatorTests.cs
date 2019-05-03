//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Data.Aggregators;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregatorTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly Symbol symbol;
        private readonly BarAggregator barAggregator;

        public BarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            this.symbol = new Symbol("AUDUSD", Venue.FXCM);
            var setupFactory = new StubComponentryContainerFactory();
            var container = setupFactory.Create();
            this.logger = setupFactory.LoggingAdapter;

            this.barAggregator = new BarAggregator(container, this.symbol, false);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenNoSubscriptions_ThenDoesNothing()
        {
            // Arrange
            var closeBarMessage = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage);

            // Assert
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscriptionButNoTicks_ThenDoesNothing()
        {
            // Arrange
            var subscribeMessage = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var closeBarMessage = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribeMessage);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage);

            // Assert
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenDifferentSubscription_ThenDoesNothing()
        {
            // Arrange
            var subscribeMessage = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage = new CloseBar(
                new BarSpecification(QuoteType.Ask, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribeMessage);
            this.barAggregator.Endpoint.Send(tick);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage);

            // Assert
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscription1_ThenReturnsExpectedBar()
        {
            // Arrange
            var subscribeMessage = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribeMessage);
            this.barAggregator.Endpoint.Send(tick);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage);

            // Assert
//            var result = this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
//            Assert.Equal(0.80000m, result.Bar.Open.Value);
//            Assert.Equal(0.80000m, result.Bar.Close.Value);
//            Assert.Equal(tick, result.LastTick);
//            Assert.Equal(StubZonedDateTime.UnixEpoch(), result.Timestamp);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscription2_ThenReturnsNextBar()
        {
            // Arrange
            var subscribeMessage = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage1 = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80100m, 5),
                Price.Create(0.80110m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage2 = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(1),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribeMessage);
            this.barAggregator.Endpoint.Send(tick1);
            this.barAggregator.Endpoint.Send(closeBarMessage1);
            // this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
            this.barAggregator.Endpoint.Send(tick2);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage2);

            // Assert
            // var result = this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
//            Assert.Equal(0.80000m, result.Bar.Open.Value);
//            Assert.Equal(0.80100m, result.Bar.Close.Value);
//            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(1), result.Timestamp);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMultipleSubscriptions1_ThenReturnsExpectedBar()
        {
            // Arrange
            var subscribeMessage1 = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribeMessage2 = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Bid, Resolution.Second, 10)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80005m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(10));

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80100m, 5),
                Price.Create(0.80105m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage1 = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick3 = new Tick(
                this.symbol,
                Price.Create(0.80200m, 5),
                Price.Create(0.80210m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(100));

            var closeBarMessage2 = new CloseBar(
                new BarSpecification(QuoteType.Bid, Resolution.Second, 10),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(10),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribeMessage1);
            this.barAggregator.Endpoint.Send(subscribeMessage2);
            this.barAggregator.Endpoint.Send(tick1);
            this.barAggregator.Endpoint.Send(tick2);
            this.barAggregator.Endpoint.Send(closeBarMessage1);

            // this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
            this.barAggregator.Endpoint.Send(tick3);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage2);

            // Assert
//            var result = this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
//            Assert.Equal(0.80000m, result.Bar.Open.Value);
//            Assert.Equal(0.80200m, result.Bar.High.Value);
//            Assert.Equal(0.80200m, result.Bar.Close.Value);
//            Assert.Equal(tick3, result.LastTick);
//            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(10), result.Timestamp);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMidBarSubscribed_ThenReturnsNextBar()
        {
            // Arrange
            var subscribe = new Subscribe<BarType>(
                new BarType(
                    this.symbol,
                    new BarSpecification(QuoteType.Mid, Resolution.Second, 1)),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1));

            var tick2 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 5),
                Price.Create(0.80070m, 5),
                StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1001));

            var closeBarMessage = new CloseBar(
                new BarSpecification(QuoteType.Mid, Resolution.Second, 1),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(1),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregator.Endpoint.Send(subscribe);
            this.barAggregator.Endpoint.Send(tick1);
            this.barAggregator.Endpoint.Send(tick2);

            // Act
            this.barAggregator.Endpoint.Send(closeBarMessage);

            // Assert
//            var result = this.ExpectMsg<BarClosed>(TimeSpan.FromMilliseconds(100));
//            Assert.Equal(0.80005m, result.Bar.Open.Value);
//            Assert.Equal(0.80035m, result.Bar.High.Value);
//            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(1), result.Timestamp);
        }
    }
}
