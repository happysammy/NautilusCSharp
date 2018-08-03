//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.ExecutionTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Moq;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Execution;
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
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly IActorRef executionServiceRef;

        public ExecutionServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(ExecutionServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.executionServiceRef = testActorSystem.ActorOf(Props.Create(() => new ExecutionService(
                setupContainer,
                messagingAdapter)));

            var message = new InitializeGateway(
                new Mock<IExecutionGateway>().Object,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.executionServiceRef.Tell(message);
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                "ExecutionService: Initializing...",
                this.mockLoggingAdapter,
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
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenSubmitTradeMessage_()
        {
            // Arrange
            var orderPacket = StubOrderPacketBuilder.Build();
            var message = new SubmitTrade(orderPacket, decimal.Zero, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
        }

        [Fact]
        internal void GivenModifyStopLossMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();

            var message = new ModifyOrder(
                trade.TradeUnits[0].StopLoss,
                Price.Create(0.79000m, 0.00001m),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }

        [Fact]
        internal void GivenClosePositionMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();

            var message = new ClosePosition(
                trade.TradeUnits[0].Position,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }

        [Fact]
        internal void GivenCancelOrderMessage_()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();

            var message = new CancelOrder(trade.TradeUnits[0].Entry, "Order Expired", Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }

        [Fact]
        internal void GivenShutdownSystemMessage_()
        {
            // Arrange
            var message = new SystemShutdown(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.executionServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }
    }
}
