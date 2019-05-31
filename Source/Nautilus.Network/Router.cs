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
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging router.
    /// </summary>
    public abstract class Router : Component
    {
        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;
        private readonly byte[] ok = Encoding.UTF8.GetBytes("OK");

        private bool isConsuming;
        private int cycles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Router"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected Router(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();

            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.ServerAddress = new ZmqServerAddress(host, port);
        }

        /// <summary>
        /// Gets the server address for the router.
        /// </summary>
        public ZmqServerAddress ServerAddress { get; }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.Log.Information($"Starting from {message}...");

            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound router socket to {this.ServerAddress}");
            this.Log.Debug("Ready to consume...");

            this.isConsuming = true;
            Task.Run(this.StartConsuming, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Debug($"Stopping from {message}...");
            this.isConsuming = false;
            this.cts.Cancel();
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound router socket from {this.ServerAddress}");

            this.socket.Dispose();
            this.Log.Debug("Stopped.");
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
            this.Log.Verbose($"Acknowledged message[{this.cycles}] receipt on {this.ServerAddress.Value}.");

            this.SendToSelf(data);
        }
    }
}
