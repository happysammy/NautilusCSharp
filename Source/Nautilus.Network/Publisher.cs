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
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging consumer.
    /// </summary>
    public class Publisher : ComponentBase
    {
        private readonly ZmqServerAddress serverAddress;
        private readonly PublisherSocket socket;
        private readonly byte[] topic;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="topic">The publishers topic.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        public Publisher(
            IComponentryContainer container,
            string topic,
            NetworkAddress host,
            Port port,
            Guid id)
            : base(NautilusService.Messaging, container)
        {
            Precondition.NotDefault(id, nameof(id));

            this.topic = Encoding.UTF8.GetBytes(topic);
            this.serverAddress = new ZmqServerAddress(host, port);
            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromSeconds(1),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.RegisterHandler<byte[]>(this.Publish);
        }

        /// <summary>
        /// Actions to be performed when starting the <see cref="Router"/>.
        /// </summary>
        public override void Start()
        {
            this.socket.Bind(this.serverAddress.Value);
            this.Log.Debug($"Bound publisher socket to {this.serverAddress}");
            this.Log.Debug("Ready to publish...");
        }

        /// <summary>
        /// Actions to be performed when stopping the <see cref="Router"/>.
        /// </summary>
        public override void Stop()
        {
            this.socket.Unbind(this.serverAddress.Value);
            this.Log.Debug($"Unbound publisher socket from {this.serverAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Sends the given message onto the socket.
        /// </summary>
        /// <param name="message">The message to send.</param>
        protected void Publish(byte[] message)
        {
            this.socket.SendMoreFrame(this.topic).SendFrame(message);

            this.cycles++;
            this.Log.Debug($"Published message[{this.cycles}].");
        }
    }
}
