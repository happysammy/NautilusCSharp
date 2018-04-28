//--------------------------------------------------------------
// <copyright file="DataServiceTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Moq;
    using Nautilus.BlackBox.Core.Enums;
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
        private readonly MockLogger mockLogger;
        private readonly IActorRef dataServiceRef;
        private readonly Symbol symbol;

        public DataServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(AlphaModelServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                NautilusEnvironment.Live,
                setupContainer.Clock,
                setupContainer.LoggerFactory);

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
            var BarSpecification = new BarSpecification(BarTimeFrame.Minute, 5);
            var tradeType = new TradeType("TestScalp");
            var message = new SubscribeSymbolDataType(
                this.symbol,
                BarSpecification,
                tradeType,
                0.00001m,
                Guid.NewGuid(),
                StubDateTime.Now());

            // Act
            this.dataServiceRef.Tell(message);

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
        internal void GivenUnsubscribeSymbolDataTypeMessage_RemovesBarAggregator()
        {
            // Arrange
            var BarSpecification = new BarSpecification(BarTimeFrame.Tick, 1000);
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
            this.dataServiceRef.Tell(message1);
            this.dataServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);

            CustomAssert.EventuallyContains(
                "MarketDataProcessor-AUDUSD.LMAX: Data for AUDUSD.LMAX(TestScalp) bars deregistered",
                this.mockLogger,
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
            LogDumper.Dump(this.mockLogger, this.output);
        }
    }
}