// -------------------------------------------------------------------------------------------------
// <copyright file="Router.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging router.
    /// </summary>
    public abstract class Router : ComponentBase
    {
        private readonly CancellationTokenSource cts;
        private readonly ZmqServerAddress serverAddress;
        private readonly RouterSocket socket;
        private readonly byte[] ok = Encoding.UTF8.GetBytes("OK");

        private bool isConsuming;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected Router(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(NautilusService.Network, container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();
            this.serverAddress = new ZmqServerAddress(host, port);

            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromSeconds(1),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.socket.Bind(this.serverAddress.Value);
            this.Log.Debug($"Bound router socket to {this.serverAddress}");
            this.Log.Debug("Ready to consume...");

            this.isConsuming = true;
            Task.Run(this.StartConsuming, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Debug($"Stopping...");
            this.isConsuming = false;
            this.cts.Cancel();
            this.socket.Unbind(this.serverAddress.Value);
            this.Log.Debug($"Unbound router socket from {this.serverAddress}");

            this.socket.Dispose();
            this.Log.Debug($"Stopped.");
        }

        private Task StartConsuming()
        {
            while (this.isConsuming)
            {
                this.ConsumeMessage();
            }

            this.Log.Debug("Stopped consuming messages.");
            return Task.CompletedTask;
        }

        private void ConsumeMessage()
        {
            var identity = this.socket.ReceiveFrameBytes();
            var delimiter = this.socket.ReceiveFrameBytes();
            var data = this.socket.ReceiveFrameBytes();

            var response = new[] { identity, delimiter, this.ok };
            this.socket.SendMultipartBytes(response);

            this.cycles++;
            this.Log.Debug($"Acknowledged message[{this.cycles}] receipt.");

            this.SendToSelf(data);
        }
    }
}
