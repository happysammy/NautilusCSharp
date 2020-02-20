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
    using Microsoft.Extensions.Logging;
    using Nautilus.Execution.Configuration;
    using Nautilus.Fix;
    using Nautilus.Network.Configuration;

    /// <summary>
    /// Represents an <see cref="ExecutionService"/> configuration.
    /// </summary>
    public sealed class ServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfiguration"/> class.
        /// </summary>
        /// <param name="loggerFactory">The service logging adapter.</param>
        /// <param name="fixConfig">The FIX configuration.</param>
        /// <param name="wireConfig">The messaging configuration.</param>
        /// <param name="networkConfig">The network configuration.</param>
        /// <param name="symbolMap">The service symbol map.</param>
        public ServiceConfiguration(
            ILoggerFactory loggerFactory,
            FixConfiguration fixConfig,
            WireConfiguration wireConfig,
            NetworkConfiguration networkConfig,
            ImmutableDictionary<string, string> symbolMap)
        {
            this.LoggerFactory = loggerFactory;
            this.FixConfig = fixConfig;
            this.WireConfig = wireConfig;
            this.NetworkConfig = networkConfig;
            this.SymbolMap = symbolMap;
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the FIX configuration.
        /// </summary>
        public FixConfiguration FixConfig { get; }

        /// <summary>
        /// Gets the messaging configuration.
        /// </summary>
        public WireConfiguration WireConfig { get; }

        /// <summary>
        /// Gets the network configuration.
        /// </summary>
        public NetworkConfiguration NetworkConfig { get; }

        /// <summary>
        /// Gets the symbol conversion index.
        /// </summary>
        public ImmutableDictionary<string, string> SymbolMap { get; }
    }
}
