// -------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
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
    /// Provides a generic message publisher.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : MessagingComponent, IDisposable
    {
        private readonly ISerializer<T> serializer;
        private readonly ICompressor compressor;
        private readonly PublisherSocket socket;

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
                    Identity = Encoding.Unicode.GetBytes($"{nameof(Nautilus)}-{this.Name.Value}"),
                    Linger = TimeSpan.FromSeconds(1),
                },
            };

            this.NetworkAddress = new ZmqNetworkAddress(host, port);

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
                this.Logger.LogInformation(LogId.Networking, $"{encryption.Algorithm} encryption setup for {this.NetworkAddress}");
            }
            else
            {
                this.Logger.LogWarning(LogId.Networking, $"No encryption setup for {this.NetworkAddress}");
            }

            this.CountPublished = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the network address for the publisher.
        /// </summary>
        public ZmqNetworkAddress NetworkAddress { get; }

        /// <summary>
        /// Gets the server published message count.
        /// </summary>
        public int CountPublished { get; private set; }

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
                this.Logger.LogCritical(LogId.Networking, ex.Message, ex);
            }

            this.Logger.LogInformation(LogId.Operation, "Disposed.");
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.NetworkAddress.Value);
            this.Logger.LogInformation(LogId.Networking, $"Bound {this.socket.GetType().Name} to {this.NetworkAddress}");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.socket.Unbind(this.NetworkAddress.Value);
            this.Logger.LogInformation(LogId.Networking, $"Unbound {this.socket.GetType().Name} from {this.NetworkAddress}");
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            var serialized = this.serializer.Serialize(message);
            var length = BitConverter.GetBytes((uint)serialized.Length);
            var payload = this.compressor.Compress(serialized);
            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), length, payload);

            this.CountPublished++;
            this.LogPublished(topic, message, serialized.Length, payload.Length);
        }

        [Conditional("DEBUG")]
        private void LogPublished(
            string topic,
            T message,
            int serializedLength,
            int compressedLength)
        {
            var logMessage = $"[{this.CountPublished}]--> " +
                             $"Topic={topic}, " +
                             $"Serialized={serializedLength:N0} bytes, " +
                             $"Compressed={compressedLength:N0} bytes, " +
                             $"Message={message}";

            this.Logger.LogTrace(LogId.Networking, logMessage);
        }
    }
}
