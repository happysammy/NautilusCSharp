//--------------------------------------------------------------------------------------------------
// <copyright file="PublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Network;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PublisherTests : TestKit
    {
        private const string TestTopic = "test_topic";

        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly NetworkAddress localHost = NetworkAddress.LocalHost();
        private readonly IEndpoint testEndpoint;

        public PublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.testEndpoint = new ActorEndpoint(this.TestActor);
        }

        [Fact]
        internal void Test_publish_bytes()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:55504";
            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(TestTopic);

            var bytes = Encoding.UTF8.GetBytes("1234");

            var publisher = this.Sys.ActorOf(Props.Create(() => new Publisher(
                this.setupContainer,
                new Label("EventPublisher"),
                TestTopic,
                this.localHost,
                new Port(55504),
                Guid.NewGuid())));

            // Act
            publisher.Tell(bytes);
            this.output.WriteLine("Waiting for published messages...");

            // var message = subscriber.ReceiveFrameBytes();

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);

            // Assert.Equal(topic, Encoding.UTF8.GetBytes(TestTopic));
            // Assert.Equal(bytes, message);

            // Tear Down
            publisher.GracefulStop(TimeSpan.FromMilliseconds(1000));
            subscriber.Unsubscribe(TestTopic);
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
        }
    }
}
