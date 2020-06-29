//--------------------------------------------------------------------------------------------------
// <copyright file="NetworkConfiguration.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Configuration
{
    using Nautilus.Network;

    /// <summary>
    /// Represents an execution service network configuration.
    /// </summary>
    public sealed class NetworkConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConfiguration"/> class.
        /// </summary>
        /// <param name="commandReqPort">The command request port.</param>
        /// <param name="commandResPort">The command response port.</param>
        /// <param name="eventPubPort">The event publisher port.</param>
        /// <param name="commandsPerSecond">The throttling level for commands.</param>
        /// <param name="newOrdersPerSecond">The throttling level for new orders.</param>
        public NetworkConfiguration(
            Port commandReqPort,
            Port commandResPort,
            Port eventPubPort,
            int commandsPerSecond,
            int newOrdersPerSecond)
        {
            this.CommandReqPort = commandReqPort;
            this.CommandResPort = commandResPort;
            this.EventPubPort = eventPubPort;
            this.CommandsPerSecond = commandsPerSecond;
            this.NewOrdersPerSecond = newOrdersPerSecond;
        }

        /// <summary>
        /// Gets the configuration command request port.
        /// </summary>
        public Port CommandReqPort { get; }

        /// <summary>
        /// Gets the configuration command response port.
        /// </summary>
        public Port CommandResPort { get; }

        /// <summary>
        /// Gets the configuration event publisher port.
        /// </summary>
        public Port EventPubPort { get; }

        /// <summary>
        /// Gets the configuration maximum commands per second.
        /// </summary>
        public int CommandsPerSecond { get; }

        /// <summary>
        /// Gets the configuration maximum new orders per second.
        /// </summary>
        public int NewOrdersPerSecond { get; }
    }
}
