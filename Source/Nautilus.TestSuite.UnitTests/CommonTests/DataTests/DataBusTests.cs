//--------------------------------------------------------------------------------------------------
// <copyright file="DataBusTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests.DataTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class DataBusTests
    {
        private readonly IComponentryContainer container;
        private readonly MockComponent receiver;
        private readonly DataBus<Tick> dataBus;

        public DataBusTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.receiver = new MockComponent(this.container, "1");
            this.dataBus = new DataBus<Tick>(this.container);
            this.receiver.RegisterHandler<Tick>(this.receiver.OnMessage);
            this.dataBus.Start().Wait();
        }

        [Fact]
        internal void InitializedMessageBus_IsInExpectedState()
        {
            // Arrange
            // Act
            // Assert
            Assert.Equal("DataBus<Tick>", this.dataBus.Name.Value);
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
            this.dataBus.Endpoint.SendAsync(subscribe);
            this.dataBus.Stop().Wait();

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
            this.dataBus.Endpoint.SendAsync(subscribe);
            this.dataBus.Stop().Wait();

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
            this.dataBus.Endpoint.SendAsync(subscribe);
            this.dataBus.Endpoint.SendAsync(subscribe);
            this.dataBus.Stop().Wait();

            // Assert
            Assert.Equal(1, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenSubscribe_WithMultipleSubscribers_SubscribesCorrectly()
        {
            // Arrange
            var receiver2 = new MockComponent(this.container, "2");

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
            this.dataBus.Endpoint.SendAsync(subscribe1);
            this.dataBus.Endpoint.SendAsync(subscribe2);
            this.dataBus.Stop().Wait();

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
            this.dataBus.Endpoint.SendAsync(unsubscribe);
            this.dataBus.Stop().Wait();

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
            this.dataBus.Endpoint.SendAsync(subscribe);
            this.dataBus.Endpoint.SendAsync(unsubscribe);
            this.dataBus.Stop().Wait();

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenMultipleSubscribeAndUnsubscribe_HandlesCorrectly()
        {
            // Arrange
            var receiver2 = new MockComponent(this.container, "2");

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
            this.dataBus.Endpoint.SendAsync(subscribe1);
            this.dataBus.Endpoint.SendAsync(subscribe2);
            this.dataBus.Endpoint.SendAsync(unsubscribe).Wait();
            this.dataBus.Stop().Wait();

            // Assert
            Assert.Equal(1, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenData_WhenNoSubscribers_DoesNothing()
        {
            // Arrange
            var tick = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")));

            // Act
            this.dataBus.PostData(tick);
            this.dataBus.PostData(tick);
            this.dataBus.PostData(tick);
            this.dataBus.Stop().Wait();
            this.dataBus.StopData().Wait();

            // Assert
            Assert.Equal(0, this.dataBus.Subscriptions.Count);
        }

        [Fact]
        internal void GivenData_WhenSubscribers_SendsDataToSubscriber()
        {
            // Arrange
            var receiver2 = new MockComponent(this.container, "2");
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

            var tick1 = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")));
            var tick2 = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")), StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(1));
            var tick3 = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")), StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(2));

            // Act
            this.dataBus.Endpoint.SendAsync(subscribe1);
            this.dataBus.Endpoint.SendAsync(subscribe2);
            Task.Delay(100).Wait(); // Allow subscriptions

            this.dataBus.PostData(tick1);
            this.dataBus.PostData(tick2);
            this.dataBus.PostData(tick3);

            this.dataBus.Stop().Wait();
            this.dataBus.StopData().Wait();
            this.receiver.Stop().Wait();
            receiver2.Stop().Wait();

            // Assert
            Assert.Contains(tick1, this.receiver.Messages);
            Assert.Contains(tick2, this.receiver.Messages);
            Assert.Contains(tick3, this.receiver.Messages);
            Assert.Contains(tick1, receiver2.Messages);
            Assert.Contains(tick2, receiver2.Messages);
            Assert.Contains(tick3, receiver2.Messages);
            Assert.Equal(3, this.receiver.Messages.Count);
            Assert.Equal(3, receiver2.Messages.Count);
        }
    }
}
