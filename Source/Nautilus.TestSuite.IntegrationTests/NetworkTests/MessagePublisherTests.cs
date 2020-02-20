//--------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;
    using Encoding = System.Text.Encoding;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessagePublisherTests : IDisposable
    {
        private const string TestTopic = "TEST";

        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer container;
        private readonly MockLogger logger;

        public MessagePublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.container = containerFactory.Create();
            this.logger = containerFactory.Logger;
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
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
                NetworkAddress.LocalHost,
                new NetworkPort(55555));

            // Assert
            Assert.Equal("tcp://127.0.0.1:55555", publisher.NetworkAddress.Value);
            Assert.Equal(ComponentState.Initialized, publisher.ComponentState);
            Assert.Equal(0, publisher.PublishedCount);
        }

        [Fact]
        internal void GivenMessageToPublish_WhenMessageValid_PublishesToSubscriber()
        {
            // Arrange
            var publisher = new MockDataPublisher(
                this.container,
                DataBusFactory.Create(this.container),
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new NetworkPort(55555));
            publisher.Start();
            Task.Delay(100).Wait(); // Allow publisher to start

            const string testAddress = "tcp://localhost:55555";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(TestTopic);

            Task.Delay(100).Wait(); // Allow sockets to subscribe

            // Act
            const string message = "1234,1234";
            publisher.Endpoint.Send((TestTopic, message));

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(TestTopic, Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(message, Encoding.UTF8.GetString(receivedMessage));
            Assert.Equal(ComponentState.Running, publisher.ComponentState);
            Assert.Equal(1, publisher.PublishedCount);

            // Tear Down
            LogDumper.DumpWithDelay(this.logger, this.output);
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            publisher.Dispose();
        }
    }
}
