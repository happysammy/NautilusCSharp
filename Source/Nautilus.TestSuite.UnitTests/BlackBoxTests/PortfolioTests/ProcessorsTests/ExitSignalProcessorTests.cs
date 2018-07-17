//--------------------------------------------------------------------------------------------------
// <copyright file="ExitSignalProcessorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.ProcessorsTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Portfolio.Processors;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExitSignalProcessorTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly ExitSignalProcessor exitSignalProcessor;
        private readonly TradeBook tradeBook;

        public ExitSignalProcessorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var instrument = StubInstrumentFactory.AUDUSD();
            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(ExitSignalProcessorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.tradeBook = new TradeBook(setupContainer, instrument.Symbol);

            this.exitSignalProcessor = new ExitSignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook);
        }

        [Fact]
        internal void GivenExitSignal_WhenValidExitSignalWithTradeInitialized_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);
            var exitSignal = StubSignalBuilder.LongExitSignalForAllUnits(new TradeType("TestTrade"), Period.FromMinutes(1));

            // Act
            this.exitSignalProcessor.Process(exitSignal);
            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Initialized, result[0].TradeStatus);
            Assert.Equal(0, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenExitSignal_WhenExitSignalForWrongMarketPosition_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            var exitSignal = StubSignalBuilder.ShortExitSignalForAllUnits(new TradeType("TestTrade"), Period.FromMinutes(1));

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, result[0].MarketPosition);
            Assert.Equal(0, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenExitSignal_WhenExitSignalAtSameTimeAsTradeEntryTimestamp_ThenDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            // Both trade timestamp and exit timestamp are stubbed.TimeNow().
            var exitSignal = StubSignalBuilder.LongExitSignalForAllUnits(new TradeType("TestTrade"), Period.Zero);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, result[0].MarketPosition);
            Assert.Equal(0, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenExitSignal_WhenValidLongSignal_ThenReturnsClosePosition()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            var exitSignal = StubSignalBuilder.LongExitSignalForAllUnits(new TradeType("TestTrade"), Period.FromMinutes(1));

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, result[0].MarketPosition);
            CustomAssert.EventuallyContains(
                typeof(ClosePosition),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenExitSignal_WhenValidSignalShort_ThenReturnsClosePosition()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);

            var exitSignal = StubSignalBuilder.ShortExitSignalForAllUnits(new TradeType("TestTrade"), Period.FromMinutes(1));

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, result[0].MarketPosition);
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                typeof(ClosePosition),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenExitSignal_WhenForAUnitWhichIsNotActive_ThenReturnsNoResponse()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);

            var exitSignal = new ExitSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("Test"),
                new Label("TestSignal"),
                new TradeType("TestTrade"),
                MarketPosition.Short,
                new List<int> { 3 },
                StubZonedDateTime.UnixEpoch());

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, result[0].MarketPosition);
            Assert.Equal(0, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void GivenExitSignal_WhenMultipleForUnits_ThenResultsInClosePositionCommands()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            var exitSignal = new ExitSignal(
                new Symbol("SYMBOL", Venue.GLOBEX),
                new EntityId("Test"),
                new Label("Test"),
                new TradeType("TestTrade"),
                MarketPosition.Long,
                new List<int> { 0 },
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration());

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].Entry));

            this.exitSignalProcessor.Process(exitSignal);

            var result = this.tradeBook.GetTradesByTradeType(new TradeType("TestTrade"));

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(TradeStatus.Active, result[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, result[0].MarketPosition);
            CustomAssert.EventuallyContains(
                typeof(ClosePosition),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}
