//--------------------------------------------------------------
// <copyright file="EntrySignalProcessorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.ProcessorsTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.BlackBox.Portfolio.Processors;
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
    public class EntrySignalProcessorTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly EntrySignalProcessor entrySignalProcessor;
        private readonly TradeBook tradeBook;

        public EntrySignalProcessorTests(ITestOutputHelper output)
        {
            this.output = output;
            var instrument = StubInstrumentFactory.AUDUSD();
            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            setupFactory.QuoteProvider.OnQuote(
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.80001m, 0.00001m),
                    Price.Create(0.80005m, 0.00001m),
                    StubDateTime.Now()));

            var testActorSystem = ActorSystem.Create(nameof(EntrySignalProcessorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.tradeBook = new TradeBook(setupContainer, instrument.Symbol);

            this.entrySignalProcessor = new EntrySignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                this.tradeBook);
        }

        [Fact]
        internal void GivenEntrySignalWrapper_WhenValidBuySignalTradeStatusNone_ThenSendsRequestTradeApprovalMessage()
        {
            // Arrange
            var entrySignal = StubSignalBuilder.BuyEntrySignal();

            // Act
            this.entrySignalProcessor.Process(entrySignal);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                typeof(RequestTradeApproval),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            var requestTradeApproval = this.inMemoryMessageStore.CommandEnvelopes[0].Open(StubDateTime.Now()) as RequestTradeApproval;

            Assert.Equal(2, requestTradeApproval?.OrderPacket.Orders.Count);
        }

        [Fact]
        internal void GivenEntrySignal_WhenTradeStatusPending_ThenIgnoresSignalDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.SellThreeUnits();
            this.tradeBook.AddTrade(trade);
            this.tradeBook.AddTrade(trade);

            // Act
            var eventMessage = StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry);
            trade.Apply(eventMessage);
            trade.Apply(eventMessage);

            var entrySignal = StubSignalBuilder.BuyEntrySignal();

            this.entrySignalProcessor.Process(entrySignal);

            var result = this.tradeBook.GetTradesByTradeType(trade.TradeType);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            Assert.Equal(2, result.Count);
            Assert.Equal(0, this.inMemoryMessageStore.EventEnvelopes.Count);
        }

        [Fact]
        internal void GivenEntrySignal_WhenTradeStatusActive_IgnoresSignalDoesNothing()
        {
            // Arrange
            var trade = StubTradeBuilder.SellThreeUnits();
            this.tradeBook.AddTrade(trade);
            this.tradeBook.AddTrade(trade);

            // Act
            var eventMessage = StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry);
            trade.Apply(eventMessage);
            trade.Apply(eventMessage);

            var entrySignal = StubSignalBuilder.BuyEntrySignal();

            this.entrySignalProcessor.Process(entrySignal);

            var result = this.tradeBook.GetTradesByTradeType(trade.TradeType);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            Assert.Equal(2, result.Count);
            Assert.Equal(0, this.inMemoryMessageStore.EventEnvelopes.Count);
        }
    }
}
