//--------------------------------------------------------------------------------------------------
// <copyright file="MessageServerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Message;
using Nautilus.Data.Messages.Requests;
using Nautilus.Network;
using Nautilus.Network.Compression;
using Nautilus.Network.Encryption;
using Nautilus.Network.Messages;
using Nautilus.Serialization.MessageSerializers;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Facades;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Mocks;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageServerTests : NetMQTestBase
    {
        private readonly IComponentryContainer container;
        private readonly IMessageBusAdapter messagingAdapter;
        private readonly ISerializer<Dictionary<string, string>> headerSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;

        private readonly ZmqNetworkAddress testRequestAddress;
        private readonly ZmqNetworkAddress testResponseAddress;

        public MessageServerTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.messagingAdapter = new MockMessageBusProvider(this.container).Adapter;
            this.headerSerializer = new MsgPackDictionarySerializer();
            this.requestSerializer = new MsgPackRequestSerializer();
            this.responseSerializer = new MsgPackResponseSerializer();
            this.compressor = new CompressorBypass();

            this.testRequestAddress = ZmqNetworkAddress.LocalHost(new Port(55655));
            this.testResponseAddress = ZmqNetworkAddress.LocalHost(new Port(55656));
        }

        [Fact]
        internal void InstantiatedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);

            // Assert
            Assert.Equal(ComponentState.Initialized, server.ComponentState);
            Assert.Equal(0, server.ReceivedCount);
            Assert.Equal(0, server.SentCount);
        }

        [Fact]
        internal void StartedServer_IsInCorrectState()
        {
            // Arrange
            // Act
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            dealer.Start().Wait();

            // Act
            dealer.SendRaw(new byte[] { });
            var response = dealer.Receive();

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(1, server.SentCount);

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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            dealer.Start().Wait();

            // Act
            dealer.SendString(string.Empty);
            var response = dealer.Receive();

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(1, server.SentCount);

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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            dealer.Start().Wait();

            // Act
            dealer.SendRaw(
                Encoding.UTF8.GetBytes("WOW"),
                BitConverter.GetBytes(3),
                Encoding.UTF8.GetBytes("Payload"));

            var response = dealer.Receive();

            // Assert
            Assert.Equal(typeof(MessageRejected), response.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(1, server.SentCount);

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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "001");
            dealer.Start().Wait();

            var connect = new Connect(
                dealer.ClientId,
                "None",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            dealer.Send(connect);
            var response = (Connected)dealer.Receive();

            // Assert
            Assert.Equal(typeof(Connected), response.Type);
            Assert.Equal(1, server.ReceivedCount);
            Assert.Equal(1, server.SentCount);
            Assert.Equal("TestDealer-001 connected to test-server session None", response.Message);

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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "001");
            dealer.Start().Wait();

            var connect = new Connect(
                dealer.ClientId,
                "None",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            dealer.Send(connect);
            var response1 = (Connected)dealer.Receive();

            var disconnect = new Disconnect(
                dealer.ClientId,
                response1.SessionId,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            dealer.Send(disconnect);
            var response2 = (Disconnected)dealer.Receive();

            // Assert
            Assert.Equal(typeof(Connected), response1.Type);
            Assert.Equal(typeof(Disconnected), response2.Type);
            Assert.Equal(2, server.ReceivedCount);
            Assert.Equal(2, server.SentCount);
            Assert.Equal("TestDealer-001 connected to test-server session None", response1.Message);
            Assert.Equal("TestDealer-001 disconnected from test-server session None", response2.Message);

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
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer1 = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "001");
            dealer1.Start().Wait();

            var dealer2 = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "002");
            dealer2.Start().Wait();

            // Act
            var connect1 = new Connect(
                dealer1.ClientId,
                "None",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var connect2 = new Connect(
                dealer2.ClientId,
                "None",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            dealer1.Send(connect1);
            dealer2.Send(connect2);
            var response1 = (Connected)dealer1.Receive();
            var response2 = (Connected)dealer2.Receive();

            // Assert
            Assert.Equal("TestDealer-001 connected to test-server session None", response1.Message);
            Assert.Equal("TestDealer-002 connected to test-server session None", response2.Message);
            Assert.Equal(2, server.ReceivedCount);
            Assert.Equal(2, server.SentCount);

            // Tear Down
            dealer1.Stop().Wait();
            dealer2.Stop().Wait();
            server.Stop().Wait();
            dealer1.Dispose();
            dealer2.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void Given1000Messages_StoresAndSendsResponsesToSenderInOrder()
        {
            // Arrange
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "001");
            dealer.Start().Wait();

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var request = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i}" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());
                dealer.Send(request);
                dealer.Receive();
            }

            // Assert
            Assert.Equal(1000, server.ReceivedMessages.Count);
            Assert.Equal(1000, server.ReceivedCount);
            Assert.Equal(1000, server.SentCount);
            Assert.Equal("TEST-999", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);

            // Tear Down
            dealer.Stop().Wait();
            server.Stop().Wait();
            dealer.Dispose();
            server.Dispose();
        }

        [Fact]
        internal void Given1000Messages_FromDifferentSenders_StoresAndSendsResponsesToSendersInOrder()
        {
            // Arrange
            var server = new MessageServerFacade(
                this.container,
                this.messagingAdapter,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress);
            server.Start().Wait();

            var dealer1 = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "001");
            dealer1.Start().Wait();

            var dealer2 = new TestDealer(
                this.container,
                this.headerSerializer,
                this.requestSerializer,
                this.responseSerializer,
                this.compressor,
                EncryptionSettings.None(),
                this.testRequestAddress,
                this.testResponseAddress,
                "002");
            dealer2.Start().Wait();

            // Act
            for (var i = 0; i < 1000; i++)
            {
                var request1 = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i} from 1" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                var request2 = new DataRequest(
                    new Dictionary<string, string> { { "Payload", $"TEST-{i} from 2" } },
                    Guid.NewGuid(),
                    StubZonedDateTime.UnixEpoch());

                dealer1.Send(request1);
                dealer1.Receive();

                dealer2.Send(request2);
                dealer2.Receive();
            }

            // Assert
            Assert.Equal(2000, server.ReceivedMessages.Count);
            Assert.Equal(2000, server.ReceivedCount);
            Assert.Equal(2000, server.SentCount);
            Assert.Equal("TEST-999 from 2", server.ReceivedMessages[^1].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-999 from 1", server.ReceivedMessages[^2].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998 from 2", server.ReceivedMessages[^3].Query.FirstOrDefault().Value);
            Assert.Equal("TEST-998 from 1", server.ReceivedMessages[^4].Query.FirstOrDefault().Value);

            // Tear Down
            dealer1.Stop().Wait();
            dealer2.Stop().Wait();
            server.Stop().Wait();
            dealer1.Dispose();
            dealer2.Dispose();
            server.Dispose();
        }
    }
}
