//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageBusTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockMessagingAgent receiver;
        private readonly MessageBus<Event> messageBus;

        public MessageBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.receiver = new MockMessagingAgent();
            this.messageBus = new MessageBus<Event>(container);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.DataService, this.receiver.Endpoint },
                { ServiceAddress.BarAggregationController, this.receiver.Endpoint },
                { ServiceAddress.DatabaseTaskManager, this.receiver.Endpoint },
            };

            this.messageBus.Endpoint.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                Guid.NewGuid(),
                containerFactory.Clock.TimeNow()));

            this.receiver.RegisterHandler<IEnvelope>(this.receiver.OnMessage);
        }

        [Fact]
        internal void InitializedMessageBus_IsInExpectedState()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal("MessageBus<Event>", this.messageBus.Name.ToString());
            Assert.Equal(typeof(Event), this.messageBus.BusType);
            Assert.Equal(0, this.messageBus.Subscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionCount);
            Assert.Equal(0, this.messageBus.DeadLetters.Count);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsInvalid_DoesNotSubscribe()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Command),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsBusType_SubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(typeof(Event), this.messageBus.Subscriptions);
            Assert.Equal(1, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenAlreadySubscribedToBusType_DoesNotDuplicateSubscription()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(Event)].Count);
            Assert.Equal(1, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsSpecificType_SubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(typeof(MarketOpened), this.messageBus.Subscriptions);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(MarketOpened)].Count);
            Assert.Equal(1, this.messageBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenSubscribe_WhenAlreadySubscribedToSpecificType_DoesNotDuplicateSubscription()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(typeof(MarketOpened), this.messageBus.Subscriptions);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(MarketOpened)].Count);
            Assert.Equal(1, this.messageBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenMultipleSubscribe_HandlesCorrectly()
        {
            // Arrange
            var receiver2 = new MockMessagingAgent("MockMessagingAgent2");

            var subscribe1 = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<Type>(
                typeof(Event),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<Type>(
                typeof(FixSessionConnected),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe5 = new Subscribe<Type>(
                typeof(FixSessionConnected),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe1);
            this.messageBus.Endpoint.Send(subscribe2);
            this.messageBus.Endpoint.Send(subscribe3);
            this.messageBus.Endpoint.Send(subscribe4);
            this.messageBus.Endpoint.Send(subscribe5);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(typeof(Event), this.messageBus.Subscriptions);
            Assert.Contains(typeof(MarketOpened), this.messageBus.Subscriptions);
            Assert.Contains(typeof(FixSessionConnected), this.messageBus.Subscriptions);
            Assert.Equal(2, this.messageBus.Subscriptions[typeof(Event)].Count);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(MarketOpened)].Count);
            Assert.Equal(2, this.messageBus.Subscriptions[typeof(FixSessionConnected)].Count);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(MarketOpened)].Count);
            Assert.Equal(5, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenSubscribedToBusType_UnsubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenSubscribedToSpecificType_UnsubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenTypeIsInvalid_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(Command),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenNotSubscribedToBusType_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenNotSubscribedToSpecificType_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenMultipleSubscribe_ThenUnsubscribe_HandlesCorrectly()
        {
            // Arrange
            var receiver2 = new MockMessagingAgent("MockMessagingAgent2");

            var subscribe1 = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<Type>(
                typeof(Event),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<Type>(
                typeof(FixSessionConnected),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe5 = new Subscribe<Type>(
                typeof(FixSessionConnected),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe1 = new Unsubscribe<Type>(
                typeof(Event),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe2 = new Unsubscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe3 = new Unsubscribe<Type>(
                typeof(FixSessionConnected),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe1);
            this.messageBus.Endpoint.Send(subscribe2);
            this.messageBus.Endpoint.Send(subscribe3);
            this.messageBus.Endpoint.Send(subscribe4);
            this.messageBus.Endpoint.Send(subscribe5);
            this.messageBus.Endpoint.Send(subscribe5); // Test duplicate subscriptions do not occur
            this.messageBus.Endpoint.Send(unsubscribe1);
            this.messageBus.Endpoint.Send(unsubscribe2);
            this.messageBus.Endpoint.Send(unsubscribe3);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(typeof(Event), this.messageBus.Subscriptions);
            Assert.Contains(typeof(FixSessionConnected), this.messageBus.Subscriptions);
            Assert.DoesNotContain(typeof(MarketOpened), this.messageBus.Subscriptions);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(Event)].Count);
            Assert.Equal(1, this.messageBus.Subscriptions[typeof(FixSessionConnected)].Count);
            Assert.Equal(2, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenAddressedEnvelope_WhenAddressInSwitchboard_SendsToReceiver()
        {
            // Arrange
            var message = new MarketOpened(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var envelope = new Envelope<MarketOpened>(
                message,
                ServiceAddress.BarAggregationController,
                ServiceAddress.DataService,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.receiver.Messages);
        }

        [Fact]
        internal void GivenAddressedEnvelope_WhenAddressUnknown_SendsToDeadLetters()
        {
            // Arrange
            var message = new MarketOpened(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var envelope = new Envelope<MarketOpened>(
                message,
                ServiceAddress.Scheduler,
                ServiceAddress.DataService,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.messageBus.DeadLetters);
        }

        [Fact]
        internal void GivenUnaddressedEnvelope_WhenNoSubscribers_HandlesCorrectly()
        {
            // Arrange
            var message = new MarketOpened(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var envelope = new Envelope<MarketOpened>(
                message,
                null,
                ServiceAddress.DataService,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.messageBus.SubscriptionCount);
        }

        [Fact]
        internal void GivenUnaddressedEnvelope_WhenSubscriberSubscribedToBusType_PublishesToSubscriber()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message = new MarketOpened(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var envelope = new Envelope<MarketOpened>(
                message,
                null,
                ServiceAddress.DataService,
                StubZonedDateTime.UnixEpoch());

            this.messageBus.Endpoint.Send(subscribe);

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.receiver.Messages);
        }

        [Fact]
        internal void GivenUnaddressedEnvelope_WhenSubscriberSubscribedToSpecificType_PublishesToSubscriber()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message = new MarketOpened(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var envelope = new Envelope<MarketOpened>(
                message,
                null,
                ServiceAddress.DataService,
                StubZonedDateTime.UnixEpoch());

            this.messageBus.Endpoint.Send(subscribe);

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.receiver.Messages);
        }
    }
}
