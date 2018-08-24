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
    using System.Threading.Tasks;
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
        private const string TestAddress = "tcp://127.0.0.1:5555";
        private readonly ITestOutputHelper output;
        private readonly ActorSystem testActorSystem;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly DealerSocket testDealer1;
        private readonly IEndpoint testEndpoint;

        public ConsumerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.testActorSystem = ActorSystem.Create(nameof(ConsumerTests));

            var setupFactory = new StubSetupContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.testEndpoint = new ActorEndpoint(this.TestActor);

            this.testDealer1 = new DealerSocket(TestAddress);
            this.testDealer1.Connect(TestAddress);
        }

        [Fact]
        internal void Test_consumer_can_receive_one_message()
        {
            // Arrange
            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                new Label("CommandConsumer"),
                this.setupContainer,
                TestAddress,
                Guid.NewGuid(),
                this.testEndpoint)));

            Task.Delay(100).Wait();

            // Act
            this.testDealer1.SendFrame("MSG");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            this.testDealer1.Disconnect(TestAddress);
            this.testDealer1.Dispose();
        }

        [Fact]
        internal void Test_consumer_can_receive_multiple_messages()
        {
            // Arrange
            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                new Label("CommandConsumer"),
                this.setupContainer,
                TestAddress,
                Guid.NewGuid(),
                this.testEndpoint)));

            Task.Delay(100).Wait();

            // Act
            this.testDealer1.SendFrame("MSG");
            this.testDealer1.SendFrame("MSG");
            this.testDealer1.SendFrame("MSG");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectMsg<byte[]>();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            this.testDealer1.Disconnect(TestAddress);
            this.testDealer1.Dispose();
        }

        [Fact]
        internal void Test_consumer_can_be_stopped_by_receiving_poison_pill()
        {
            // Arrange
            var consumer = this.Sys.ActorOf(Props.Create(() => new Consumer(
                new Label("CommandConsumer"),
                this.setupContainer,
                TestAddress,
                Guid.NewGuid(),
                this.testEndpoint)));

            Task.Delay(100).Wait();

            // Act
            this.testDealer1.SendFrame("MSG");
            consumer.Tell(PoisonPill.Instance);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            this.ExpectNoMsg();
            consumer.GracefulStop(TimeSpan.FromMilliseconds(1000));
            this.testDealer1.Disconnect(TestAddress);
            this.testDealer1.Dispose();
        }
    }
}
