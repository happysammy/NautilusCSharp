//--------------------------------------------------------------------------------------------------
// <copyright file="PublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PublisherTests
    {
        private const string TestTopic = "test_topic";

        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly NetworkAddress localHost = NetworkAddress.LocalHost();

        public PublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
        }

        [Fact]
        internal void InitializedPublisher_HasCorrectServerAddress()
        {
            // Arrange
            var publisher = new MockPublisher(
                this.setupContainer,
                this.localHost,
                new NetworkPort(55504));

            // Act
            var result = publisher.ServerAddress;

            // Assert
            Assert.Equal("tcp://127.0.0.1:55504", publisher.ServerAddress.Value);
        }

        [Fact]
        internal void GivenMessageToPublish_WhenMessageValid_PublishesToSubscriber()
        {
            // Arrange
            var publisher = new MockPublisher(
                this.setupContainer,
                this.localHost,
                new NetworkPort(55504));
            publisher.Start();

            const string testAddress = "tcp://localhost:55504";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(TestTopic);
            Task.Delay(100).Wait();

            // Act
            const string message = "1234,1234";
            publisher.Endpoint.Send((TestTopic, message));

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(TestTopic, Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(message, Encoding.UTF8.GetString(receivedMessage));

            // Tear Down
            subscriber.Unsubscribe(TestTopic);
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
        }
    }
}
