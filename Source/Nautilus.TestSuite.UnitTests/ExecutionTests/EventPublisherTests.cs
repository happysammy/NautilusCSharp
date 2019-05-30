//--------------------------------------------------------------------------------------------------
// <copyright file="EventPublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.ExecutionTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.Execution.Network;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class EventPublisherTests
    {
        private const string EXECUTION_EVENTS = "nautilus_execution_events";

        private readonly NetworkAddress localHost = new NetworkAddress("127.0.0.1");
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly IEndpoint testReceiver;

        public EventPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
            this.testReceiver = new MockMessagingAgent().Endpoint;
        }

        [Fact]
        internal void Test_can_publish_events()
        {
            // Arrange
            const string testAddress = "tcp://127.0.0.1:56601";

            var publisher = new EventPublisher(
                this.setupContainer,
                new MsgPackEventSerializer(),
                this.localHost,
                new NetworkPort(56601));
            publisher.Start();

            Task.Delay(100).Wait();

            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe("NAUTILUS");
            Task.Delay(100).Wait();

            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = StubEventMessages.OrderRejectedEvent(order);

            // Act
            publisher.Endpoint.Send(rejected);
            this.output.WriteLine("Waiting for published events...");

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();
            var @event = serializer.Deserialize(message);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            Assert.Equal("NAUTILUS:EXECUTION:O-123456", Encoding.UTF8.GetString(topic));
            Assert.Equal(rejected, @event);

            // Tear Down
            subscriber.Unsubscribe("NAUTILUS");
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
        }
    }
}
