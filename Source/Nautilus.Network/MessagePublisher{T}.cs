// -------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a generic message publisher.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : MessageBusConnected
        where T : Message
    {
        private readonly PublisherSocket socket;
        private readonly ISerializer<T> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublisher{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="serializer">The message serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        protected MessagePublisher(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            ISerializer<T> serializer,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container, messageBusAdapter)
        {
            Condition.NotDefault(id, nameof(id));

            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.serializer = serializer;

            this.ServerAddress = new ZmqServerAddress(host, port);
            this.PublishedCount = 0;
        }

        /// <summary>
        /// Gets the server address for the publisher.
        /// </summary>
        public ZmqServerAddress ServerAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int PublishedCount { get; private set; }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound {this.socket.GetType().Name} to {this.ServerAddress}");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound {this.socket.GetType().Name} from {this.ServerAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), this.serializer.Serialize(message));

            this.PublishedCount++;
            this.Log.Debug($"Published[{this.PublishedCount}] Topic={topic}, Message={message}");
        }
    }
}
