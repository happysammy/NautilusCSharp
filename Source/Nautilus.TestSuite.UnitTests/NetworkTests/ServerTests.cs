//--------------------------------------------------------------------------------------------------
// <copyright file="RouterTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System;
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
    public class ServerTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer container;
        private readonly MockLoggingAdapter mockLoggingAdapter;
        private readonly MockMessagingAgent testReceiver;

        public ServerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            this.container = containerFactory.Create();
            this.mockLoggingAdapter = containerFactory.LoggingAdapter;
            this.testReceiver = new MockMessagingAgent();
            this.testReceiver.RegisterHandler<string>(this.testReceiver.OnMessage);
        }

        [Fact]
        internal void Test_can_receive_one_message()
        {
            // Arrange
            var server = new MockServer(
                this.testReceiver.Endpoint,
                this.container,
                NetworkAddress.LocalHost,
                new NetworkPort(5555),
                Guid.NewGuid());
            server.Start();

            const string testAddress = "tcp://127.0.0.1:5555";
            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            // Act
            var message = new MockMessage("TEST", Guid.NewGuid(), StubZonedDateTime.UnixEpoch());
            var serialized = new MockSerializer().Serialize(message);
            requester.SendFrame(serialized);

            Task.Delay(100).Wait();

            // var response = requester.ReceiveMultipartMessage();

            // Assert
            // this.output.WriteLine(Encoding.UTF8.GetString(response));
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Contains("MSG", this.testReceiver.Messages);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }

        [Fact]
        internal void Test_can_receive_multiple_messages()
        {
            // Arrange
            var server = new MockServer(
                this.testReceiver.Endpoint,
                this.container,
                NetworkAddress.LocalHost,
                new NetworkPort(5556),
                Guid.NewGuid());
            server.Start();

            const string testAddress = "tcp://127.0.0.1:5556";
            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            // Act
            requester.SendFrame("MSG-1");
            var response1 = Encoding.UTF8.GetString(requester.ReceiveFrameBytes());
            requester.SendFrame("MSG-2");
            var response2 = Encoding.UTF8.GetString(requester.ReceiveFrameBytes());

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Contains("MSG-1", this.testReceiver.Messages);
            Assert.Contains("MSG-2", this.testReceiver.Messages);
            Assert.Equal("OK", response1);
            Assert.Equal("OK", response2);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }

        [Fact]
        internal void Test_can_be_stopped()
        {
            // Arrange
            var server = new MockServer(
                this.testReceiver.Endpoint,
                this.container,
                NetworkAddress.LocalHost,
                new NetworkPort(5557),
                Guid.NewGuid());
            server.Start();

            const string testAddress = "tcp://127.0.0.1:5557";
            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            requester.SendFrame("MSG");
            requester.ReceiveFrameBytes();
            Task.Delay(100).Wait();

            // Act
            server.Stop();

            requester.SendFrame("AFTER-STOPPED");

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Contains("MSG", this.testReceiver.Messages);
            Assert.DoesNotContain("AFTER-STOPPED", this.testReceiver.Messages);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Close();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }

        [Fact]
        internal void Test_can_receive_one_thousand_messages_in_order()
        {
            // Arrange
            var server = new MockServer(
                this.testReceiver.Endpoint,
                this.container,
                NetworkAddress.LocalHost,
                new NetworkPort(5558),
                Guid.NewGuid());
            server.Start();

            const string testAddress = "tcp://127.0.0.1:5558";
            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            // Act
            for (var i = 0; i < 1000; i++)
            {
                requester.SendFrame($"MSG-{i}");
                requester.ReceiveFrameBytes();
            }

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(1000, this.testReceiver.Messages.Count);
            Assert.Equal("MSG-999", this.testReceiver.Messages[this.testReceiver.Messages.Count - 1]);
            Assert.Equal("MSG-998", this.testReceiver.Messages[this.testReceiver.Messages.Count - 2]);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }

        [Fact]
        internal void Test_can_receive_one_thousand_messages_from_multiple_request_sockets()
        {
            // Arrange
            var server = new MockServer(
                this.testReceiver.Endpoint,
                this.container,
                NetworkAddress.LocalHost,
                new NetworkPort(5559),
                Guid.NewGuid());
            server.Start();

            const string testAddress = "tcp://127.0.0.1:5559";
            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            // Act
            for (var i = 0; i < 1000; i++)
            {
                requester1.SendFrame($"MSG-{i} from 1");
                requester2.SendFrame($"MSG-{i} from 2");
                requester1.ReceiveFrameBytes();
                requester2.ReceiveFrameBytes();
            }

            Task.Delay(100).Wait();

            // Assert
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Assert.Equal(2000, this.testReceiver.Messages.Count);
            Assert.Equal("MSG-999 from 2", this.testReceiver.Messages[this.testReceiver.Messages.Count - 1]);
            Assert.Equal("MSG-999 from 1", this.testReceiver.Messages[this.testReceiver.Messages.Count - 2]);
            Assert.Equal("MSG-998 from 2", this.testReceiver.Messages[this.testReceiver.Messages.Count - 3]);
            Assert.Equal("MSG-998 from 1", this.testReceiver.Messages[this.testReceiver.Messages.Count - 4]);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester2.Disconnect(testAddress);
            requester1.Dispose();
            requester2.Dispose();
            server.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
