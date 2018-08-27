// -------------------------------------------------------------------------------------------------
// <copyright file="Publisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using System.Text;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging consumer.
    /// </summary>
    public class Publisher : ActorComponentBase
    {
        private readonly string topic;
        private readonly string serverAddress;
        private readonly PublisherSocket socket;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Publisher"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="label">The publisher label.</param>
        /// <param name="topic">The publishers topic.</param>
        /// <param name="host">The publishers host address.</param>
        /// <param name="port">The publishers port.</param>
        /// <param name="id">The publishers identifier.</param>
        public Publisher(
            IComponentryContainer container,
            Label label,
            string topic,
            string host,
            int port,
            Guid id)
            : base(
                NautilusService.Messaging,
                label,
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(label, nameof(label));
            Validate.NotNull(host, nameof(host));
            Validate.NotEqualTo(port, nameof(host), 0);
            Validate.NotDefault(id, nameof(id));

            this.topic = topic;
            this.serverAddress = $"tcp://{host}:{port}";
            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromMilliseconds(1000),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            // Setup message handling.
            this.Receive<byte[]>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Actions to be performed when starting the <see cref="Consumer"/>.
        /// </summary>
        protected override void PreStart()
        {
            this.Execute(() =>
            {
                base.PreStart();
                this.socket.Bind(this.serverAddress);
                this.Log.Debug($"Bound publisher socket to {this.serverAddress}");

                this.Log.Debug("Ready to publish...");
            });
        }

        /// <summary>
        /// Actions to be performed when stopping the <see cref="Consumer"/>.
        /// </summary>
        protected override void PostStop()
        {
            this.Execute(() =>
            {
                this.socket.Unbind(this.serverAddress);
                this.Log.Debug($"Unbound publisher socket from {this.serverAddress}");

                this.socket.Dispose();
            });
        }

        private void OnMessage(byte[] message)
        {
            Debug.NotNull(message, nameof(message));

            this.socket.SendMoreFrame(this.topic).SendFrame(message);

            this.cycles++;
            this.Log.Debug($"Published message[{this.cycles}].");
        }
    }
}
