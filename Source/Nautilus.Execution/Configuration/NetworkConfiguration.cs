//--------------------------------------------------------------------------------------------------
// <copyright file="NetworkConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
