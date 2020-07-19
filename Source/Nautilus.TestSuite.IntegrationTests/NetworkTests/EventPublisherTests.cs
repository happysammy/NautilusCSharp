//--------------------------------------------------------------------------------------------------
// <copyright file="EventPublisherTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Events;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Execution.Network;
using Nautilus.Network;
using Nautilus.Network.Compression;
using Nautilus.Network.Encryption;
using Nautilus.Serialization.MessageSerializers;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Mocks;
using Nautilus.TestSuite.TestKit.Stubs;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class EventPublisherTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;

        public EventPublisherTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            var service = new MockMessageBusProvider(this.container);
        }

        [Fact]
        internal void Test_can_publish_events()
        {
            // Arrange
            const string testAddress = "tcp://127.0.0.1:56601";

            var publisher = new EventPublisher(
                this.container,
                new MsgPackEventSerializer(),
                new BypassCompressor(),
                EncryptionSettings.None(),
                new Label("test-publisher"),
                new Port(56601));
            publisher.Start().Wait();

            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe("Event:Trade:TESTER-001");

            Task.Delay(100).Wait(); // Allow socket to subscribe

            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = StubEventMessageProvider.OrderRejectedEvent(order);
            var tradeEvent = new TradeEvent(TraderId.FromString("TESTER-001"), rejected);

            // Act
            publisher.Endpoint.Send(tradeEvent);
            this.Output.WriteLine("Waiting for published events...");

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();
            var @event = serializer.Deserialize(message);

            // Assert
            Assert.Equal("Event:Trade:TESTER-001", Encoding.UTF8.GetString(topic));
            Assert.Equal(typeof(OrderRejected), @event.GetType());

            // Tear Down
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop().Wait();
            publisher.Dispose();
        }
    }
}
