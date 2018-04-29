//--------------------------------------------------------------
// <copyright file="TimeBarAggregatorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests.MarketTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TimeBarAggregatorTests
    {
        private readonly ITestOutputHelper output;
        private readonly ActorSystem testActorSystem;
        private readonly BlackBoxSetupContainer setupContainer;
        private readonly MockLogger mockLogger;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly Symbol symbol;

        public TimeBarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            this.testActorSystem = ActorSystem.Create(nameof(TimeBarAggregatorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                this.testActorSystem,
                this.setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            this.messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.symbol = new Symbol("AUDUSD", Exchange.LMAX);
        }

        [Fact]
        internal void OnFirstTick_LogsExpected()
        {
            // Arrange
            var message = new SubscribeSymbolDataType(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 5),
                new TradeType("TestScalp"),
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TimeBarAggregator(
                this.setupContainer,
                this.messagingAdapter,
                message)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now() + Duration.FromMinutes(1));

            // Act
            barAggregatorRef.Tell(quote1);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains(
//                "TimeBarAggregator-AUDUSD.LMAX(TestScalp): Registered for Minute(5) bars quoteBarStart=1970-01-01T00:01:01.000Z, quoteBarEnd=1970-01-01T00:05:00.000Z",
//                this.mockLogger,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
//
//            CustomAssert.EventuallyContains(
//                "TimeBarAggregator-AUDUSD.LMAX(TestScalp): Receiving quotes (AUDUSD) from LMAX...",
//                this.mockLogger,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenTickMessages_WhenSecondBar_ReturnsValidBar()
        {
            // Arrange
            var message = new SubscribeSymbolDataType(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 30),
                new TradeType("TestScalp"),
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TimeBarAggregator(
                this.setupContainer,
                this.messagingAdapter,
                message)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(10));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(20));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(30));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<MarketDataEvent>(
//                typeof(MarketDataEvent),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
//
//            var marketDataEvent = this.messageWarehouse.EventEnvelopes[0].Open(StubDateTime.Now()).Event.AsInstanceOf<MarketDataEvent>();
//
//            Assert.Equal(StubDateTime.Now() - Duration.FromSeconds(1) + Duration.FromSeconds(30), marketDataEvent?.Bar.Timestamp);
        }

        [Fact]
        internal void GivenTickMessages_WhenMinuteBar_ReturnsValidBar()
        {
            // Arrange
            var message = new SubscribeSymbolDataType(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 5),
                new TradeType("TestScalp"),
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TimeBarAggregator(
                this.setupContainer,
                this.messagingAdapter,
                message)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(2));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(4));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(5));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<MarketDataEvent>(
//                typeof(MarketDataEvent),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
//
//            var marketDataEvent = this.messageWarehouse.EventEnvelopes[0].Open(StubDateTime.Now()).Event.AsInstanceOf<MarketDataEvent>();
//
//            Assert.Equal(StubDateTime.Now() - Duration.FromSeconds(1) + Duration.FromMinutes(5), marketDataEvent?.Bar.Timestamp);
//            Assert.Equal(Quantity.Create(5), marketDataEvent?.Bar.Volume);
        }

        [Fact]
        internal void GivenTickMessages_WhenHourBar_ReturnsValidBar()
        {
            // Arrange
            var message = new SubscribeSymbolDataType(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Hour, 1),
                new TradeType("TestScalp"),
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TimeBarAggregator(
                this.setupContainer,
                this.messagingAdapter,
                message)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(10));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(20));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(30));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(40));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(60));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<MarketDataEvent>(
//                typeof(MarketDataEvent),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
//
//            var marketDataEvent = this.messageWarehouse.EventEnvelopes[0].Open(StubDateTime.Now()).Event.AsInstanceOf<MarketDataEvent>();
//
//            Assert.Equal(StubDateTime.Now() - Duration.FromSeconds(1) + Duration.FromMinutes(60), marketDataEvent?.Bar.Timestamp);
//            Assert.Equal(Quantity.Create(5), marketDataEvent?.Bar.Volume);
        }
    }
}
