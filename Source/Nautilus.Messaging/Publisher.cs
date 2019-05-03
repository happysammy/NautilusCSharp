// -------------------------------------------------------------------------------------------------
// <copyright file="Publisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using System.Linq;
    using System.Text;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Network;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// Provides a messaging consumer.
    /// </summary>
    public class Publisher : ComponentBase
    {
        private readonly byte[] delimiter = Encoding.UTF8.GetBytes(" ");
        private readonly byte[] topic;
        private readonly ZmqServerAddress serverAddress;
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
            NetworkAddress host,
            Port port,
            Guid id)
            : base(
                NautilusService.Messaging,
                label,
                container)
        {
            Precondition.NotDefault(id, nameof(id));

            this.topic = Encoding.UTF8.GetBytes(topic);
            this.serverAddress = new ZmqServerAddress(host, port);
            this.socket = new PublisherSocket()
            {
                Options =
                {
                    Linger = TimeSpan.FromMilliseconds(1000),
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };
        }

        /// <summary>
        /// Actions to be performed when starting the <see cref="Consumer"/>.
        /// </summary>
        public override void Start()
        {
            this.Execute(() =>
            {
                this.socket.Bind(this.serverAddress.Value);
                this.Log.Debug($"Bound publisher socket to {this.serverAddress}");

                this.Log.Debug("Ready to publish...");
            });
        }

        /// <summary>
        /// Actions to be performed when stopping the <see cref="Consumer"/>.
        /// </summary>
        public override void Stop()
        {
            this.Execute(() =>
            {
                this.socket.Unbind(this.serverAddress.Value);
                this.Log.Debug($"Unbound publisher socket from {this.serverAddress}");

                this.socket.Dispose();
            });
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            var combined = new byte[arrays.Sum(a => a.Length)];
            var offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, combined, offset, array.Length);
                offset += array.Length;
            }

            return combined;
        }

        private void OnMessage(byte[] message)
        {
            Debug.NotNull(message, nameof(message));

            this.socket.SendFrame(Combine(this.topic, this.delimiter, message));

            this.cycles++;
            this.Log.Debug($"Published message[{this.cycles}].");
        }
    }
}
