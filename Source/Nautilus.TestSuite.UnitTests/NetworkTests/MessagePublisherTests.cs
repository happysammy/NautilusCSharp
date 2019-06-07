//--------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class MessagePublisherTests
    {
        private const string TEST_TOPIC = "TEST";

        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;

        public MessagePublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
        }

        [Fact]
        internal void InitializedPublisher_IsInCorrectState()
        {
            // Arrange
            // Act
            var publisher = new MockPublisher(
                this.setupContainer,
                NetworkAddress.LocalHost,
                new NetworkPort(55555));

            // Assert
            Assert.Equal("tcp://127.0.0.1:55555", publisher.ServerAddress.Value);
            Assert.Equal(State.Init, publisher.State);
            Assert.Equal(0, publisher.PublishedCount);
        }

        [Fact]
        internal void GivenMessageToPublish_WhenMessageValid_PublishesToSubscriber()
        {
            // Arrange
            var publisher = new MockPublisher(
                this.setupContainer,
                NetworkAddress.LocalHost,
                new NetworkPort(55555));
            publisher.Start();

            Task.Delay(100).Wait();
            const string testAddress = "tcp://localhost:55555";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(TEST_TOPIC);
            Task.Delay(100).Wait();

            // Act
            const string message = "1234,1234";
            publisher.Endpoint.Send((TEST_TOPIC, message));

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert
            Assert.Equal(TEST_TOPIC, Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(message, Encoding.UTF8.GetString(receivedMessage));
            Assert.Equal(State.Running, publisher.State);
            Assert.Equal(1, publisher.PublishedCount);

            // Tear Down
            subscriber.Unsubscribe(TEST_TOPIC);
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
