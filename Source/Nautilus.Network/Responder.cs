// -------------------------------------------------------------------------------------------------
// <copyright file="Responder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Collections.Generic;
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
    public abstract class Responder : Component
    {
        private static readonly byte[] Delimiter = { };

        private readonly CancellationTokenSource cts;
        private readonly ResponseSocket socket;

        private bool isResponding;
        private int cycles;

        private byte[] requesterIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Responder"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected Responder(
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();
            this.socket = new ResponseSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.ServerAddress = new ZmqServerAddress(host, port);
            this.requesterIdentity = new byte[] { };
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

            this.isResponding = true;
            Task.Run(this.StartResponding, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.isResponding = false;
            this.cts.Cancel();
            this.socket.Unbind(this.ServerAddress.Value);
            this.socket.Close();
            this.Log.Debug($"Unbound router socket from {this.ServerAddress}");

            this.socket.Dispose();
        }

        /// <summary>
        /// Respond to the last request with the given response.
        /// </summary>
        /// <param name="response">The response bytes.</param>
        protected void SendResponse(byte[] response)
        {
            this.socket.SendMultipartBytes(this.requesterIdentity, Delimiter, response);

            this.cycles++;
            this.Log.Verbose($"Responded to message[{this.cycles}] on {this.ServerAddress.Value}.");
        }

        /// <summary>
        /// Respond to the last request with the given response.
        /// </summary>
        /// <param name="response">The response byte arrays.</param>
        protected void SendResponse(List<byte[]> response)
        {
            response.Insert(0, Delimiter);
            response.Insert(0, this.requesterIdentity);

            this.socket.SendMultipartBytes(response);

            this.cycles++;
            this.Log.Verbose($"Responded to message[{this.cycles}] on {this.ServerAddress.Value}.");
        }

        private Task StartResponding()
        {
            while (this.isResponding)
            {
                this.ListenForRequest();
            }

            this.Log.Debug("Stopped responding to messages.");
            return Task.CompletedTask;
        }

        private void ListenForRequest()
        {
            this.requesterIdentity = this.socket.ReceiveFrameBytes();
            this.socket.ReceiveFrameBytes(); // Delimiter
            var request = this.socket.ReceiveFrameBytes();

            this.SendToSelf(request);
        }
    }
}
