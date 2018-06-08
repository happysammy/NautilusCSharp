//--------------------------------------------------------------------------------------------------
// <copyright file="TickBarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests
{
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.Data;
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
    public class TickBarAggregatorTests
    {
        private readonly ITestOutputHelper output;
        private readonly ActorSystem testActorSystem;
        private readonly BlackBoxContainer container;
        private readonly MockLoggingAdatper mockLoggingAdatper;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly Symbol symbol;

        public TickBarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.container = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;

            this.testActorSystem = ActorSystem.Create(nameof(TickBarAggregatorTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                this.testActorSystem,
                this.container);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            this.messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.symbol = new Symbol("AUDUSD", Exchange.FXCM);
        }

        [Fact]
        internal void OnFirstTick_LogsExpected()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                new Symbol("AUDUSD", Exchange.FXCM),
                new BarSpecification(BarQuoteType.Ask, BarResolution.Tick, 5));

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TickBarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now() + Duration.FromSeconds(1));

            // Act
            barAggregatorRef.Tell(quote1);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            CustomAssert.EventuallyContains(
                "TickBarAggregator-AUDUSD.FXCM-5-Tick[Ask]: Registered for 5-Tick[Ask] bars",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            CustomAssert.EventuallyContains(
                "TickBarAggregator-AUDUSD.FXCM-5-Tick[Ask]: Receiving quotes (AUDUSD) from FXCM...",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenTickMessages_WhenSecondBar_ReturnsValidBar()
        {
            // Arrange
            var symbolBarSpec = StubSymbolBarSpec.AUDUSD();

            var barAggregatorRef = this.testActorSystem.ActorOf(Props.Create(() => new TickBarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m)));

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(2));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(4));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(5));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
            // TODO: Change actor testing methods.

//            CustomAssert.EventuallyContains<MarketDataEvent>(
//                typeof(MarketDataEvent),
//                this.messageWarehouse.EventEnvelopes,
//                EventuallyContains.TimeoutMilliseconds,
//                EventuallyContains.PollIntervalMilliseconds);
//
//            var marketDataEvent = this.messageWarehouse.EventEnvelopes[0].Open(StubDateTime.Now()).Event.AsInstanceOf<MarketDataEvent>();
//
//            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(5), marketDataEvent?.Bar.Timestamp);
//            Assert.Equal(Quantity.Create(5), marketDataEvent?.Bar.Volume);
        }
    }
}
