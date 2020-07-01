// -------------------------------------------------------------------------------------------------
// <copyright file="ZmqNetworkAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using Nautilus.Core.Annotations;

namespace Nautilus.Network
{
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
        public ZmqNetworkAddress(NetworkAddress host, Port port)
        {
            this.Host = host;
            this.Port = port;
            this.Value = $"tcp://{host}:{port}";
        }

        /// <summary>
        /// Gets the network addresses host.
        /// </summary>
        public NetworkAddress Host { get; }

        /// <summary>
        /// Gets the network addresses port.
        /// </summary>
        public Port Port { get; }

        /// <summary>
        /// Gets the network addresses string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a <see cref="ZmqNetworkAddress"/> for local host at the given port.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <returns>The network address.</returns>
        public static ZmqNetworkAddress LocalHost(int port)
        {
            return new ZmqNetworkAddress(NetworkAddress.LocalHost, new Port(port));
        }

        /// <summary>
        /// Returns a <see cref="ZmqNetworkAddress"/> for local host at the given port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>The network address.</returns>
        public static ZmqNetworkAddress LocalHost(Port port)
        {
            return new ZmqNetworkAddress(NetworkAddress.LocalHost, port);
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ZmqNetworkAddress"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
