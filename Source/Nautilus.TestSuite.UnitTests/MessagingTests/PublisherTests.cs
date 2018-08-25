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
    using System.Collections.Generic;
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
    public class PublisherTests : TestKit
    {
        private const string LocalHost = "127.0.0.1";
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly List<byte[]> receivedBytes;

        public PublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;

            this.receivedBytes = new List<byte[]>();
        }

        [Fact]
        internal void Test_consumer_can_receive_one_message()
        {
            // Arrange
            var listen = Task.Run(this.ListenForBytes);

            var eventBytes = new byte[100];

            var publisher = this.Sys.ActorOf(Props.Create(() => new Publisher(
                this.setupContainer,
                new Label("EventPublisher"),
                LocalHost,
                5555,
                Guid.NewGuid())));

            // Act
            publisher.Tell(eventBytes);

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Single(this.receivedBytes);
            publisher.GracefulStop(TimeSpan.FromMilliseconds(1000));
        }

        private Task ListenForBytes()
        {
            var socket = new ResponseSocket();
            socket.Connect("tcp://127.0.0.1:5555");

            while (true)
            {
                var message = socket.ReceiveFrameBytes();

                this.receivedBytes.Add(message);

                socket.Disconnect("tcp://127.0.0.1:5555");
                socket.Dispose();
            }
        }
    }
}
