//--------------------------------------------------------------------------------------------------
// <copyright file="MessageServerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Mocks;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageServerTests : IDisposable
    {
        private readonly IComponentryContainer container;
        private readonly ISerializer<Request> requestSerializer;
        private readonly ISerializer<Response> responseSerializer;

        public MessageServerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
        }

        [Fact]
        internal void InstantiatedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(55555));

            // Assert
            Assert.Equal("tcp://127.0.0.1:55555", server.NetworkAddress.ToString());
            Assert.Equal(ComponentState.Initialized, server.ComponentState);
            Assert.Equal(0, server.CountReceived);
            Assert.Equal(0, server.CountSent);
        }

        [Fact]
        internal void StartedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(55556));
            server.Start().Wait();

            // Assert
            Assert.Equal(ComponentState.Running, server.ComponentState);

            // Tear Down
            server.Stop().Wait();
        }

        [Fact]
        internal void GivenMessage_WhichIsEmptyBytes_RespondsWithMessageRejected()
        {
            // Arrange
            const int testPort = 55557;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester1 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            requester1.SendMultipartBytes(BitConverter.GetBytes(0U), new byte[] { });
            var response1 = this.responseSerializer.Deserialize(requester1.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(MessageRejected), response1.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester1.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenMessage_WhichHasIncorrectFrameCount_RespondsWithMessageRejected()
        {
            // Arrange
            const int testPort = 55557;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester1 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            requester1.SendMultipartBytes(BitConverter.GetBytes(0U), new byte[] { }, new byte[] { }); // Two payloads incorrect
            var response1 = this.responseSerializer.Deserialize(requester1.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(MessageRejected), response1.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester1.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenMessage_WhichIsInvalidForThisPort_RespondsWithMessageRejected()
        {
            // Arrange
            const int testPort = 55558;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requesters to connect

            // Act
            requester1.SendMultipartBytes(BitConverter.GetBytes(3U), Encoding.UTF8.GetBytes("WOW"));
            var response1 = this.responseSerializer.Deserialize(requester1.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(MessageRejected), response1.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester1.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenConnectMessage_WhenNotAlreadyConnected_SendsConnectedResponseToSender()
        {
            // Arrange
            const int testPort = 55559;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            var message = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized = this.requestSerializer.Serialize(message);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized.Length), serialized);
            var response = (Connected)this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(Connected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);
            Assert.Equal("Trader-001 connected to session Trader-001-1970-01-01-0.", response.Message);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenDisconnectMessage_WhenConnected_SendsDisconnectedToSender()
        {
            // Arrange
            const int testPort = 55560;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);

            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            var connect = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized1 = this.requestSerializer.Serialize(connect);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized1.Length), serialized1);
            var response1 = (Connected)this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            var disconnect = new Disconnect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized2 = this.requestSerializer.Serialize(disconnect);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized2.Length), serialized2);
            var response2 = (Disconnected)this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(Connected), response1.Type);
            Assert.Equal(typeof(Disconnected), response2.Type);
            Assert.Equal(2, server.CountReceived);
            Assert.Equal(2, server.CountSent);
            Assert.Equal("Trader-001 connected to session Trader-001-1970-01-01-0.", response1.Message);
            Assert.Equal("Trader-001 disconnected from session Trader-001-1970-01-01-0.", response2.Message);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenOneMessage_SendsResponseToSender()
        {
            // Arrange
            const int testPort = 55561;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            var message = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized = this.requestSerializer.Serialize(message);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized.Length), this.requestSerializer.Serialize(message));
            var response = this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(Connected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void GivenMultipleMessages_StoresAndSendsResponsesToSender()
        {
            // Arrange
            const int testPort = 55562;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            var message1 = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new Connect(
                new TraderId("Trader", "002"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized1 = this.requestSerializer.Serialize(message1);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized1.Length), serialized1);
            var response1 = this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            var serialized2 = this.requestSerializer.Serialize(message2);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized2.Length), serialized2);
            var response2 = this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            // Assert
            Assert.Equal(typeof(Connected), response1.Type);
            Assert.Equal(typeof(Connected), response2.Type);
            Assert.Equal(2, server.CountReceived);
            Assert.Equal(2, server.CountSent);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void ServerCanBeStopped()
        {
            // Arrange
            const int testPort = 55563;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            var message1 = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var serialized = this.requestSerializer.Serialize(message1);
            requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized.Length), serialized);
            var response1 = this.responseSerializer.Deserialize(requester.ReceiveMultipartBytes()[1]);

            // Act
            server.Stop().Wait();

            var message2 = new Connect(
                new TraderId("Trader", "001"),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());
            requester.SendFrame(this.requestSerializer.Serialize(message2));

            // Assert
            Assert.Equal(typeof(Connected), response1.Type);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Dispose();  // server already stopped
        }

        [Fact]
        internal void Given1000Messages_StoresAndSendsResponsesToSenderInOrder()
        {
            // Arrange
            const int testPort = 55564;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester = new RequestSocket(testAddress);
            requester.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requester to connect

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var message = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i}" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                var serialized = this.requestSerializer.Serialize(message);
                requester.SendMultipartBytes(BitConverter.GetBytes((uint)serialized.Length), serialized);
                requester.ReceiveMultipartBytes();
            }

            server.Stop().Wait();

            // Assert
            Assert.Equal(1000, server.ReceivedMessages.Count);
            Assert.Equal(1000, server.CountReceived);
            Assert.Equal(1000, server.CountSent);
            Assert.Equal("TEST-999", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);

            // Tear Down
            requester.Disconnect(testAddress);
            requester.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }

        [Fact]
        internal void Given1000Messages_FromDifferentSenders_StoresAndSendsResponsesToSendersInOrder()
        {
            // Arrange
            const int testPort = 55565;
            var testAddress = "tcp://127.0.0.1:" + testPort;

            var server = new MockMessageServer(
                this.container,
                EncryptionSettings.None(),
                NetworkAddress.LocalHost,
                new Port(testPort));
            server.Start().Wait();

            var requester1 = new RequestSocket(testAddress);
            var requester2 = new RequestSocket(testAddress);
            requester1.Connect(testAddress);
            requester2.Connect(testAddress);

            Task.Delay(100).Wait(); // Allow requesters to connect

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var message1 = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i} from 1" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                var message2 = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i} from 2" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                var serialized1 = this.requestSerializer.Serialize(message1);
                requester1.SendMultipartBytes(BitConverter.GetBytes(serialized1.Length), serialized1);
                requester1.ReceiveMultipartBytes();

                var serialized2 = this.requestSerializer.Serialize(message2);
                requester2.SendMultipartBytes(BitConverter.GetBytes(serialized2.Length), serialized2);
                requester2.ReceiveMultipartBytes();
            }

            // Assert
            Assert.Equal(2000, server.ReceivedMessages.Count);
            Assert.Equal(2000, server.CountReceived);
            Assert.Equal(2000, server.CountSent);
            Assert.Equal("TEST-999 from 2", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-999 from 1", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998 from 2", server.ReceivedMessages[^3].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998 from 1", server.ReceivedMessages[^4].Query.FirstOrDefault().Value);

            // Tear Down
            requester1.Disconnect(testAddress);
            requester2.Disconnect(testAddress);
            requester1.Dispose();
            requester2.Dispose();
            server.Stop().Wait();
            server.Dispose();
        }
    }
}
