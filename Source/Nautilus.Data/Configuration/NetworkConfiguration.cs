//--------------------------------------------------------------------------------------------------
// <copyright file="NetworkConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Configuration
{
    using Nautilus.Network;

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
