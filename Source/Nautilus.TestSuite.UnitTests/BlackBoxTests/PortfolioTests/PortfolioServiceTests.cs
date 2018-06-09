//--------------------------------------------------------------------------------------------------
// <copyright file="PortfolioServiceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PortfolioServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdatper mockLoggingAdatper;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly SecurityPortfolioStore portfolioStore;
        private readonly IActorRef portfolioServiceRef;

        public PortfolioServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;

            var testActorSystem = ActorSystem.Create(nameof(PortfolioServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.portfolioStore = new SecurityPortfolioStore();
            this.portfolioServiceRef = testActorSystem.ActorOf(Props.Create(() => new PortfolioService(setupContainer, messagingAdapter, this.portfolioStore)));
        }

        [Fact]
        internal void InitializesCorrectly_PortfolioStoreReturnsZeroCount()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(0, this.portfolioStore.Count);
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "PortfolioService: Initializing...",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnexpectedMessage_HandlesUnexpectedMessageAndLogs()
        {
            // Arrange
            // Act
            this.portfolioServiceRef.Tell("random_object", null);

            // Assert
            CustomAssert.EventuallyContains("PortfolioService: Unhandled message random_object", this.mockLoggingAdatper, 100, 20);
        }

        [Fact]
        internal void GivenMarketDataEventMessage_WithNoPortfolios_ThenLogsError()
        {
            // Arrange
            // Act
            this.portfolioServiceRef.Tell(ValidMarketDataEvent());

            // Assert
            Assert.Equal(0, this.portfolioStore.Count);
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "PortfolioService: Validation Failed (The collection does not contain the portfolioSymbol key).",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenSignalEventMessages_WithNoPortfolios_ThenLogsErrors()
        {
            // Arrange
            // Act
            this.portfolioServiceRef.Tell(DummyEntrySignalEvent());
            this.portfolioServiceRef.Tell(DummyExitSignalEvent());
            this.portfolioServiceRef.Tell(DummyTrailingStopSignalEvent());

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "PortfolioService: Validation Failed (The collection does not contain the portfolioSymbol key).",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(0, this.portfolioStore.Count);
        }

        [Fact]
        internal void GivenOrderEventMessage_WithNoPortfolios_ThenLogsErrors()
        {
            // Arrange
            // Act
            this.portfolioServiceRef.Tell(DummyOrderFilledEventWrapper());

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "PortfolioService: Validation Failed (The collection does not contain the portfolioSymbol key).",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(0, this.portfolioStore.Count);
        }

        [Fact]
        internal void GivenCreatePortfolioMessage_WithNoPortfolios_ThenCreatesCorrectPortfolio()
        {
            // Arrange
            var message = new CreatePortfolio(StubInstrumentFactory.AUDUSD(), Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.portfolioServiceRef.Tell(message);

            // Assert
            Task.Delay(100).Wait();
            Assert.Equal(1, this.portfolioStore.Count);
            Assert.StartsWith("AUDUSD.FXCM", this.portfolioStore.SymbolList[0].ToString());
        }

        [Fact]
        internal void GivenCreatePortfolioMessage_WithPortfolioAlreadyExisting_ThenLogsError()
        {
            // Arrange
            var message = new CreatePortfolio(StubInstrumentFactory.AUDUSD(), Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.portfolioServiceRef.Tell(message);
            this.portfolioServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "PortfolioService: Validation Failed (The collection already contains the portfolioSymbol key).",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(1, this.portfolioStore.Count);
            Assert.StartsWith("AUDUSD.FXCM", this.portfolioStore.SymbolList[0].ToString());
        }

        [Fact]
        internal void GivenTradeApprovedMessage_WithPortfolio_ThenSendsMessageToCorrectPortfolio()
        {
            // Arrange
            var message1 = new CreatePortfolio(StubInstrumentFactory.AUDUSD(), Guid.NewGuid(), StubDateTime.Now());

            var message2 = new TradeApproved(
                StubOrderPacketBuilder.Build(),
                1,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.portfolioServiceRef.Tell(message1);
            this.portfolioServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                typeof(SubmitTrade),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        private static BarDataEvent ValidMarketDataEvent()
        {
            return new BarDataEvent(
                new Symbol("SYMBOL", Exchange.LMAX),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 1),
                StubBarBuilder.Build(),
                StubTickFactory.Create(new Symbol("SYMBOL", Exchange.LMAX)),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now());
        }

        private static SignalEvent DummyEntrySignalEvent()
        {
            return new SignalEvent(
                new EntrySignal(
                    new Symbol("SYMBOL", Exchange.LMAX),
                    new EntityId("NONE"),
                    new Label("TestSignal"),
                    StubTradeProfileFactory.Create(20),
                    OrderSide.Buy,
                    Price.Create(2, 1),
                    Price.Create(1, 1),
                    new SortedDictionary<int, Price>(),
                    StubDateTime.Now()),
                Guid.NewGuid(),
                StubDateTime.Now());
        }

        private static SignalEvent DummyExitSignalEvent()
        {
            return new SignalEvent(
                StubSignalBuilder.LongExitSignalForAllUnits(new TradeType("TestTrade"), Period.Zero),
                Guid.NewGuid(),
                StubDateTime.Now());
        }

        private static SignalEvent DummyTrailingStopSignalEvent()
        {
            return new SignalEvent(
                StubSignalBuilder.LongExitSignalForAllUnits(new TradeType("TestTrade"), Period.Zero), // TODO?
                Guid.NewGuid(),
                StubDateTime.Now());
        }

        private static OrderFilled DummyOrderFilledEventWrapper()
        {
            return new OrderFilled(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("some_orderId"),
                new EntityId("some_executionId"),
                new EntityId("some_executionTicket"),
                OrderSide.Buy,
                Quantity.Create(100),
                Price.Create(100m, 0.01m),
                StubDateTime.Now(),
                Guid.NewGuid(),
                StubDateTime.Now());
        }
    }
}
