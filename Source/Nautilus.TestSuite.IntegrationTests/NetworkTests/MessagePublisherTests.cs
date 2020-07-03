//--------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisherTests.cs" company="Nautech Systems Pty Ltd">
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
using System.Threading.Tasks;
using Nautilus.Common.Data;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Mocks;
using NetMQ;
using NetMQ.Sockets;
using Xunit;
using Xunit.Abstractions;
using Encoding = System.Text.Encoding;

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessagePublisherTests : NetMQTestBase
    {
        private const string TestTopic = "TEST";
        private readonly IComponentryContainer container;

        public MessagePublisherTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
        }

        [Fact]
        internal void InitializedPublisher_IsInCorrectState()
        {
            // Arrange
            // Act
            var publisher = new MockDataPublisher(
                this.container,
                DataBusFactory.Create(this.container),
                EncryptionSettings.None(),
                ZmqNetworkAddress.LocalHost(new Port(55555)));

            // Assert
            Assert.Equal(ComponentState.Initialized, publisher.ComponentState);
            Assert.Equal(0, publisher.SentCount);
        }

        [Fact]
        internal void GivenMessageToPublish_WhenMessageValid_PublishesToSubscriber()
        {
            // Arrange
            var publisher = new MockDataPublisher(
                this.container,
                DataBusFactory.Create(this.container),
                EncryptionSettings.None(),
                ZmqNetworkAddress.LocalHost(new Port(55555)));
            publisher.Start().Wait();

            const string testAddress = "tcp://localhost:55555";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(TestTopic);

            Task.Delay(100).Wait(); // Allow sockets to subscribe

            // Act
            const string toSend = "1234,1234";
            publisher.Endpoint.SendAsync((TestTopic, toSend));

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(TestTopic, Encoding.UTF8.GetString(topic));
            Assert.Equal(toSend, Encoding.UTF8.GetString(message));
            Assert.Equal(ComponentState.Running, publisher.ComponentState);
            Assert.Equal(1, publisher.SentCount);

            // Tear Down
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop().Wait();
            publisher.Dispose();
        }
    }
}
