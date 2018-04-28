//--------------------------------------------------------------
// <copyright file="TrailingStopSignalProcessorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.ProcessorsTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Messaging.MessageStore;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Portfolio.Processors;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TrailingStopSignalProcessorTests
    {
        private readonly ITestOutputHelper output;
        private readonly MessageWarehouse messageWarehouse;
        private readonly TrailingStopSignalProcessor trailingStopSignalProcessor;
        private readonly TradeBook tradeBook;

        public TrailingStopSignalProcessorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var instrument = StubInstrumentFactory.AUDUSD();
            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();

            var testActorSystem = ActorSystem.Create(nameof(TrailingStopSignalProcessorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                NautilusEnvironment.Live,
                setupContainer.Clock,
                setupContainer.LoggerFactory);

            this.messageWarehouse = messagingServiceFactory.MessageWarehouse;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.tradeBook = new TradeBook(setupContainer, instrument.Symbol);

            this.trailingStopSignalProcessor = new TrailingStopSignalProcessor(setupContainer, messagingAdapter, instrument, this.tradeBook);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenTradeInitialized_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.80050m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Initialized, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenSignalAtSameTimeAsEntry_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.80050m, 0.00001m) } },
                Period.Zero);

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenSignalMarketPositionInvalid_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.80050m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenForUnitDoesNotExist_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 4, Price.Create(0.80050m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenLongSignalValid_ThenSendsModifyStoplossCommand()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.80150m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            CustomAssert.EventuallyContains(
                typeof(ModifyStopLoss),
                this.messageWarehouse.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.TimeoutMilliseconds);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenShortSignalValid_ThenSendsModifyStoplossCommand()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.ShortTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(1.20000m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            CustomAssert.EventuallyContains(
                typeof(ModifyStopLoss),
                this.messageWarehouse.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.TimeoutMilliseconds);
        }

        [Fact]
        internal void GivenTrailingStopSignalWrapper_WhenModifyStopPriceBeyondCurrentStopPriceWithLongPosition_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.LongTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(0.79000m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Task.Delay(300).Wait();
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }

        [Fact] // TODO: erratic test? FAILED 1/08/2017 at 10:19PM
        internal void GivenTrailingStopSignalWrapper_WhenModifyStopPriceBeyondCurrentStopPriceWithShortPosition_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            var trailingStopSignal = StubSignalBuilder.ShortTrailingStopSignal(
                new TradeType("TestTrade"),
                new Dictionary<int, Price> { { 0, Price.Create(1.31000m, 0.00001m) } },
                Period.FromMinutes(1));

            // Act
            this.trailingStopSignalProcessor.Process(trailingStopSignal);

            // Assert
            Task.Delay(300).Wait();
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"))[0].MarketPosition);
            Assert.Equal(0, this.messageWarehouse.CommandEnvelopes.Count);
        }
    }
}
