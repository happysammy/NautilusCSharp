// -------------------------------------------------------------------------------------------------
// <copyright file="Publisher.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging consumer.
    /// </summary>
    public abstract class Publisher : ComponentBase
    {
        private readonly PublisherSocket socket;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publisher"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        protected Publisher(
            IComponentryContainer container,
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

            this.ServerAddress = new ZmqServerAddress(host, port);
        }

        /// <summary>
        /// Gets the server address for the publisher.
        /// </summary>
        public ZmqServerAddress ServerAddress { get; }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound publisher socket to {this.ServerAddress}");
            this.Log.Debug("Ready to publish...");
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound publisher socket from {this.ServerAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="topic">The messages topic.</param>
        /// <param name="message">The message to publish.</param>
        protected void Publish(byte[] topic, byte[] message)
        {
            this.socket
                .SendMoreFrame(topic)
                .SendFrame(message);

            this.cycles++;
            this.Log.Verbose($"Published message[{this.cycles}].");
        }
    }
}
