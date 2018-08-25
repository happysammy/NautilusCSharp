// -------------------------------------------------------------------------------------------------
// <copyright file="Consumer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Akka.Actor;
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
    public class Consumer : ActorComponentBase
    {
        private readonly IEndpoint receiver;
        private readonly string serverAddress;
        private readonly RouterSocket socket;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="receiver">The receiver endpoint.</param>
        /// <param name="label">The consumer label.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        public Consumer(
            IComponentryContainer container,
            IEndpoint receiver,
            Label label,
            string host,
            int port,
            Guid id)
            : base(
                NautilusService.Messaging,
                label,
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(receiver, nameof(receiver));
            Validate.NotNull(label, nameof(label));
            Validate.NotNull(host, nameof(host));
            Validate.NotEqualTo(port, nameof(host), 0);
            Validate.NotDefault(id, nameof(id));

            this.receiver = receiver;
            this.serverAddress = $"tcp://{host}:{port.ToString()}";

            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromMilliseconds(1000),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.socket.ReceiveReady += this.ServerReceiveReady;

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
                this.socket.ReceiveReady += this.ServerReceiveReady;
                this.Log.Debug($"Bound router socket to {this.serverAddress}");

                this.Log.Debug("Ready to consume...");
                this.StartConsuming().PipeTo(this.Self);
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
                this.Log.Debug($"Unbound router socket from {this.serverAddress}");

                this.socket.Dispose();
            });
        }

        private void ServerReceiveReady(object sender, NetMQSocketEventArgs e)
        {
        }

        private void OnMessage(byte[] message)
        {
            Debug.NotNull(message, nameof(message));

            this.Log.Debug("Received byte[], sending to receiver.");
            this.receiver.Send(message);

            this.StartConsuming().PipeTo(this.Self);
        }

        private Task<byte[]> StartConsuming()
        {
            var message = this.socket.ReceiveFrameBytes(out var hasMore);
            while (hasMore)
            {
                message = this.socket.ReceiveFrameBytes(out hasMore);
            }

            this.cycles++;
            this.Log.Debug($"Message[{this.cycles}] received.");

            return Task.FromResult(message);
        }
    }
}
