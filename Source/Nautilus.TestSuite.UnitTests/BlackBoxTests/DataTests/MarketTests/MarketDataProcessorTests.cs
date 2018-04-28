//--------------------------------------------------------------
// <copyright file="MarketDataProcessorTests.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.Common.Enums;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MarketDataProcessorTests
    {
        private readonly ITestOutputHelper output;
        private readonly IActorRef marketDataProcessorRef;
        private readonly MockLogger mockLogger;
        private readonly MessageWarehouse messageWarehouse;
        private readonly Symbol symbol;

        public MarketDataProcessorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(MarketDataProcessorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.messageWarehouse = messagingServiceFactory.MessageWarehouse;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.symbol = new Symbol("AUDUSD", Exchange.LMAX);

            this.marketDataProcessorRef = testActorSystem.ActorOf(Props.Create(() => new MarketDataProcessor(
                setupContainer,
                messagingAdapter,
                this.symbol)));
        }

        [Fact]
        internal void GivenSubscribeSymbolDataTypeMessage_WithMinuteBarSpecification_SetsUpBarAggregator()
        {
            // Arrange
            var BarSpecification = new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5);
            var tradeType = new TradeType("TestScalp");
            var message = new SubscribeSymbolDataType(
                this.symbol,
                BarSpecification,
                tradeType,
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.marketDataProcessorRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Setup for Minute(5) bars",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            CustomAssert.EventuallyContains(
                "TimeBarAggregator-AUDUSD.LMAX(TestScalp): Nautilus.BlackBox.Data.Market.TimeBarAggregator initializing...",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenSubscribeSymbolDataTypeMessage_WithTickBarSpecification_SetsUpBarAggregator()
        {
            // Arrange
            var BarSpecification = new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 1000);
            var tradeType = new TradeType("TestScalp");
            var message = new SubscribeSymbolDataType(
                this.symbol,
                BarSpecification,
                tradeType,
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.marketDataProcessorRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Setup for Tick(1000) bars",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            CustomAssert.EventuallyContains(
                "TickBarAggregator-AUDUSD.LMAX(TestScalp): Nautilus.BlackBox.Data.Market.TickBarAggregator initializing...",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnsubscribeSymbolDataTypeMessage_RemovesBarAggregator()
        {
            // Arrange
            var BarSpecification = new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 1000);
            var tradeType = new TradeType("TestScalp");
            var message1 = new SubscribeSymbolDataType(
                this.symbol,
                BarSpecification,
                tradeType,
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());
            var message2 = new UnsubscribeSymbolDataType(
                this.symbol,
                tradeType,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.marketDataProcessorRef.Tell(message1);
            this.marketDataProcessorRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Data for AUDUSD.LMAX(TestScalp) bars deregistered",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}
