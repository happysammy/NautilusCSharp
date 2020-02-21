//--------------------------------------------------------------------------------------------------
// <copyright file="NetworkConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Configuration
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
            Port tickRouterPort,
            Port tickPublisherPort,
            Port barRouterPort,
            Port barPublisherPort,
            Port instrumentRouterPort,
            Port instrumentPublisherPort)
        {
            this.TickRouterPort = tickRouterPort;
            this.TickPublisherPort = tickPublisherPort;
            this.BarRouterPort = barRouterPort;
            this.BarPublisherPort = barPublisherPort;
            this.InstrumentRouterPort = instrumentRouterPort;
            this.InstrumentPublisherPort = instrumentPublisherPort;
        }

        /// <summary>
        /// Gets the network configuration tick request port.
        /// </summary>
        public Port TickRouterPort { get; }

        /// <summary>
        /// Gets the network configuration tick subscribe port.
        /// </summary>
        public Port TickPublisherPort { get; }

        /// <summary>
        /// Gets the network configuration bar request port.
        /// </summary>
        public Port BarRouterPort { get; }

        /// <summary>
        /// Gets the network configuration bar subscribe port.
        /// </summary>
        public Port BarPublisherPort { get; }

        /// <summary>
        /// Gets the network configuration instrument request port.
        /// </summary>
        public Port InstrumentRouterPort { get; }

        /// <summary>
        /// Gets the network configuration instrument subscribe port.
        /// </summary>
        public Port InstrumentPublisherPort { get; }
    }
}
