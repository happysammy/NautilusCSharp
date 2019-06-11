//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
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
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly MockMessagingAgent mockReceiver;
        private readonly MessageBus<Event> messageBus;

        public MessageBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.mockLoggingAdapter = containerFactory.LoggingAdapter;
            this.mockReceiver = new MockMessagingAgent();
            this.messageBus = new MessageBus<Event>(container);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { DataServiceAddress.Core, this.mockReceiver.Endpoint },
                { DataServiceAddress.BarAggregationController, this.mockReceiver.Endpoint },
                { DataServiceAddress.DatabaseTaskManager, this.mockReceiver.Endpoint },
            };

            this.messageBus.Endpoint.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                Guid.NewGuid(),
                containerFactory.Clock.TimeNow()));

            this.mockReceiver.RegisterHandler<IEnvelope>(this.mockReceiver.OnMessage);
        }

        [Fact]
        internal void InitializedMessageBus_IsInExpectedState()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal("MessageBus<Event>", this.messageBus.Name.ToString());
            Assert.Equal(typeof(Event), this.messageBus.BusMessageType);
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
            Assert.Equal(0, this.messageBus.DeadLetters.Count);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsInvalid_DoesNotSubscribe()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Command),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsBusType_SubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.True(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(1, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenAlreadySubscribedToBusType_DoesNotResubscribe()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.True(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(1, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsSpecificType_SubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.True(this.messageBus.HasSubscribers);
            Assert.Contains(typeof(MarketOpened), this.messageBus.TypeSubscriptions);
            Assert.Equal(1, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(1, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenSubscribe_WhenAlreadySubscribedToSpecificType_DoesNotResubscribe()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(subscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.True(this.messageBus.HasSubscribers);
            Assert.Equal(1, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(1, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenSubscribedToBusType_UnsubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenSubscribedToSpecificType_UnsubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(subscribe);
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenTypeIsInvalid_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(Command),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenNotSubscribedToBusType_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenNotSubscribedToSpecificType_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(unsubscribe);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
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
                DataServiceAddress.BarAggregationController,
                DataServiceAddress.Core,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.mockReceiver.Messages);
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
                DataServiceAddress.Scheduler,
                DataServiceAddress.Core,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

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
                DataServiceAddress.Core,
                StubZonedDateTime.UnixEpoch());

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.False(this.messageBus.HasSubscribers);
            Assert.Equal(0, this.messageBus.TypeSubscriptions.Count);
            Assert.Equal(0, this.messageBus.SubscriptionsAllCount);
            Assert.Equal(0, this.messageBus.SubscriptionsCount);
        }

        [Fact]
        internal void GivenUnaddressedEnvelope_WhenSubscriberSubscribedToBusType_PublishesToSubscriber()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Event),
                this.mockReceiver.Mailbox,
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
                DataServiceAddress.Core,
                StubZonedDateTime.UnixEpoch());

            this.messageBus.Endpoint.Send(subscribe);

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.mockReceiver.Messages);
        }

        [Fact]
        internal void GivenUnaddressedEnvelope_WhenSubscriberSubscribedToSpecificType_PublishesToSubscriber()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(MarketOpened),
                this.mockReceiver.Mailbox,
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
                DataServiceAddress.Core,
                StubZonedDateTime.UnixEpoch());

            this.messageBus.Endpoint.Send(subscribe);

            // Act
            this.messageBus.Endpoint.Send(envelope);

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.Contains(envelope, this.mockReceiver.Messages);
        }
    }
}
