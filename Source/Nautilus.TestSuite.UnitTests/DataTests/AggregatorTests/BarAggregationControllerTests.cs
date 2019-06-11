//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationControllerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Data.Aggregation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduler;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregationControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly MockMessagingAgent receiver;
        private readonly HashedWheelTimerScheduler scheduler;
        private readonly BarAggregationController controller;

        public BarAggregationControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.logger = setupFactory.LoggingAdapter;
            this.receiver = new MockMessagingAgent();
            var container = setupFactory.Create();
            var dataBusAdapter = DataBusFactory.Create(container);
            this.scheduler = new HashedWheelTimerScheduler(container);

            this.controller = new BarAggregationController(
                container,
                dataBusAdapter,
                this.scheduler);
        }

        [Fact]
        internal void GivenSubscribeBarDataMessages_CreatesAggregatorAndSubscriptions()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
            Assert.Equal(1, this.controller.BarAggregators.Count);
            Assert.Equal(2, this.controller.Subscriptions.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenDuplicateSubscribeBarDataMessages_CreatesAggregatorsSubscriptionsAndHandlesDuplicate()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            this.controller.Endpoint.Send(subscribe2);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
            Assert.Equal(1, this.controller.BarAggregators.Count);
            Assert.Equal(2, this.controller.Subscriptions.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenMultipleSubscribeBarDataMessages_CreatesAggregatorsAndSubscriptions()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol1, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType4 = new BarType(symbol2, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            this.controller.Endpoint.Send(subscribe3);
            this.controller.Endpoint.Send(subscribe4);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
            Assert.Equal(2, this.controller.BarAggregators.Count);
            Assert.Equal(4, this.controller.Subscriptions.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol1].Specifications.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol2].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenUnsubscribeBarDataMessage_RemovesSubscription()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);

            // Act
            Task.Delay(100).Wait();

            this.controller.Endpoint.Send(unsubscribe);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
            Assert.Equal(1, this.controller.Subscriptions.Count);
            Assert.Equal(1, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenDuplicateUnsubscribeBarDataMessages_RemovesSubscriptionAndHandlesDuplicate()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);

            // Act
            Task.Delay(100).Wait();

            this.controller.Endpoint.Send(unsubscribe);
            this.controller.Endpoint.Send(unsubscribe);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
            Assert.Equal(1, this.controller.Subscriptions.Count);
            Assert.Equal(1, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenMultipleUnsubscribeBarDataMessages_RemovesAggregatorsAndSubscriptions()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol1, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType4 = new BarType(symbol2, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe1 = new Unsubscribe<BarType>(
                barType1,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe2 = new Unsubscribe<BarType>(
                barType2,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe3 = new Unsubscribe<BarType>(
                barType3,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe4 = new Unsubscribe<BarType>(
                barType4,
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            this.controller.Endpoint.Send(subscribe3);
            this.controller.Endpoint.Send(subscribe4);

            // Act
            Task.Delay(100).Wait();

            this.controller.Endpoint.Send(unsubscribe1);
            this.controller.Endpoint.Send(unsubscribe2);
            this.controller.Endpoint.Send(unsubscribe3);
            this.controller.Endpoint.Send(unsubscribe4);

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.logger, this.output);
        }
    }
}
