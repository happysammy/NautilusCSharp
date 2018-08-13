//--------------------------------------------------------------------------------------------------
// <copyright file="RiskServiceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.RiskTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Messages.Commands;
    using Nautilus.BlackBox.Risk;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RiskServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly IActorRef riskServiceRef;

        public RiskServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(RiskServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.riskServiceRef = testActorSystem.ActorOf(Props.Create(() => new RiskService(
                setupContainer,
                messagingAdapter)));
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                "RiskService: Initializing...",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnexpectedMessage_HandlesUnexpectedMessageAndLogs()
        {
            // Arrange
            // Act
            this.riskServiceRef.Tell("random_object", null);

            // Assert
            CustomAssert.EventuallyContains(
                "RiskService: Unhandled message random_object",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenInitializeRiskModelMessage_Logs()
        {
            // Arrange
            var account = StubAccountFactory.Create();
            var riskModel = new RiskModel(
                new RiskModelId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            var message = new InitializeRiskModel(account, riskModel, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.riskServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                "RiskService: BrokerageAccount and RiskModel initialized",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenRequestTradeApprovalMessage_WithValidTrade_ApprovesAndSendsMessageToCommandBus()
        {
            // Arrange
            var account = StubAccountFactory.Create();
            var riskModel = new RiskModel(
                new RiskModelId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            var message1 = new InitializeRiskModel(account, riskModel, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            var orderPacket = StubOrderPacketBuilder.Build();
            var entrySignal = StubSignalBuilder.BuyEntrySignal();
            var message2 = new RequestTradeApproval(orderPacket, entrySignal, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.riskServiceRef.Tell(message1);
            this.riskServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                typeof(TradeApproved),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenRequestTradeApprovalMessage_WhenFreeEquityZero_DoesNothingAndLogs()
        {
            // Arrange
            var account = StubAccountFactory.ZeroCash();
            var riskModel = new RiskModel(
                new RiskModelId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            var message1 = new InitializeRiskModel(account, riskModel, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            var orderPacket = StubOrderPacketBuilder.Build();
            var entrySignal = StubSignalBuilder.BuyEntrySignal();
            var message2 = new RequestTradeApproval(orderPacket, entrySignal, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            this.riskServiceRef.Tell(message1);
            this.riskServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                "RiskService: RequestTradeApproval-TestTrade ignored... (Free Equity <= zero)",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.True(this.inMemoryMessageStore.DocumentEnvelopes.Count == 0);
        }

        [Fact]
        internal void GivenAccountEventMessage_Logs()
        {
            // Arrange
            var account = StubAccountFactory.Create();
            var riskModel = new RiskModel(
                new RiskModelId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            var message1 = new InitializeRiskModel(account, riskModel, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            var message2 = new AccountEvent(
                EntityIdFactory.Account(Broker.InteractiveBrokers, "123456789"),
                Broker.InteractiveBrokers,
                "123456789",
                CurrencyCode.USD,
                Money.Create(90000, CurrencyCode.USD),
                Money.Create(100000, CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                Money.Zero(CurrencyCode.USD),
                decimal.Zero,
                string.Empty,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.riskServiceRef.Tell(message1);
            this.riskServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                "RiskService: BrokerageAccount updated",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEventMessage_()
        {
            // Arrange
            var account = StubAccountFactory.Create();
            var riskModel = new RiskModel(
                new RiskModelId(Guid.NewGuid().ToString()),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            var message1 = new InitializeRiskModel(account, riskModel, Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            var message2 = new BarDataEvent(
                new BarType(
                    new Symbol("AUDUSD", Venue.FXCM),
                    new BarSpecification(QuoteType.Bid, Resolution.Minute, 1)),
                StubBarBuilder.Build(),
                StubTickFactory.Create(new Symbol("AUDUSD", Venue.FXCM)),
                decimal.One,
                false,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.riskServiceRef.Tell(message1);
            this.riskServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }
    }
}
