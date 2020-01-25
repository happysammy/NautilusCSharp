// -------------------------------------------------------------------------------------------------
// <copyright file="ZmqNetworkAddress.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a valid ZeroMQ network address.
    /// </summary>
    [Immutable]
    public sealed class ZmqNetworkAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZmqNetworkAddress"/> class.
        /// </summary>
        /// <param name="host">The host network address.</param>
        /// <param name="port">The port.</param>
        public ZmqNetworkAddress(NetworkHost host, NetworkPort port)
        {
            this.Host = host;
            this.Port = port;
            this.Value = $"tcp://{host}:{port}";
        }

        /// <summary>
        /// Gets the network addresses host address.
        /// </summary>
        public NetworkHost Host { get; }

        /// <summary>
        /// Gets the network addresses port.
        /// </summary>
        public NetworkPort Port { get; }

        /// <summary>
        /// Gets the network addresses string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="ZmqNetworkAddress"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
