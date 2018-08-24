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
    using System.Threading;
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
        private readonly string serverAddress;
        private readonly RouterSocket socket;
        private readonly NetMQPoller poller;
        private readonly IEndpoint receiver;
        private readonly ThreadLocal<DealerSocket> clients;

        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="consumerLabel">The consumer label.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="serverAddress">The consumer server address.</param>
        /// <param name="id">The consumer identifier.</param>
        /// <param name="receiver">The message receiver.</param>
        public Consumer(
            Label consumerLabel,
            IComponentryContainer container,
            string serverAddress,
            Guid id,
            IEndpoint receiver)
            : base(
                NautilusService.Messaging,
                consumerLabel,
                container)
        {
            Validate.NotNull(consumerLabel, nameof(consumerLabel));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(serverAddress, nameof(serverAddress));
            Validate.NotDefault(id, nameof(id));
            Validate.NotNull(receiver, nameof(receiver));

            this.serverAddress = serverAddress;
            this.poller = new NetMQPoller();
            this.socket = new RouterSocket
            {
                Options =
                {
                    Linger = TimeSpan.FromMilliseconds(1000),
                    Identity = Encoding.Unicode.GetBytes(id.ToString())
                }
            };

            socket.ReceiveReady += ServerReceiveReady;

            this.receiver = receiver;
            this.clients = new ThreadLocal<DealerSocket>();
        }

        /// <summary>
        /// Actions to be performed when starting the <see cref="Consumer"/>.
        /// </summary>
        protected override void PreStart()
        {
            this.Execute(() =>
            {
                base.PreStart();
                this.socket.Connect(this.serverAddress);
                this.socket.Bind(this.serverAddress);
                this.socket.ReceiveReady += this.ServerReceiveReady;
                this.poller.Add(this.socket);
                this.Log.Debug($"Connected socket to {this.serverAddress}");

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
                this.poller.Stop();
                this.poller.Dispose();
                this.socket.Disconnect(this.serverAddress);
                this.socket.Close();
                this.socket.Dispose();
            });
        }

        private void ServerReceiveReady(object sender, NetMQSocketEventArgs e)
        {
        }

        private Task StartConsuming()
        {
            this.Log.Debug("Started consuming...");

            while (true)
            {
                var message = this.socket.ReceiveFrameBytes(out var hasMore);

                if (hasMore)
                {
                    // There are more byte frames to the message so continue the loop.
                    continue;
                }

                this.cycles++;
                this.Log.Debug($"Received message {Encoding.UTF8.GetString(message)} {this.cycles}");

                this.receiver.Send(message);
            }
        }
    }
}
