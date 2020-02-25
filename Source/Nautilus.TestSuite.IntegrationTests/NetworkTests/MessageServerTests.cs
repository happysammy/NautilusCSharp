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
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;
    using Nautilus.Network;
    using Nautilus.Network.Compression;
    using Nautilus.Network.Encryption;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.MessageSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Facades;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageServerTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;
        private readonly ISerializer<Request> requestSerializer;
        private readonly ISerializer<Response> responseSerializer;
        private readonly ICompressor compressor;

        public MessageServerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.requestSerializer = new MsgPackRequestSerializer(new MsgPackQuerySerializer());
            this.responseSerializer = new MsgPackResponseSerializer();
            this.compressor = new CompressorBypass();
        }

        [Fact]
        internal void InstantiatedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                ZmqNetworkAddress.LocalHost(55555));

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
            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                ZmqNetworkAddress.LocalHost(55556));
            server.Start().Wait();

            // Assert
            Assert.Equal(ComponentState.Running, server.ComponentState);

            // Tear Down
            server.Stop().Wait();
        }

        [Fact]
        internal void GivenMessage_WhichHasIncorrectFrameCount_RespondsWithMessageRejected()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55557);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            dealer.Start().Wait();

            // Act
            var response = dealer.SendRaw(new[] { new byte[] { } });

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void GivenMessage_WhichIsEmptyBytes_RespondsWithMessageRejected()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55558);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            dealer.Start().Wait();

            // Act
            var response = dealer.Send("BogusMessage", new byte[] { });

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void GivenMessage_WhichIsInvalidForThisPort_RespondsWithMessageRejected()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55559);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            dealer.Start().Wait();

            // Act
            var response = dealer.SendRaw(new[]
            {
                Encoding.UTF8.GetBytes("WOW"),
                BitConverter.GetBytes(3),
                Encoding.UTF8.GetBytes("Payload"),
            });

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void GivenConnectMessage_WhenNotAlreadyConnected_SendsConnectedResponseToSender()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55560);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress,
                "001");
            dealer.Start().Wait();

            var connect = new Connect(
                dealer.ClientId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var response = (Connected)dealer.Send(connect);

            // Assert
            Assert.Equal(typeof(Connected), response.Type);
            Assert.Equal(1, server.CountReceived);
            Assert.Equal(1, server.CountSent);
            Assert.Equal("TestDealer-001 connected to session TestDealer-001-1970-01-01-0 at tcp://127.0.0.1:55560", response.Message);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void GivenDisconnectMessage_WhenConnected_SendsDisconnectedToSender()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55561);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress,
                "001");
            dealer.Start().Wait();

            var connect = new Connect(
                dealer.ClientId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var response1 = (Connected)dealer.Send(connect);

            var disconnect = new Disconnect(
                dealer.ClientId,
                response1.SessionId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var response2 = (Disconnected)dealer.Send(disconnect);

            // Assert
            Assert.Equal(typeof(Connected), response1.Type);
            Assert.Equal(typeof(Disconnected), response2.Type);
            Assert.Equal(2, server.CountReceived);
            Assert.Equal(2, server.CountSent);
            Assert.Equal("TestDealer-001 connected to session TestDealer-001-1970-01-01-0 at tcp://127.0.0.1:55561", response1.Message);
            Assert.Equal("TestDealer-001 disconnected from session TestDealer-001-1970-01-01-0 at tcp://127.0.0.1:55561", response2.Message);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void GivenMultipleMessages_StoresAndSendsResponsesToCorrectSender()
        {
            // Arrange
            var testAddress = ZmqNetworkAddress.LocalHost(55562);

            var server = new MessageServerFacade(
                this.container,
                this.compressor,
                EncryptionSettings.None(),
                testAddress);
            server.Start().Wait();

            var dealer1 = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress,
                "001");
            dealer1.Start().Wait();

            var dealer2 = new TestDealer(
                this.container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                this.compressor,
                EncryptionSettings.None(),
                testAddress,
                "002");
            dealer2.Start().Wait();

            // Act
            var connect1 = new Connect(
                dealer1.ClientId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var connect2 = new Connect(
                dealer2.ClientId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var response1 = (Connected)dealer1.Send(connect1);
            var response2 = (Connected)dealer2.Send(connect2);

            // Assert
            Assert.Equal("TestDealer-001 connected to session TestDealer-001-1970-01-01-0 at tcp://127.0.0.1:55562", response1.Message);
            Assert.Equal("TestDealer-002 connected to session TestDealer-002-1970-01-01-0 at tcp://127.0.0.1:55562", response2.Message);
            Assert.Equal(2, server.CountReceived);
            Assert.Equal(2, server.CountSent);

            // Tear Down
            dealer1.Stop().Wait();
            dealer2.Stop().Wait();
            server.Stop().Wait();
            dealer1.Dispose();
            dealer2.Dispose();
            server.Dispose();
        }

        // [Fact]
        // internal void Given1000Messages_StoresAndSendsResponsesToSenderInOrder()
        // {
        //     // Arrange
        //     var testAddress = ZmqNetworkAddress.LocalHost(55564);
        //
        //     var server = new MessageServerFacade(
        //         this.container,
        //         this.compressor,
        //         EncryptionSettings.None(),
        //         testAddress);
        //     server.Start().Wait();
        //
        //     var dealer = new TestDealer(
        //         this.container,
        //         new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
        //         new MsgPackResponseSerializer(),
        //         this.compressor,
        //         EncryptionSettings.None(),
        //         testAddress,
        //         "001");
        //     dealer.Start().Wait();
        //
        //     // Act
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         var request = new DataRequest(
        //             new Dictionary<string, string> { { "Payload", $"TEST-{i}" } },
        //             Guid.NewGuid(),
        //             StubZonedDateTime.UnixEpoch());
        //
        //         var response = dealer.Send(request);
        //     }
        //
        //     // Assert
        //     Assert.Equal(1000, server.ReceivedMessages.Count);
        //     Assert.Equal(1000, server.CountReceived);
        //     Assert.Equal(1000, server.CountSent);
        //     Assert.Equal("TEST-999", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
        //     Assert.Equal("TEST-998", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);
        //
        //     // Tear Down
        //     dealer.Stop().Wait();
        //     server.Stop().Wait();
        //     dealer.Dispose();
        //     server.Dispose();
        // }

        // [Fact]
        // internal void Given1000Messages_FromDifferentSenders_StoresAndSendsResponsesToSendersInOrder()
        // {
        //     // Arrange
        //     var testAddress = ZmqNetworkAddress.LocalHost(55565);
        //
        //     var server = new MessageServerFacade(
        //         this.container,
        //         this.compressor,
        //         EncryptionSettings.None(),
        //         testAddress);
        //     server.Start().Wait();
        //
        //     var requester1 = new RequestSocket(testAddress);
        //     var requester2 = new RequestSocket(testAddress);
        //     requester1.Connect(testAddress);
        //     requester2.Connect(testAddress);
        //
        //     Task.Delay(100).Wait(); // Allow requesters to connect
        //
        //     // Act
        //     for (var i = 0; i < 1000; i++)
        //     {
        //         var message1 = new DataRequest(
        //             new Dictionary<string, string> { { "Payload", $"TEST-{i} from 1" } },
        //             Guid.NewGuid(),
        //             StubZonedDateTime.UnixEpoch());
        //
        //         var message2 = new DataRequest(
        //             new Dictionary<string, string> { { "Payload", $"TEST-{i} from 2" } },
        //             Guid.NewGuid(),
        //             StubZonedDateTime.UnixEpoch());
        //
        //         var serialized1 = this.requestSerializer.Serialize(message1);
        //         requester1.SendMultipartBytes(BitConverter.GetBytes(serialized1.Length), serialized1);
        //         requester1.ReceiveMultipartBytes();
        //
        //         var serialized2 = this.requestSerializer.Serialize(message2);
        //         requester2.SendMultipartBytes(BitConverter.GetBytes(serialized2.Length), serialized2);
        //         requester2.ReceiveMultipartBytes();
        //     }
        //
        //     // Assert
        //     Assert.Equal(2000, server.ReceivedMessages.Count);
        //     Assert.Equal(2000, server.CountReceived);
        //     Assert.Equal(2000, server.CountSent);
        //     Assert.Equal("TEST-999 from 2", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
        //     Assert.Equal("TEST-999 from 1", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);
        //     Assert.Equal("TEST-998 from 2", server.ReceivedMessages[^3].Query.FirstOrDefault().Value);
        //     Assert.Equal("TEST-998 from 1", server.ReceivedMessages[^4].Query.FirstOrDefault().Value);
        //
        //     // Tear Down
        //     try
        //     {
        //         requester1.Disconnect(testAddress);
        //         requester2.Disconnect(testAddress);
        //         requester1.Dispose();
        //         requester2.Dispose();
        //         server.Stop().Wait();
        //         server.Dispose();
        //     }
        //     catch (Exception ex)
        //     {
        //         this.Output.WriteLine(ex.Message);
        //     }
        // }
    }
}
