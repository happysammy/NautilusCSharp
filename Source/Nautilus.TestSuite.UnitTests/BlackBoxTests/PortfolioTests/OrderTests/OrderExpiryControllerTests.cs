//--------------------------------------------------------------------------------------------------
// <copyright file="OrderExpiryControllerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.Common.MessageStore;
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderExpiryControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly InMemoryMessageStore inMemoryMessageStore;
        private readonly OrderExpiryController orderExpiryController;

        public OrderExpiryControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var instrument = StubInstrumentFactory.AUDUSD();
            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            var testActorSystem = ActorSystem.Create(nameof(OrderExpiryControllerTests));

            var messagingServiceFactory = new MockMessagingServiceFactory();
            messagingServiceFactory.Create(
                testActorSystem,
                setupContainer);

            this.inMemoryMessageStore = messagingServiceFactory.InMemoryMessageStore;
            var messagingAdapter = messagingServiceFactory.MessagingAdapter;

            this.orderExpiryController = new OrderExpiryController(
                setupContainer,
                messagingAdapter,
                instrument.Symbol);
        }

        [Fact]
        internal void GivenNewInstantiation_InitializesCorrectly()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal(0, this.orderExpiryController.TotalCounters);
        }

        [Fact]
        internal void AddCounters_StubOrderPacket_ReturnsCorrectTimerCount()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration();
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);
            var barsValid = 1;

            // Act
            this.orderExpiryController.AddCounters(orderPacket, barsValid);
            var result = this.orderExpiryController.TotalCounters;

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        internal void RemoveCounters_WhenStubOrderPacketHasBeenAdded_ReturnsTotalCountersToZero()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration();
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);
            var barsValid = 2;

            // Act
            this.orderExpiryController.AddCounters(orderPacket, barsValid);
            var result1 = this.orderExpiryController.TotalCounters;

            this.orderExpiryController.RemoveCounter(orderPacket.Orders[0].Entry.Id);
            this.orderExpiryController.RemoveCounter(orderPacket.Orders[1].Entry.Id);
            this.orderExpiryController.RemoveCounter(orderPacket.Orders[2].Entry.Id);
            var result2 = this.orderExpiryController.TotalCounters;

            // Assert
            Assert.Equal(3, result1);
            Assert.Equal(0, result2);
        }

        [Fact]
        internal void ProcessCounters_WithNoCounters_DoesNothing()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);

            // Act
            this.orderExpiryController.ProcessCounters(orderPacket.OrderIdList);

            // Assert
            Assert.Equal(0, this.orderExpiryController.TotalCounters);
        }

        [Fact]
        internal void ProcessCounters_OrderPacketWithThreeUnitsWhichHaveNotExpired_DoesNothing()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);
            var barsValid = 2;
            this.orderExpiryController.AddCounters(orderPacket, barsValid);

            // Act
            this.orderExpiryController.ProcessCounters(orderPacket.OrderIdList);

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(0, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void ProcessTimers_OrderPacketWithThreeUnitsExpired_SendsCorrectMessageToCommandBusRemovesTimers()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1);
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);
            var barsValid = 1;
            this.orderExpiryController.AddCounters(orderPacket, barsValid);

            // Act
            this.orderExpiryController.ProcessCounters(orderPacket.OrderIdList);

            // Assert
            Task.Delay(100).Wait();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            CustomAssert.EventuallyContains(
                typeof(CancelOrder),
                this.inMemoryMessageStore.CommandEnvelopes,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);

            Assert.Equal(3, this.inMemoryMessageStore.CommandEnvelopes.Count);
        }

        [Fact]
        internal void ProcessTimers_OrderPacketWithThreeUnitsNotExpiredThenProcessListOfUnrecognizedOrders_ResultsInForceRemoveAndReturnsTimerCountZero()
        {
            // Arrange
            var expireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(5);
            var orderPacket = StubOrderPacketBuilder.ThreeUnitsAndExpireTime(expireTime);
            var unrecognizedActiveOrders = new List<OrderId>
                                               {
                                                   new OrderId("some_other_orderId1"),
                                                   new OrderId("some_other_orderId2")
                                               };

            // Act
            this.orderExpiryController.AddCounters(orderPacket, 1);
            var result1 = this.orderExpiryController.TotalCounters;

            this.orderExpiryController.ProcessCounters(new ReadOnlyList<OrderId>(unrecognizedActiveOrders));
            var result2 = this.orderExpiryController.TotalCounters;

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(3, result1);
            Assert.Equal(0, result2);
        }
    }
}
