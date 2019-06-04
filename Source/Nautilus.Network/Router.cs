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
        private static readonly byte[] Delimiter = { };  // Empty bytes.
        private static readonly byte[] Ok = { 0x4f, 0x4b };  // "OK" UTF-8 encoding.

        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;

        private bool isReceiving;
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
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.ServerAddress.Value);
            this.Log.Debug($"Bound router socket to {this.ServerAddress}");

            this.isReceiving = true;
            Task.Run(this.StartWork, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.isReceiving = false;
            this.cts.Cancel();
            this.socket.Unbind(this.ServerAddress.Value);
            this.Log.Debug($"Unbound router socket from {this.ServerAddress}");

            this.socket.Dispose();
        }

        private Task StartWork()
        {
            while (this.isReceiving)
            {
                this.ReceiveMessage();
            }

            this.Log.Debug("Stopped receiving messages.");
            return Task.CompletedTask;
        }

        private void ReceiveMessage()
        {
            var requestBytes = this.socket.ReceiveMultipartBytes(); // [1] is empty bytes delimiter
            var response = new[] { requestBytes[0], Delimiter, Ok };
            this.socket.SendMultipartBytes(response);

            this.cycles++;
            this.Log.Verbose($"Acknowledged message[{this.cycles}] receipt on {this.ServerAddress.Value}.");

            this.SendToSelf(requestBytes[2]);
        }
    }
}
