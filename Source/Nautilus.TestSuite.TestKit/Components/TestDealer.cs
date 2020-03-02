//--------------------------------------------------------------------------------------------------
// <copyright file="TestDealer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
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

    /// <summary>
    /// Provides a mock requester for testing.
    /// </summary>
    public sealed class TestDealer : MessagingComponent
    {
        private readonly ISerializer<Dictionary<string, string>> headerSerializer;
        private readonly IMessageSerializer<Request> requestSerializer;
        private readonly IMessageSerializer<Response> responseSerializer;
        private readonly ICompressor compressor;
        private readonly DealerSocket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDealer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="headerSerializer">The header serializer.</param>
        /// <param name="requestSerializer">The outbound serializer.</param>
        /// <param name="responseSerializer">The inbound serializer.</param>
        /// <param name="compressor">The compressor.</param>
        /// <param name="encryption">The encryption settings.</param>
        /// <param name="serviceAddress">The service address to connect to.</param>
        /// <param name="subName">The requesters sub-name.</param>
        public TestDealer(
            IComponentryContainer container,
            ISerializer<Dictionary<string, string>> headerSerializer,
            IMessageSerializer<Request> requestSerializer,
            IMessageSerializer<Response> responseSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress serviceAddress,
            string subName = "")
            : base(container, subName)
        {
            this.headerSerializer = headerSerializer;
            this.requestSerializer = requestSerializer;
            this.responseSerializer = responseSerializer;
            this.compressor = compressor;

            this.socket = new DealerSocket()
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes(this.Name.Value),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.ClientId = new ClientId(this.Name.Value);
            this.ServiceAddress = serviceAddress;

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(
                    LogId.Networking,
                    $"{encryption.Algorithm} encryption setup for connections to {this.ServiceAddress}");
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Networking,
                    $"No encryption setup for connections to {this.ServiceAddress}");
            }

            this.ReceivedCount = 0;
            this.SendCount = 0;
        }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        public ClientId ClientId { get; }

        /// <summary>
        /// Gets the network address for the router.
        /// </summary>
        public ZmqNetworkAddress ServiceAddress { get; }

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
                if (!this.socket.IsDisposed)
                {
                    this.socket.Dispose();
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
        /// <returns>The response.</returns>
        public Response SendRaw(byte[][] frames)
        {
            this.socket.SendMultipartBytes(frames);
            this.SendCount++;

            var received = this.socket.ReceiveMultipartBytes();
            this.ReceivedCount++;

            var decompressed = this.compressor.Decompress(received[1]);
            var deserialized = this.responseSerializer.Deserialize(decompressed);

            return deserialized;
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="message">The message string to send.</param>
        /// <returns>The response.</returns>
        public Response SendString(string message)
        {
            var header = new Dictionary<string, string>
            {
                { nameof(MessageType), MessageType.String.ToString() },
                { "TypeName", "utf-8" },
            };

            var frameHeader = this.compressor.Compress(this.headerSerializer.Serialize(header));
            var frameBody = this.compressor.Compress(Encoding.UTF8.GetBytes(message));

            return this.Send(frameHeader, frameBody);
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The response.</returns>
        public Response Send(Request request)
        {
            var header = new Dictionary<string, string>
            {
                { nameof(MessageType), request.MessageType.ToString() },
                { "TypeName", request.Type.Name },
            };

            var frameHeader = this.compressor.Compress(this.headerSerializer.Serialize(header));
            var frameBody = this.compressor.Compress(this.requestSerializer.Serialize(request));

            return this.Send(frameHeader, frameBody);
        }

        /// <inheritdoc/>
        protected override void OnStart(Start start)
        {
            this.socket.Connect(this.ServiceAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Connecting {this.socket.GetType().Name} to {this.ServiceAddress}...");
        }

        /// <inheritdoc/>
        protected override void OnStop(Stop stop)
        {
            this.socket.Disconnect(this.ServiceAddress.Value);
            this.Logger.LogInformation(
                LogId.Networking,
                $"Disconnected {this.socket.GetType().Name} from {this.ServiceAddress}");
        }

        private Response Send(byte[] header, byte[] body)
        {
            this.socket.SendMultipartBytes(header, body);
            this.SendCount++;

            var received = this.socket.ReceiveMultipartBytes();
            this.ReceivedCount++;

            var decompressed = this.compressor.Decompress(received[1]);
            var deserialized = this.responseSerializer.Deserialize(decompressed);

            return deserialized;
        }
    }
}
