// -------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Nodes
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Encryption;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all message publishers.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : MessagingComponent, IDisposable
    {
        private readonly ISerializer<T> serializer;
        private readonly ICompressor compressor;
        private readonly PublisherSocket socket;
        private readonly ZmqNetworkAddress networkAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublisher{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The message serializer.</param>
        /// <param name="compressor">The message compressor.</param>
        /// <param name="encryption">The encryption configuration.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        protected MessagePublisher(
            IComponentryContainer container,
            ISerializer<T> serializer,
            ICompressor compressor,
            EncryptionSettings encryption,
            NetworkAddress host,
            Port port)
            : base(container)
        {
            this.serializer = serializer;
            this.compressor = compressor;

            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Identity = Encoding.UTF8.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.networkAddress = new ZmqNetworkAddress(host, port);

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(LogId.Networking, $"{encryption.Algorithm} encryption setup for {this.networkAddress}");
            }
            else
            {
                this.Logger.LogWarning(LogId.Networking, $"No encryption setup for {this.networkAddress}");
            }

            this.SentCount = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the server published message count.
        /// </summary>
        public int SentCount { get; private set; }

        /// <inheritdoc />
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
                this.Logger.LogCritical(LogId.Networking, ex.Message, ex);
            }

            this.Logger.LogInformation(LogId.Component, "Disposed.");
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            try
            {
                this.socket.Bind(this.networkAddress.Value);
                this.Logger.LogInformation(LogId.Networking, $"Bound {this.socket.GetType().Name} to {this.networkAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            try
            {
                this.socket.Unbind(this.networkAddress.Value);
                this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socket.GetType().Name} from {this.networkAddress}");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            var body = this.compressor.Compress(this.serializer.Serialize(message));

            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), body);

            this.SentCount++;
            this.LogPublished(topic, body.Length);
        }

        [Conditional("DEBUG")]
        private void LogPublished(string topic, int compressedLength)
        {
            var logMessage = $"[{this.SentCount}]--> Topic={topic}, Body={compressedLength} bytes";

            this.Logger.LogTrace(LogId.Networking, logMessage);
        }
    }
}
