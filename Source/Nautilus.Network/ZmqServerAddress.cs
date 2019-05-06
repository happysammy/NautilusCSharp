// -------------------------------------------------------------------------------------------------
// <copyright file="ZmqServerAddress.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a valid ZeroMQ server address.
    /// </summary>
    [Immutable]
    public sealed class ZmqServerAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZmqServerAddress"/> class.
        /// </summary>
        /// <param name="host">The host network address.</param>
        /// <param name="port">The port.</param>
        public ZmqServerAddress(NetworkAddress host, Port port)
        {
            this.Host = host;
            this.Port = port;
            this.Value = $"tcp://{host}:{port}";
        }

        /// <summary>
        /// Gets the server addresses network address.
        /// </summary>
        public NetworkAddress Host { get; }

        /// <summary>
        /// Gets the server addresses port.
        /// </summary>
        public Port Port { get; }

        /// <summary>
        /// Gets the server addresses string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="ZmqServerAddress"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
