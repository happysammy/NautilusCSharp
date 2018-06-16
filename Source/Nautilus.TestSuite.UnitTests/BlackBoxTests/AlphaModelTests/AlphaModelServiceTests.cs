//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaModelServiceTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.BlackBox.AlphaModel;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.Common.MessageStore;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class AlphaModelServiceTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly AlphaStrategyModuleStore alphaStrategyModuleStore;
        private readonly IActorRef alphaModelServiceRef;

        public AlphaModelServiceTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(AlphaModelServiceTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.alphaStrategyModuleStore = new AlphaStrategyModuleStore();
            this.alphaModelServiceRef = testActorSystem.ActorOf(Props.Create(() => new AlphaModelService(
                setupContainer,
                messagingAdapter,
                this.alphaStrategyModuleStore)));
        }

        [Fact]
        internal void InitializesCorrectly_ReturnsStoreCountZero()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(0, this.alphaStrategyModuleStore.Count);
        }

        [Fact]
        internal void InitializesCorrectly_ReturnsEmptyLabelList()
        {
            // Arrange
            // Act
            var result = this.alphaStrategyModuleStore.StrategyLabelList;

            // Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        internal void InitializesCorrectly_LogsExpectedMessages()
        {
            // Arrange
            // Act
            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                "AlphaModelService: Initializing...",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenUnexpectedMessage_HandlesUnexpectedMessageAndLogs()
        {
            // Arrange
            // Act
            this.alphaModelServiceRef.Tell("random_object", null);

            // Assert
            CustomAssert.EventuallyContains(
                "AlphaModelService: Unhandled message random_object",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenMarketDataEventMessage_NoAlphaStrategyModules_LogsErrorAndRespondsFalse()
        {
            // Arrange
            var message = new BarDataEvent(
                new Symbol("some_symbol", Exchange.GLOBEX),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 1),
                StubBarBuilder.Build(),
                StubTickFactory.Create(new Symbol("some_symbol", Exchange.GLOBEX)),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.alphaModelServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            CustomAssert.EventuallyContains(
                "AlphaModelService: Validation Failed (The dictionary does not contain the symbolBarSpec key).",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenCreateAlphaStrategyModuleMessage_NoAlphaStrategyModules_CreatesModuleAndLogsAndSendsRelatedMessages()
        {
            // Arrange
            var message = CreateAlphaStrategyModuleMessage();

            // Act
            this.alphaModelServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                typeof(CreatePortfolio),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            CustomAssert.EventuallyContains(
                typeof(SubscribeBarData),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(1, this.alphaStrategyModuleStore.Count);
            Assert.Equal("AUDUSD.FXCM(TestTrade)", this.alphaStrategyModuleStore.StrategyLabelList.ElementAt(0).ToString());
        }

        [Fact]
        internal void GivenCreateAlphaStrategyModuleMessage_AlphaStrategyModuleAlreadyPresent_DoesNotCreateModuleAndLogsError()
        {
            // Arrange
            var message = new CreateAlphaStrategyModule(
                StubAlphaStrategyFactory.Create(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.alphaModelServiceRef.Tell(message);
            this.alphaModelServiceRef.Tell(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                "AlphaModelService: Validation Failed (The dictionary already contains the strategyLabel key).",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(1, this.alphaStrategyModuleStore.Count);
        }

        [Fact]
        internal void GivenMarketDataEventMessage_AlphaStrategyModuleDoesNotExist_LogsError()
        {
            // Arrange
            var message1 = CreateAlphaStrategyModuleMessage();

            var message2 = new BarDataEvent(
                new Symbol("AUDUSD", Exchange.LMAX),
                message1.Strategy.TradeProfile.BarSpecification,
                StubBarBuilder.Build(),
                StubTickFactory.Create(new Symbol("AUDUSD", Exchange.LMAX)),
                0.00001m,
                false,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.alphaModelServiceRef.Tell(message1);
            this.alphaModelServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                "AlphaModelService: Validation Failed (The dictionary does not contain the symbolBarSpec key).",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
            Assert.Equal(1, this.alphaStrategyModuleStore.Count);
        }

        [Fact]
        internal void GivenRemoveAlphaStratgeyModuleMessage_AlphaStrategyModuleExists_CreatesModuleAndLogsAndSendsRelatedMessages()
        {
            // Arrange
            var message1 = CreateAlphaStrategyModuleMessage();
            var message2 = RemoveAlphaStrategyModuleMessage();

            // Act
            this.alphaModelServiceRef.Tell(message1);

            // Allows the strategy module to instantiate prior to the removal (which is being tested)
            Task.Delay(100).Wait();

            this.alphaModelServiceRef.Tell(message2);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                "AlphaModelService: AUDUSD.FXCM(TestTrade) removed",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }

        [Fact]
        internal void GivenRemoveAlphaStratgeyModuleMessage_NoAlphaStrategyModules_LogsError()
        {
            // Arrange
            var message = RemoveAlphaStrategyModuleMessage();

            // Act
            this.alphaModelServiceRef.Tell(message);

            // Assert
            CustomAssert.EventuallyContains(
                "AlphaModelService: Validation Failed (The dictionary does not contain the strategyLabel key).",
                this.mockLoggingAdapter,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
            Assert.Equal(0, this.alphaStrategyModuleStore.Count);
        }

        private static CreateAlphaStrategyModule CreateAlphaStrategyModuleMessage()
        {
            return new CreateAlphaStrategyModule(
                StubAlphaStrategyFactory.Create(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }

        private static RemoveAlphaStrategyModule RemoveAlphaStrategyModuleMessage()
        {
            return new RemoveAlphaStrategyModule(
                new Symbol("AUDUSD", Exchange.FXCM),
                new TradeType("TestTrade"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
