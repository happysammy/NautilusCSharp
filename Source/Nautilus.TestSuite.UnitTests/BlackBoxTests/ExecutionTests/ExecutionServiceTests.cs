//--------------------------------------------------------------
// <copyright file="ExecutionServiceTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.ExecutionTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Moq;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Execution;
    using Nautilus.Common.Enums;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExecutionServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly MessageWarehouse messageWarehouse;
        private readonly IActorRef executionServiceRef;

        public ExecutionServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            var testActorSystem = ActorSystem.Create(nameof(ExecutionServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.messageWarehouse = messagingServiceFactory.MessageWarehouse;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.executionServiceRef = testActorSystem.ActorOf(Props.Create(() => new ExecutionService(
                setupContainer,
                messagingAdapter)));

            var message = new InitializeBrokerageGateway(
                new Mock<IBrokerageGateway>().Object,
                Guid.NewGuid(),
                StubDateTime.Now());

            this.executionServiceRef.Tell(message);
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
            CustomAssert.EventuallyContains(
                "ExecutionService: Nautilus.BlackBox.Execution.ExecutionService initializing...",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnexpectedMessage_HandlesUnexpectedMessageAndLogs()
        {
            // Arrange
            // Act
            this.executionServiceRef.Tell("random_object", null);

            // Assert
            CustomAssert.EventuallyContains(
                "ExecutionService: Unhandled message random_object",
                this.mockLogger,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenSubmitTradeMessage_()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.Build();
            var message = new SubmitTrade(orderPacket, decimal.Zero, Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
        }

        [Fact]
        internal void GivenModifyStopLossMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            var stopLossModificationsIndex = new Dictionary<Order, Price> { { trade.TradeUnits[0].StopLoss, Price.Create(0.79000m, 0.00001m) } };
            var order = new StubOrderBuilder().StoplossOrder("1234");

            var message = new ModifyStopLoss(trade, stopLossModificationsIndex, Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
        }

        [Fact]
        internal void GivenClosePositionMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();

            var message = new ClosePosition(trade.TradeUnits[0], Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
        }

        [Fact]
        internal void GivenCancelOrderMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();

            var message = new CancelOrder(trade.TradeUnits[0].Entry, "Order Expired", Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
        }

        [Fact]
        internal void GivenShutdownSystemMessage_()
        {
            // Arrange
            var message = new ShutdownSystem(Guid.NewGuid(), StubDateTime.Now());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLogger, this.output);
        }
    }
}
