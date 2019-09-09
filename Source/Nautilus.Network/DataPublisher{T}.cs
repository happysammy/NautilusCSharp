// -------------------------------------------------------------------------------------------------
// <copyright file="DataPublisher{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Text;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a generic data publisher.
    /// </summary>
    /// <typeparam name="T">The publishing data type.</typeparam>
    public abstract class DataPublisher<T> : DataBusConnected, IDisposable
    {
        private readonly PublisherSocket socket;
        private readonly IDataSerializer<T> dataSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPublisher{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="dataSerializer">The data serializer.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        protected DataPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<T> dataSerializer,
            NetworkHost host,
            NetworkPort port,
            Guid id)
            : base(container, dataBusAdapter)
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

            this.dataSerializer = dataSerializer;

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

        /// <summary>
        /// Dispose of the socket.
        /// </summary>
        public void Dispose()
        {
            this.socket?.Dispose();
        }

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
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(string topic, T message)
        {
            this.socket.SendMultipartBytes(Encoding.UTF8.GetBytes(topic), this.dataSerializer.Serialize(message));

            this.PublishedCount++;
            this.Log.Verbose($"[{this.PublishedCount}]--> Topic={topic}, Message={message}");
        }
    }
}
