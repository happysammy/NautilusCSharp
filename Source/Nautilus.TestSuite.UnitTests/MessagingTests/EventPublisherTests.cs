//--------------------------------------------------------------------------------------------------
// <copyright file="EventPublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.MessagingTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Events;
    using Nautilus.Messaging;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class EventPublisherTests : TestKit
    {
        private const string LocalHost = "127.0.0.1";
        private const string ExecutionEvents = "nautilus_execution_events";
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly IEndpoint testEndpoint;

        public EventPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.testEndpoint = new ActorEndpoint(this.TestActor);
        }

        [Fact]
        internal void Test_can_publish_events()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:56601";
            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(ExecutionEvents);

            var serializer = new MsgPackEventSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var rejected = new OrderRejected(
                order.Symbol,
                order.Id,
                StubZonedDateTime.UnixEpoch(),
                "INVALID_ORDER",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var publisher = this.Sys.ActorOf(Props.Create(() => new EventPublisher(
                this.setupContainer,
                new MsgPackEventSerializer(),
                LocalHost,
                56601)));

            // Act
            publisher.Tell(rejected);
            this.output.WriteLine("Waiting for published events...");

            // var topic = subscriber.ReceiveFrameBytes();
            // var eventBytes = subscriber.ReceiveFrameBytes();
            // var @event = serializer.Deserialize(eventBytes);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert.Equal(ExecutionEvents, Encoding.UTF8.GetString(topic));
            // Assert.Equal(rejected, @event);

            // Tear Down
            publisher.GracefulStop(TimeSpan.FromMilliseconds(1000));
            subscriber.Unsubscribe(ExecutionEvents);
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
        }
    }
}
