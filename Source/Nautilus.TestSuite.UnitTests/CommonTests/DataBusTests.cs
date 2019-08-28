//--------------------------------------------------------------------------------------------------
// <copyright file="DataBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Data;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DataBusTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockMessagingAgent receiver;
        private readonly DataBus<Tick> dataBus;

        public DataBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();

            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.receiver = new MockMessagingAgent();
            this.dataBus = new DataBus<Tick>(container);
            this.receiver.RegisterHandler<Tick>(this.receiver.OnMessage);
        }

        [Fact]
        internal void InitializedMessageBus_IsInExpectedState()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal("DataBus<Tick>", this.dataBus.Name.ToString());
            Assert.Equal(typeof(Tick), this.dataBus.BusType);
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
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
            this.dataBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenSubscribe_WhenTypeIsValid_SubscribesCorrectly()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(this.receiver.Mailbox.Address, this.dataBus.Subscriptions);
            Assert.Equal(1, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenSubscribe_WhenAlreadySubscribed_DoesNotDuplicateSubscription()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(subscribe);
            this.dataBus.Endpoint.Send(subscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(1, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenSubscribe_WithMultipleSubscribers_SubscribesCorrectly()
        {
            // Arrange
            var receiver2 = new MockMessagingAgent("TickReceiver2");

            var subscribe1 = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<Type>(
                typeof(Tick),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(subscribe1);
            this.dataBus.Endpoint.Send(subscribe2);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(this.receiver.Mailbox.Address, this.dataBus.Subscriptions);
            Assert.Contains(receiver2.Mailbox.Address, this.dataBus.Subscriptions);
            Assert.Equal(2, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenNoSubscribers_HandlesCorrectly()
        {
            // Arrange
            var unsubscribe = new Unsubscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenUnsubscribe_WhenSubscribed_RemovesSubscription()
        {
            // Arrange
            var subscribe = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(subscribe);
            this.dataBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenMultipleSubscribeAndUnsubscribe_HandlesCorrectly()
        {
            // Arrange
            var receiver2 = new MockMessagingAgent("TickReceiver2");

            var subscribe1 = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<Type>(
                typeof(Tick),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<Type>(
                typeof(Tick),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.dataBus.Endpoint.Send(subscribe1);
            this.dataBus.Endpoint.Send(subscribe2);
            this.dataBus.Endpoint.Send(unsubscribe);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(1, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenData_WhenNoSubscribers_DoesNothing()
        {
            // Arrange
            var tick = StubTickFactory.Create(new Symbol("AUDUSD", new Venue("FXCM")));

            // Act
            this.dataBus.Endpoint.Send(tick);
            this.dataBus.Endpoint.Send(tick);
            this.dataBus.Endpoint.Send(tick);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenData_WhenSubscribers_SendsDataToSubscriber()
        {
            // Arrange
            var receiver2 = new MockMessagingAgent("TickReceiver2");
            receiver2.RegisterHandler<Tick>(receiver2.OnMessage);

            var subscribe1 = new Subscribe<Type>(
                typeof(Tick),
                this.receiver.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<Type>(
                typeof(Tick),
                receiver2.Mailbox,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var tick = StubTickFactory.Create(new Symbol("AUDUSD", new Venue("FXCM")));

            // Act
            this.dataBus.Endpoint.Send(subscribe1);
            this.dataBus.Endpoint.Send(subscribe2);
            this.dataBus.Endpoint.Send(tick);
            this.dataBus.Endpoint.Send(tick);
            this.dataBus.Endpoint.Send(tick);

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(tick, this.receiver.Messages);
            Assert.Contains(tick, receiver2.Messages);
            Assert.Equal(3, this.receiver.Messages.Count);
            Assert.Equal(3, receiver2.Messages.Count);
        }
    }
}
