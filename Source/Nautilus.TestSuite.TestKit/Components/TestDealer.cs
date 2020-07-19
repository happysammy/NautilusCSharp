//--------------------------------------------------------------------------------------------------
// <copyright file="TestDealer.cs" company="Nautech Systems Pty Ltd">
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
using System.Text;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Enums;
using Nautilus.Core.Message;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Identifiers;
using NetMQ;
using NetMQ.Sockets;

namespace Nautilus.TestSuite.TestKit.Components
{
    /// <summary>
    /// Provides a mock requester for testing.
    /// </summary>
    public sealed class TestDealer : MessagingComponent
    {
        private readonly ISerializer<Dictionary<string, string>> headerSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;
        private readonly DealerSocket socketOutbound;
        private readonly DealerSocket socketInbound;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDealer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The outbound serializer.</param>
        /// <param name="responseSerializer">The inbound serializer.</param>
        /// <param name="compressor">The compressor.</param>
        /// <param name="encryption">The encryption settings.</param>
        /// <param name="serverReqAddress">The server request address to connect to.</param>
        /// <param name="serverResAddress">The server response address to connect to.</param>
        /// <param name="subName">The requesters sub-name.</param>
        public TestDealer(
            IComponentryContainer container,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress serverReqAddress,
            ZmqNetworkAddress serverResAddress,
            string subName = "")
            : base(container, subName)
        {
            this.headerSerializer = headerSerializer;
            this.requestSerializer = requestSerializer;
            this.responseSerializer = responseSerializer;
            this.compressor = compressor;

            this.socketOutbound = new DealerSocket
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes(this.Name.Value),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.socketInbound = new DealerSocket
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes(this.Name.Value),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.ClientId = new ClientId(this.Name.Value);
            this.ServerReqAddress = serverReqAddress;
            this.ServerResAddress = serverResAddress;

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socketOutbound);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for connections to {this.ServerReqAddress}");

                EncryptionProvider.SetupSocket(encryption, this.socketInbound);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for connections to {this.ServerResAddress}");
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for connections to {this.ServerReqAddress}");

                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for connections to {this.ServerResAddress}");
            }

            this.ReceivedCount = 0;
            this.SendCount = 0;
        }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        public ClientId ClientId { get; }

        /// <summary>
        /// Gets the network address for server requests.
        /// </summary>
        public ZmqNetworkAddress ServerReqAddress { get; }

        /// <summary>
        /// Gets the network address for server responses.
        /// </summary>
        public ZmqNetworkAddress ServerResAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int ReceivedCount { get; private set; }

        /// <summary>
        /// Gets the server sent message count.
        /// </summary>
        public int SendCount { get; private set; }

        /// <summary>
        /// Dispose of the socket.
        /// </summary>
        public void Dispose()
        {
            if (this.ComponentState == ComponentState.Running)
            {
                throw new InvalidOperationException("Cannot dispose a running component.");
            }

            try
            {
                if (!this.socketOutbound.IsDisposed)
                {
                    this.socketOutbound.Dispose();
                }

                if (!this.socketInbound.IsDisposed)
                {
                    this.socketInbound.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex.Message);
            }

            this.Logger.LogInformation("Disposed.");
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="frames">The raw frames to send.</param>
        public void SendRaw(params byte[][] frames)
        {
            this.Send(frames);
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="message">The message string to send.</param>
        public void SendString(string message)
        {
            var header = new Dictionary<string, string>
            {
                { nameof(MessageType), MessageType.String.ToString() },
                { nameof(Type), nameof(Encoding.UTF8) },
            };

            var frameHeader = this.compressor.Compress(this.headerSerializer.Serialize(header));
            var frameBody = this.compressor.Compress(Encoding.UTF8.GetBytes(message));

            this.Send(frameHeader, frameBody);
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="request">The request to send.</param>
        public void Send(Request request)
        {
            var header = new Dictionary<string, string>
            {
                { nameof(MessageType), request.MessageType.ToString() },
                { nameof(Type), request.Type.Name },
            };

            var frameHeader = this.compressor.Compress(this.headerSerializer.Serialize(header));
            var frameBody = this.compressor.Compress(this.requestSerializer.Serialize(request));

            this.Send(frameHeader, frameBody);
        }

        /// <summary>
        /// Receive the next response.
        /// </summary>
        /// <returns>The response.</returns>
        public Response Receive()
        {
            var received = this.socketInbound.ReceiveMultipartBytes();
            this.ReceivedCount++;

            var decompressed = this.compressor.Decompress(received[1]);
            var deserialized = this.responseSerializer.Deserialize(decompressed);

            return deserialized;
        }

        /// <inheritdoc/>
        protected override void OnStart(Start start)
        {
            this.socketOutbound.Connect(this.ServerReqAddress.Value);
            this.socketInbound.Connect(this.ServerResAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Connecting {this.socketInbound.GetType().Name} to {this.ServerReqAddress}...");
            this.Logger.LogInformation(
                LogId.Networking,
                $"Connecting {this.socketInbound.GetType().Name} to {this.ServerResAddress}...");
        }

        /// <inheritdoc/>
        protected override void OnStop(Stop stop)
        {
            this.socketOutbound.Disconnect(this.ServerReqAddress.Value);
            this.socketInbound.Disconnect(this.ServerResAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Disconnected {this.socketInbound.GetType().Name} from {this.ServerReqAddress}");
            this.Logger.LogInformation(
                LogId.Networking,
                $"Disconnected {this.socketInbound.GetType().Name} from {this.ServerResAddress}");
        }

        private void Send(params byte[][] frames)
        {
            this.socketOutbound.SendMultipartBytes(frames);
            this.SendCount++;
        }
    }
}
