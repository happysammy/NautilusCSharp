//--------------------------------------------------------------------------------------------------
// <copyright file="ConsumerTests.cs" company="Nautech Systems Pty Ltd">
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
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ConsumerTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly NetworkAddress localHost = NetworkAddress.LocalHost();
        private readonly IEndpoint testEndpoint;

        public ConsumerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.testEndpoint = new ActorEndpoint(this.TestActor);
        }

        [Fact]
        internal void Test_can_receive_one_message()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5555";
            var requester = new RequestSocket(TestAddress);
            requester.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                this.localHost,
                new Port(5555),
                Guid.NewGuid())));

            // Act
            requester.SendFrame("MSG");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            requester.Disconnect(TestAddress);
            requester.Dispose();
        }

        [Fact]
        internal void Test_can_receive_multiple_messages()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5556";
            var requester = new RequestSocket(TestAddress);
            requester.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                this.localHost,
                new Port(5556),
                Guid.NewGuid())));

            // Act
            requester.SendFrame("MSG-1");
            var response1 = Encoding.UTF8.GetString(requester.ReceiveFrameBytes());
            requester.SendFrame("MSG-2");
            var response2 = Encoding.UTF8.GetString(requester.ReceiveFrameBytes());

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            Assert.Equal("OK", response1);
            Assert.Equal("OK", response2);

            // Tear Down
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            requester.Disconnect(TestAddress);
            requester.Dispose();
        }

        [Fact]
        internal void Test_can_be_stopped()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5557";
            var requester = new RequestSocket(TestAddress);
            requester.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                this.localHost,
                new Port(5557),
                Guid.NewGuid())));

            // Act
            requester.SendFrame("MSG");
            consumer.Tell(PoisonPill.Instance);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectNoMsg();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(300));
            requester.Disconnect(TestAddress);
            requester.Dispose();
        }

        [Fact]
        internal void Test_can_receive_one_thousand_messages_in_order()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5558";
            var requester = new RequestSocket(TestAddress);
            requester.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                this.localHost,
                new Port(5558),
                Guid.NewGuid())));

            // Act
            for (var i = 0; i < 1000; i++)
            {
                requester.SendFrame($"MSG-{i}");
                requester.ReceiveFrameBytes();
            }

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            requester.Disconnect(TestAddress);
            requester.Dispose();
        }

        [Fact]
        internal void Test_can_receive_one_thousand_messages_from_multiple_request_sockets()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5559";
            var requester1 = new RequestSocket(TestAddress);
            var requester2 = new RequestSocket(TestAddress);
            requester1.Connect(TestAddress);
            requester2.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                this.localHost,
                new Port(5559),
                Guid.NewGuid())));

            // Act
            for (var i = 0; i < 1000; i++)
            {
                requester1.SendFrame($"MSG-{i} from 1");
                requester2.SendFrame($"MSG-{i} from 2");
                requester1.ReceiveFrameBytes();
                requester2.ReceiveFrameBytes();
            }

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();

            // Tear Down
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            requester1.Disconnect(TestAddress);
            requester2.Disconnect(TestAddress);
            requester1.Dispose();
            requester2.Dispose();
        }
    }
}
