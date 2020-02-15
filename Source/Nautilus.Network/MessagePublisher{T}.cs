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
    using System.Text;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Encryption;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a generic message publisher.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : Component, IDisposable
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
        /// <param name="id">The publishers identifier.</param>
        protected MessagePublisher(
            IComponentryContainer container,
            ISerializer<T> serializer,
            ICompressor compressor,
            EncryptionConfig encryption,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.serializer = serializer;
            this.compressor = compressor;

            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromSeconds(1),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            if (encryption.UseEncryption)
            {
                EncryptionProvider.SetupSocket(encryption, this.socket);
            }

            this.NetworkAddress = new ZmqNetworkAddress(host, port);
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

            if (!this.socket.IsDisposed)
            {
                this.socket.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.NetworkAddress.Value);
            this.Log.Debug($"Bound {this.socket.GetType().Name} to {this.NetworkAddress}");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.socket.Unbind(this.NetworkAddress.Value);
            this.Log.Debug($"Unbound {this.socket.GetType().Name} from {this.NetworkAddress}");
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            var publishable = this.compressor.Compress(this.serializer.Serialize(message));
            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), publishable);

            this.CountPublished++;
            this.Log.Verbose($"[{this.CountPublished}]--> Topic={topic}, Message={message}");
        }
    }
}
