//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Immutable;
    using Microsoft.Extensions.Logging;
    using Nautilus.Data.Configuration;
    using Nautilus.Fix;
    using Nautilus.Network.Configuration;

    /// <summary>
    /// Represents a <see cref="DataService"/> configuration.
    /// </summary>
    public sealed class ServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfiguration"/> class.
        /// </summary>
        /// <param name="loggerFactory">The service logging adapter.</param>
        /// <param name="fixConfig">The FIX configuration.</param>
        /// <param name="wireConfig">The wire configuration.</param>
        /// <param name="networkConfig">The network configuration.</param>
        /// <param name="dataConfig">The data configuration.</param>
        /// <param name="symbolMap">The symbol map.</param>
        public ServiceConfiguration(
            ILoggerFactory loggerFactory,
            FixConfiguration fixConfig,
            WireConfiguration wireConfig,
            NetworkConfiguration networkConfig,
            DataConfiguration dataConfig,
            ImmutableDictionary<string, string> symbolMap)
        {
            this.LoggerFactory = loggerFactory;
            this.FixConfig = fixConfig;
            this.WireConfig = wireConfig;
            this.NetworkConfig = networkConfig;
            this.DataConfig = dataConfig;
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
        /// Gets the data configuration.
        /// </summary>
        public DataConfiguration DataConfig { get; }

        /// <summary>
        /// Gets the symbol map.
        /// </summary>
        public ImmutableDictionary<string, string> SymbolMap { get; }
    }
}
