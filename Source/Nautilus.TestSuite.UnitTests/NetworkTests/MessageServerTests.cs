//--------------------------------------------------------------------------------------------------
// <copyright file="MessageServerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;
    using Encoding = System.Text.Encoding;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MessageServerTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer container;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MockSerializer serializer;
        private readonly MsgPackResponseSerializer responseSerializer;

        public MessageServerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.serializer = new MockSerializer();
            this.responseSerializer = new MsgPackResponseSerializer();
        }

        [Fact]
        internal void InitializedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(55555),
                Guid.NewGuid());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal("tcp://127.0.0.1:55555", server.ServerAddress.ToString());
            Assert.Equal(State.Initialized, server.State);
            Assert.Equal(0, server.ReceivedCount);
            Assert.Equal(0, server.SentCount);

            // Tear Down
            server.Stop();
        }

        [Fact]
        internal void StartedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(55556),
                Guid.NewGuid());
            server.Start();

            Task.Delay(100).Wait(); // Allow server to start

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(State.Running, server.State);

            // Tear Down
            server.Stop();
        }

        [Fact]
        internal void GivenMessage_WhichIsEmptyBytes_RespondsWithMessageRejected()
        {
            // Arrange
            const int testPort = 55557;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            requester1.SendMultipartBytes(new byte[] { }, new byte[] { });
            var response1 = this.responseSerializer.Deserialize(requester1.ReceiveFrameBytes());

            var message = new MockMessage(
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
            requester2.SendFrame(this.serializer.Serialize(message));
            var response2 = this.responseSerializer.Deserialize(requester2.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(MessageRejected), response1.Type);
            Assert.Equal(typeof(MessageReceived), response2.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(2, server.SentCount);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester2.Disconnect(testAddress);
            server.Stop();
        }

        [Fact]
        internal void GivenMessage_WhichIsInvalidForThisPort_RespondsWithMessageRejected()
        {
            // Arrange
            const int testPort = 55558;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            requester1.SendFrame(Encoding.UTF8.GetBytes("WOW"));
            var response1 = this.responseSerializer.Deserialize(requester1.ReceiveFrameBytes());

            var message = new MockMessage(
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
            requester2.SendFrame(this.serializer.Serialize(message));
            var response2 = this.responseSerializer.Deserialize(requester2.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(MessageRejected), response1.Type);
            Assert.Equal(typeof(MessageReceived), response2.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(2, server.SentCount);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester2.Disconnect(testAddress);
            server.Stop();
        }

        [Fact]
        internal void GivenOneMessage_StoresAndSendsResponseToSender()
        {
            // Arrange
            const int testPort = 55559;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            var message = new MockMessage(
                "TEST",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            requester.SendFrame(this.serializer.Serialize(message));
            var response = this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(MessageReceived), response.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(1, server.SentCount);
            Assert.Contains(message, server.ReceivedMessages);

            // Tear Down
            requester.Disconnect(testAddress);
            server.Stop();
        }

        [Fact]
        internal void GivenMultipleMessages_StoresAndSendsResponsesToSender()
        {
            // Arrange
            const int testPort = 55560;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            var message1 = new MockMessage(
                "TEST1",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new MockMessage(
                "TEST2",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            requester.SendFrame(this.serializer.Serialize(message1));
            var response1 = this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            requester.SendFrame(this.serializer.Serialize(message2));
            var response2 = this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Contains(message1, server.ReceivedMessages);
            Assert.Contains(message2, server.ReceivedMessages);
            Assert.Equal(typeof(MessageReceived), response1.Type);
            Assert.Equal(typeof(MessageReceived), response2.Type);
            Assert.Equal(2, server.ReceivedCount);
            Assert.Equal(2, server.SentCount);

            // Tear Down
            requester.Disconnect(testAddress);
            server.Stop();
        }

        [Fact]
        internal void ServerCanBeStopped()
        {
            // Arrange
            const int testPort = 55561;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            var message1 = new MockMessage(
                "TEST1",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            requester.SendFrame(this.serializer.Serialize(message1));
            var response1 = this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());

            // Act
            server.Stop();
            Task.Delay(100).Wait(); // Allow server to stop

            var message2 = new MockMessage(
                "AFTER-STOP",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
            requester.SendFrame(this.serializer.Serialize(message2));

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(typeof(MessageReceived), response1.Type);
            Assert.Contains(message1, server.ReceivedMessages);
            Assert.DoesNotContain(message2, server.ReceivedMessages);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Close();
        }

        [Fact]
        internal void Given1000Messages_StoresAndSendsResponsesToSenderInOrder()
        {
            // Arrange
            const int testPort = 55562;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var message = new MockMessage(
                    $"TEST-{i}",
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                requester.SendFrame(this.serializer.Serialize(message));
                this.responseSerializer.Deserialize(requester.ReceiveFrameBytes());
            }

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(1000, server.ReceivedMessages.Count);
            Assert.Equal(1000, server.ReceivedCount);
            Assert.Equal(1000, server.SentCount);
            Assert.Equal("TEST-999", server.ReceivedMessages[server.ReceivedMessages.Count - 1].Payload);
            Assert.Equal("TEST-998", server.ReceivedMessages[server.ReceivedMessages.Count - 2].Payload);

            // Tear Down
            requester.Disconnect(testAddress);
            server.Stop();
        }

        [Fact]
        internal void Given1000Messages_FromDifferentSenders_StoresAndSendsResponsesToSendersInOrder()
        {
            // Arrange
            const int testPort = 55563;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                NetworkHost.LocalHost,
                new NetworkPort(testPort),
                Guid.NewGuid());
            server.Start();

            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow sockets to initiate

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var message1 = new MockMessage(
                    $"TEST-{i} from 1",
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                var message2 = new MockMessage(
                    $"TEST-{i} from 2",
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                requester1.SendFrame(this.serializer.Serialize(message1));
                this.responseSerializer.Deserialize(requester1.ReceiveFrameBytes());
                requester2.SendFrame(this.serializer.Serialize(message2));
                this.responseSerializer.Deserialize(requester2.ReceiveFrameBytes());
            }

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(2000, server.ReceivedMessages.Count);
            Assert.Equal(2000, server.ReceivedCount);
            Assert.Equal(2000, server.SentCount);
            Assert.Equal("TEST-999 from 2", server.ReceivedMessages[server.ReceivedMessages.Count - 1].Payload);
            Assert.Equal("TEST-999 from 1", server.ReceivedMessages[server.ReceivedMessages.Count - 2].Payload);
            Assert.Equal("TEST-998 from 2", server.ReceivedMessages[server.ReceivedMessages.Count - 3].Payload);
            Assert.Equal("TEST-998 from 1", server.ReceivedMessages[server.ReceivedMessages.Count - 4].Payload);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester2.Disconnect(testAddress);
            server.Stop();
        }
    }
}
