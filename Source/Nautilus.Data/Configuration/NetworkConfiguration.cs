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

using Nautilus.Network;

namespace Nautilus.Data.Configuration
{
    /// <summary>
    /// Represents a data service network configuration.
    /// </summary>
    public sealed class NetworkConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConfiguration"/> class.
        /// </summary>
        /// <param name="dataReqPort">The data request port.</param>
        /// <param name="dataResPort">The data response port.</param>
        /// <param name="dataPubPort">The data publisher port.</param>
        /// <param name="tickPubPort">The tick publisher port.</param>
        public NetworkConfiguration(
            Port dataReqPort,
            Port dataResPort,
            Port dataPubPort,
            Port tickPubPort)
        {
            this.DataReqPort = dataReqPort;
            this.DataResPort = dataResPort;
            this.DataPubPort = dataPubPort;
            this.TickPubPort = tickPubPort;
        }

        /// <summary>
        /// Gets the network configuration data request port.
        /// </summary>
        public Port DataReqPort { get; }

        /// <summary>
        /// Gets the network configuration data response port.
        /// </summary>
        public Port DataResPort { get; }

        /// <summary>
        /// Gets the network configuration data publisher port.
        /// </summary>
        public Port DataPubPort { get; }

        /// <summary>
        /// Gets the network configuration tick publisher port.
        /// </summary>
        public Port TickPubPort { get; }
    }
}
