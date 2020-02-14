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
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;
    using Nautilus.Messaging.Interfaces;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a generic message publisher.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : Component, IDisposable
        where T : Message
    {
        private readonly PublisherSocket socket;
        private readonly ISerializer<T> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublisher{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The message serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        protected MessagePublisher(
            IComponentryContainer container,
            ISerializer<T> serializer,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromSeconds(1),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.serializer = serializer;

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
            this.socket.Dispose();
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

            // this.Dispose();
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), this.serializer.Serialize(message));

            this.CountPublished++;
            this.Log.Verbose($"[{this.CountPublished}]--> Topic={topic}, Message={message}");
        }
    }
}
