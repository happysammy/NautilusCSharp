//--------------------------------------------------------------------------------------------------
// <copyright file="MockRequester.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Message;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a mock requester for testing.
    /// </summary>
    public sealed class MockRequester : MessagingComponent
    {
        private readonly IMessageSerializer<Request> outboundSerializer;
        private readonly IMessageSerializer<Response> inboundSerializer;
        private readonly ICompressor compressor;
        private readonly RequestSocket socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockRequester"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="outboundSerializer">The outbound serializer.</param>
        /// <param name="inboundSerializer">The inbound serializer.</param>
        /// <param name="compressor">The compressor.</param>
        /// <param name="encryption">The encryption settings.</param>
        /// <param name="serviceAddress">The service address to connect to.</param>
        /// <param name="subName">The requesters sub-name.</param>
        public MockRequester(
            IComponentryContainer container,
            IMessageSerializer<Request> outboundSerializer,
            IMessageSerializer<Response> inboundSerializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress serviceAddress,
            string subName = "")
            : base(container, subName)
        {
            this.outboundSerializer = outboundSerializer;
            this.inboundSerializer = inboundSerializer;
            this.compressor = compressor;

            this.socket = new RequestSocket()
            {
                Options =
                {
                    Identity = Encoding.Unicode.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

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

            this.CountReceived = 0;
            this.CountSent = 0;
        }

        /// <summary>
        /// Gets the network address for the router.
        /// </summary>
        public ZmqNetworkAddress ServiceAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int CountReceived { get; private set; }

        /// <summary>
        /// Gets the server processed message count.
        /// </summary>
        public int CountSent { get; private set; }

        /// <summary>
        /// Dispose of the socket.
        /// </summary>
        public void Dispose()
        {
            if (this.ComponentState == ComponentState.Running)
            {
                throw new InvalidOperationException("Cannot dispose a running component.");
            }

            if (!this.socket.IsDisposed)
            {
                this.socket.Dispose();
            }

            this.Logger.LogDebug("Disposed.");
        }

        /// <summary>
        /// Send the request to the service address.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The response.</returns>
        public Response Send(Request request)
        {
            var serialized = this.outboundSerializer.Serialize(request);
            var payloadLength = BitConverter.GetBytes((uint)serialized.Length);
            var compressed = this.compressor.Compress(serialized);

            this.socket.SendMultipartBytes(payloadLength, compressed);
            this.CountSent++;

            var received = this.socket.ReceiveMultipartBytes();
            this.CountReceived++;

            var receivedLength = BitConverter.ToUInt32(received[0]);
            var decompressed = this.compressor.Decompress(received[1]);
            var deserialized = this.inboundSerializer.Deserialize(decompressed);

            return deserialized;
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
    }
}
