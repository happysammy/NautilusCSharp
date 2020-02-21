//--------------------------------------------------------------------------------------------------
// <copyright file="NetworkConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Configuration
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Network;

    /// <summary>
    /// Represents a data service network configuration.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1611", Justification = "TODO")]
    public sealed class NetworkConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkConfiguration"/> class.
        /// </summary>
        public NetworkConfiguration(
            Port commandsPort,
            Port eventsPort,
            int commandsPerSecond,
            int newOrdersPerSecond)
        {
            this.CommandsPort = commandsPort;
            this.EventsPort = eventsPort;
            this.CommandsPerSecond = commandsPerSecond;
            this.NewOrdersPerSecond = newOrdersPerSecond;
        }

        /// <summary>
        /// Gets the configuration commands port.
        /// </summary>
        public Port CommandsPort { get; }

        /// <summary>
        /// Gets the configuration events port.
        /// </summary>
        public Port EventsPort { get; }

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
