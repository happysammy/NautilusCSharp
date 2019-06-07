// -------------------------------------------------------------------------------------------------
// <copyright file="MessagePublisher{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging consumer.
    /// </summary>
    /// <typeparam name="T">The publishing message type.</typeparam>
    public abstract class MessagePublisher<T> : Component
    {
        private readonly ISerializer<T> serializer;
        private readonly PublisherSocket socket;

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

            this.serializer = serializer;
            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.ServerAddress = new ZmqServerAddress(host, port);
            this.PublishedCount = 0;

            this.RegisterHandler<Envelope<Command>>(this.Open);
            this.RegisterHandler<Envelope<Event>>(this.Open);
            this.RegisterHandler<Envelope<Document>>(this.Open);
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
            this.Log.Debug($"Published message[{this.PublishedCount}] Topic={topic}, Message={message}");
        }

        private void Open(Envelope<Command> envelope)
        {
            this.SendToSelf(envelope.Message);
        }

        private void Open(Envelope<Event> envelope)
        {
            this.SendToSelf(envelope.Message);
        }

        private void Open(Envelope<Document> envelope)
        {
            this.SendToSelf(envelope.Message);
        }
    }
}
