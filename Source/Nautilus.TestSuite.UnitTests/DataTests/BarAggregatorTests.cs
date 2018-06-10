//--------------------------------------------------------------------------------------------------
// <copyright file="TimeBarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Enums;
    using Nautilus.Data;
    using Nautilus.Data.Messages;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregatorTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly Symbol symbol;
        private readonly IActorRef barAggregatorRef;

        public BarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            this.symbol = new Symbol("AUDUSD", Exchange.FXCM);
            var setupFactory = new StubSetupContainerFactory();
            var container = setupFactory.Create();
            this.logger = setupFactory.LoggingAdapter;

            var props = Props.Create(() => new BarAggregator(
                container,
                ServiceContext.Database,
                this.symbol));

            this.barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenNoSubscriptions_ThenDoesNothing()
        {
            // Arrange
            var closeBarMessage = new CloseBar(
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            barAggregatorRef.Tell(closeBarMessage);

            // Assert
            ExpectNoMsg();
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscriptionButNoTicks_ThenDoesNothing()
        {
            // Arrange
            var subscribeMessage = new SubscribeBarData(
                this.symbol,
                new List<BarSpecification>
                {
                    new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1)
                },
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var closeBarMessage = new CloseBar(
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregatorRef.Tell(subscribeMessage);

            // Act
            this.barAggregatorRef.Tell(closeBarMessage);

            // Assert
            ExpectNoMsg();
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenDifferentSubscription_ThenDoesNothing()
        {
            // Arrange
            var subscribeMessage = new SubscribeBarData(
                this.symbol,
                new List<BarSpecification>
                {
                    new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1)
                },
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage = new CloseBar(
                new BarSpecification(BarQuoteType.Ask, BarResolution.Second, 10),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregatorRef.Tell(subscribeMessage);
            this.barAggregatorRef.Tell(tick);

            // Act
            this.barAggregatorRef.Tell(closeBarMessage);

            // Assert
            ExpectNoMsg();
        }

        [Fact]
        internal void GivenCloseBarMessage_WhenMatchingSubscription_ThenReturnsExpectedBar()
        {
            // Arrange
            var subscribeMessage = new SubscribeBarData(
                this.symbol,
                new List<BarSpecification>
                {
                    new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1)
                },
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = new Tick(
                this.symbol,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            var closeBarMessage = new CloseBar(
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.barAggregatorRef.Tell(subscribeMessage);
            this.barAggregatorRef.Tell(tick);

            // Act
            this.barAggregatorRef.Tell(closeBarMessage);

            // Assert
            var result = ExpectMsg<Bar>();
            Assert.Equal(0.80000m, result.Open.Value);
            Assert.Equal(0.80000m, result.Close.Value);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), result.Timestamp);
        }
    }
}
