//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationControllerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduling;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BarAggregationControllerTests
    {
        private readonly MockComponent receiver;
        private readonly BarAggregationController controller;

        public BarAggregationControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            var dataBusAdapter = DataBusFactory.Create(container);
            var scheduler = new HashedWheelTimerScheduler(container);

            this.controller = new BarAggregationController(
                container,
                dataBusAdapter,
                scheduler);

            this.receiver = new MockComponent(container);
        }

        [Fact]
        internal void GivenSubscribeBarDataMessages_CreatesAggregatorAndSubscriptions()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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
            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2).Wait();
            this.controller.Stop().Wait();
            Task.Delay(100).Wait();  // TODO: Intermittent test sometimes Specifications.Count == 1

            // Assert
            Assert.Equal(1, this.controller.BarAggregators.Count);
            Assert.Equal(2, this.controller.Subscriptions.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenDuplicateSubscribeBarDataMessages_CreatesAggregatorsSubscriptionsAndHandlesDuplicate()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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
            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2);
            this.controller.Endpoint.SendAsync(subscribe2).Wait();
            this.controller.Stop().Wait();

            Task.Delay(100).Wait(); // Extra delay needed to prevent intermittently failing test?

            // Assert
            Assert.Equal(1, this.controller.BarAggregators.Count);
            Assert.Equal(2, this.controller.Subscriptions.Count);
            Assert.Equal(2, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenMultipleSubscribeBarDataMessages_CreatesAggregatorsAndSubscriptions()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol1, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol1, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

            var symbol2 = new Symbol("GBPUSD", new Venue("FXCM"));
            var barType3 = new BarType(symbol2, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType4 = new BarType(symbol2, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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
            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2);
            this.controller.Endpoint.SendAsync(subscribe3);
            this.controller.Endpoint.SendAsync(subscribe4).Wait();
            this.controller.Stop().Wait();

            // Assert
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
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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

            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2);

            // Act
            this.controller.Endpoint.SendAsync(unsubscribe).Wait();
            this.controller.Stop().Wait();

            // Assert
            Assert.Equal(1, this.controller.Subscriptions.Count);
            Assert.Equal(1, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenDuplicateUnsubscribeBarDataMessages_RemovesSubscriptionAndHandlesDuplicate()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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

            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2);

            // Act
            this.controller.Endpoint.SendAsync(unsubscribe);
            this.controller.Endpoint.SendAsync(unsubscribe).Wait();
            this.controller.Stop().Wait();

            // Assert
            Assert.Equal(1, this.controller.Subscriptions.Count);
            Assert.Equal(1, this.controller.BarAggregators[symbol].Specifications.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }

        [Fact]
        internal void GivenMultipleUnsubscribeBarDataMessages_RemovesAggregatorsAndSubscriptions()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", new Venue("FXCM"));
            var barType1 = new BarType(symbol1, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType2 = new BarType(symbol1, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

            var symbol2 = new Symbol("GBPUSD", new Venue("FXCM"));
            var barType3 = new BarType(symbol2, new BarSpecification(1, BarStructure.Second, PriceType.Bid));
            var barType4 = new BarType(symbol2, new BarSpecification(1, BarStructure.Minute, PriceType.Bid));

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

            this.controller.Endpoint.SendAsync(subscribe1);
            this.controller.Endpoint.SendAsync(subscribe2);
            this.controller.Endpoint.SendAsync(subscribe3);
            this.controller.Endpoint.SendAsync(subscribe4);

            // Act
            this.controller.Endpoint.SendAsync(unsubscribe1);
            this.controller.Endpoint.SendAsync(unsubscribe2);
            this.controller.Endpoint.SendAsync(unsubscribe3);
            this.controller.Endpoint.SendAsync(unsubscribe4).Wait();
            this.controller.Stop().Wait();

            // Assert
            Assert.Equal(0, this.controller.Subscriptions.Count);
            Assert.Empty(this.controller.UnhandledMessages);
        }
    }
}
