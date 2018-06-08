//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Moq;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DataServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdatper mockLoggingAdatper;
        private readonly IActorRef dataServiceRef;
        private readonly Symbol symbol;

        public DataServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;

            var testActorSystem = ActorSystem.Create(nameof(AlphaModelServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.symbol = new Symbol("AUDUSD", Exchange.LMAX);

            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.dataServiceRef = testActorSystem.ActorOf(Props.Create(() => new DataService(
                setupContainer,
                messagingAdapter)));

            var mockBrokerageGateway = new Mock<IBrokerageGateway>().Object;
            var message = new InitializeBrokerageGateway(mockBrokerageGateway, Guid.NewGuid(), StubDateTime.Now());

            this.dataServiceRef.Tell(message);
        }

        [Fact]
        internal void GivenSubscribeSymbolDataTypeMessage_SetsUpBarAggregator()
        {
            // Arrange
            var barSpecification = new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5);
            var tradeType = new TradeType("TestScalp");
            var message = new SubscribeSymbolDataType(
                this.symbol,
                barSpecification,
                tradeType,
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.dataServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Setup for 5-Minute[Bid] bars",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            CustomAssert.EventuallyContains(
                "TimeBarAggregator-AUDUSD.LMAX-5-Minute[Bid]: Nautilus.Data.TimeBarAggregator initializing...",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnsubscribeSymbolDataTypeMessage_RemovesBarAggregator()
        {
            // Arrange
            var barSpecification = new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 1000);
            var tradeType = new TradeType("TestScalp");
            var message1 = new SubscribeSymbolDataType(
                this.symbol,
                barSpecification,
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
            this.dataServiceRef.Tell(message1);
            this.dataServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Data for AUDUSD.LMAX(TestScalp) bars deregistered",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenShutdownSystemMessage_()
        {
            // Arrange
            var message = new ShutdownSystem(Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.dataServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);
        }
    }
}
