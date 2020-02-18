//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Immutable;
    using Microsoft.Extensions.Configuration;
    using Nautilus.Common.Interfaces;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Nautilus.Network.Configuration;

    /// <summary>
    /// Represents an <see cref="ExecutionService"/> configuration.
    /// </summary>
    public sealed class ServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfiguration"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The service logging adapter.</param>
        /// <param name="networkSection">The network configuration section.</param>
        /// <param name="messagingConfig">The messaging configuration section.</param>
        /// <param name="fixConfig">The FIX configuration section.</param>
        /// <param name="symbolMap">The service symbol map.</param>
        public ServiceConfiguration(
            ILoggingAdapter loggingAdapter,
            IConfigurationSection networkSection,
            MessagingConfiguration messagingConfig,
            FixConfiguration fixConfig,
            ImmutableDictionary<string, string> symbolMap)
        {
            this.LoggingAdapter = loggingAdapter;
            this.MessagingConfiguration = messagingConfig;
            this.FixConfiguration = fixConfig;

            // Network Configuration
            this.CommandsPort = new NetworkPort(int.Parse(networkSection["CommandsPort"]));
            this.EventsPort = new NetworkPort(int.Parse(networkSection["EventsPort"]));
            this.CommandsPerSecond = int.Parse(networkSection["CommandsPerSecond"]);
            this.NewOrdersPerSecond = int.Parse(networkSection["NewOrdersPerSecond"]);

            this.SymbolMap = symbolMap;
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggingAdapter LoggingAdapter { get; }

        /// <summary>
        /// Gets the messaging configuration.
        /// </summary>
        public MessagingConfiguration MessagingConfiguration { get; }

        /// <summary>
        /// Gets the configuration commands port.
        /// </summary>
        public NetworkPort CommandsPort { get; }

        /// <summary>
        /// Gets the configuration events port.
        /// </summary>
        public NetworkPort EventsPort { get; }

        /// <summary>
        /// Gets the configuration maximum commands per second.
        /// </summary>
        public int CommandsPerSecond { get; }

        /// <summary>
        /// Gets the configuration maximum new orders per second.
        /// </summary>
        public int NewOrdersPerSecond { get; }

        /// <summary>
        /// Gets the FIX configuration.
        /// </summary>
        public FixConfiguration FixConfiguration { get; }

        /// <summary>
        /// Gets the symbol conversion index.
        /// </summary>
        public ImmutableDictionary<string, string> SymbolMap { get; }
    }
}
