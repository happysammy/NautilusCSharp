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
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
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
        private const string LocalHost = "127.0.0.1";
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
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
        internal void Test_consumer_can_receive_one_message()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5555";
            var dealer = new DealerSocket(TestAddress);
            dealer.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                LocalHost,
                5555,
                Guid.NewGuid())));

            // Act
            dealer.SendFrame("MSG");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            dealer.Disconnect(TestAddress);
            dealer.Dispose();
        }

        [Fact]
        internal void Test_consumer_can_receive_multiple_messages()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5556";
            var dealer = new DealerSocket(TestAddress);
            dealer.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                LocalHost,
                5556,
                Guid.NewGuid())));

            // Act
            dealer.SendFrame("MSG");
            dealer.SendFrame("MSG");
            dealer.SendFrame("MSG");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            dealer.Disconnect(TestAddress);
            dealer.Dispose();
        }

        [Fact]
        internal void Test_consumer_can_be_stopped()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5557";
            var dealer = new DealerSocket(TestAddress);
            dealer.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                LocalHost,
                5557,
                Guid.NewGuid())));

            // Act
            dealer.SendFrame("MSG");
            consumer.Tell(PoisonPill.Instance);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectNoMsg();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(300));
            dealer.Disconnect(TestAddress);
            dealer.Dispose();
        }

        [Fact]
        internal void Test_consumer_can_receive_one_hundred_thousand_messages_in_order()
        {
            // Arrange
            const string TestAddress = "tcp://127.0.0.1:5558";
            var dealer = new DealerSocket(TestAddress);
            dealer.Connect(TestAddress);

            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                this.setupContainer,
                this.testEndpoint,
                new Label("CommandConsumer"),
                LocalHost,
                5558,
                Guid.NewGuid())));

            // Act
            for (var i = 0; i < 100000; i++)
            {
                dealer.SendFrame($"MSG-{i}");
            }

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            dealer.Disconnect(TestAddress);
            dealer.Dispose();
        }
    }
}
