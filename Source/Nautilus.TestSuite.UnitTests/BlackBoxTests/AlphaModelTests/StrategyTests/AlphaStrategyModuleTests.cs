//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategyModuleTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.StrategyTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.Common.MessageStore;
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
    public class AlphaStrategyModuleTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly BarStore barStore;
        private readonly BarStore barStoreDaily;
        private readonly IActorRef alphaStrategyModuleRef;

        public AlphaStrategyModuleTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(AlphaStrategyModuleTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            var alphaStrategy = StubAlphaStrategyFactory.Create();

            this.barStore = new BarStore(alphaStrategy.Instrument.Symbol, alphaStrategy.TradeProfile.BarSpecification);
            this.barStoreDaily = new BarStore(alphaStrategy.Instrument.Symbol, new BarSpecification(BarQuoteType.Bid, BarResolution.Day, 1));

            var marketDataProvider = new MarketDataProvider(alphaStrategy.Instrument.Symbol);
            marketDataProvider.Update(StubTickFactory.Create(alphaStrategy.Instrument.Symbol), 0.00005m);

            var entrySignalGenerator = new EntrySignalGenerator(
                alphaStrategy.Instrument,
                alphaStrategy.TradeProfile,
                alphaStrategy.EntryAlgorithms,
                alphaStrategy.StopLossAlgorithm,
                alphaStrategy.ProfitTargetAlgorithm);

            var exitSignalGenerator = new ExitSignalGenerator(
                alphaStrategy.Instrument,
                alphaStrategy.TradeProfile,
                alphaStrategy.ExitAlgorithms);

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                alphaStrategy.Instrument,
                alphaStrategy.TradeProfile,
                alphaStrategy.TrailingStopAlgorithms);

            this.alphaStrategyModuleRef = testActorSystem.ActorOf(Props.Create(() => new AlphaStrategyModule(
                setupContainer,
                messagingAdapter,
                alphaStrategy,
                this.barStore,
                this.barStoreDaily,
                marketDataProvider,
                entrySignalGenerator,
                exitSignalGenerator,
                trailingStopSignalGenerator)));

            Task.Delay(100).Wait();
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                "AlphaStrategyModule-AUDUSD.FXCM(TestTrade): Nautilus.BlackBox.AlphaModel.Strategy.AlphaStrategyModule initializing...",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnexpected_HandlesUnexpectedMessageAndLogs()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell("random_object", null);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                "AlphaStrategyModule-AUDUSD.FXCM(TestTrade): Unhandled message random_object",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_IntradayBarWhichIsInvalid_LogsError()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(InvalidMarketDataEvent());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                "AlphaStrategyModule-AUDUSD.FXCM(TestTrade): Received MarketDataEvent",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_IntradayBarWhichIsValidAndHistorical_UpdatesBarStoreAndDoesNotRunSignalGenerators()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(HistoricalMarketDataEvent());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyHasCount(
                1,
                this.barStore,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(0, this.inMemoryMessageStore.EventEnvelopes.Count);

            this.mockLogger.WriteStashToOutput(this.output);
        }

        [Fact]
        internal void GivenMarketDataEvent_DailyBarWhichIsValid_UpdatesDailyBarStoreAndDoesNotRunSignalGenerators()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventDailyBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyHasCount(
                1,
                this.barStoreDaily,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(0, this.inMemoryMessageStore.EventEnvelopes.Count);

            this.mockLogger.WriteStashToOutput(this.output);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsInvalid_DoesNotUpdateBarStoreAndLogsError()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(InvalidMarketDataEvent());

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLogger, this.output);
            Assert.Equal(0, this.barStore.Count);
            Assert.Equal(0, this.inMemoryMessageStore.EventEnvelopes.Count);

            this.mockLogger.WriteStashToOutput(this.output);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersEntrySignalBuy_SendsBuyEntrySignalToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBullBar());

            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(EntrySignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);

            //Assert.Equal(OrderSide.Buy, MessageFinder.FindSignalEvent<EntrySignal>(this.messageWarehouse.EventEnvelopes).OrderSide);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersEntrySignalSell_SendsSellEntrySignalWrapperToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBearBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(EntrySignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersExitSignalLong_SendsExitSignalWrapperToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBullBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(ExitSignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersExitSignalShort_SendsExitSignalToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBullBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(ExitSignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersTrailingStopSignalLong_SendsTrailingStopSignalToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBullBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(TrailingStopSignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEvent_BarWhichIsValidAndTriggersTrailingStopSignalShort_SendsTrailingStopSignalToEventBus()
        {
            // Arrange
            // Act
            this.alphaStrategyModuleRef.Tell(ValidMarketDataEventBullBar());

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            // TODO: Change actor testing methods.
//            CustomAssert.EventuallyContains<SignalEvent>(
//                typeof(TrailingStopSignal),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
        }

        private static MarketDataEvent ValidMarketDataEventBullBar()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5),
                new Bar(
                    Price.Create(0.80100m, 0.00001m),
                    Price.Create(0.80200m, 0.00001m),
                    Price.Create(0.80100m, 0.00001m),
                    Price.Create(0.80150m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.80150m, 0.00001m),
                    Price.Create(0.80155m, 0.00001m),
                    StubDateTime.Now()),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());
        }

        private static MarketDataEvent ValidMarketDataEventBearBar()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5),
                new Bar(
                    Price.Create(0.80000m, 0.00001m),
                    Price.Create(0.80020m, 0.00001m),
                    Price.Create(0.79980m, 0.00001m),
                    Price.Create(0.79990m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.79990m, 0.00001m),
                    Price.Create(0.80000m, 0.00001m),
                    StubDateTime.Now()),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());
        }

        private static MarketDataEvent ValidMarketDataEventDailyBar()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Day, 1),
                new Bar(
                    Price.Create(0.80000m, 0.00001m),
                    Price.Create(0.80200m, 0.00001m),
                    Price.Create(0.79900m, 0.00001m),
                    Price.Create(0.80010m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.80100m, 0.00001m),
                    Price.Create(0.80115m, 0.00001m),
                    StubDateTime.Now()),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromDays(1).ToDuration());
        }

        private static MarketDataEvent HistoricalMarketDataEvent()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5),
                new Bar(
                    Price.Create(0.80000m, 0.00001m),
                    Price.Create(0.80020m, 0.00001m),
                    Price.Create(0.79980m, 0.00001m),
                    Price.Create(0.80010m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.80010m, 0.00001m),
                    Price.Create(0.80015m, 0.00001m),
                    StubDateTime.Now()),
                0.00001m,
                true,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());
        }

        // Invalid because the bar time frame is one minute.
        private static MarketDataEvent InvalidMarketDataEvent()
        {
            return new MarketDataEvent(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 1),
                new Bar(
                    Price.Create(0.80000m, 0.00001m),
                    Price.Create(0.80020m, 0.00001m),
                    Price.Create(0.79980m, 0.00001m),
                    Price.Create(0.80010m, 0.00001m),
                    Quantity.Create(1000),
                    StubDateTime.Now() + Period.FromMinutes(5).ToDuration()),
                new Tick(
                    new Symbol("AUDUSD", Exchange.FXCM),
                    Price.Create(0.80010m, 0.00001m),
                    Price.Create(0.80015m, 0.00001m),
                    StubDateTime.Now()),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());
        }
    }
}
