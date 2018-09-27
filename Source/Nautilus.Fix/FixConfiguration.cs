//--------------------------------------------------------------------------------------------------
// <copyright file="FixConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents the configuration for a FIX session.
    /// </summary>
    [Immutable]
    public sealed class FixConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixConfiguration"/> class.
        /// </summary>
        /// <param name="broker">The FIX brokerage.</param>
        /// <param name="configPath">The FIX configuration file path.</param>
        /// <param name="credentials">The FIX credentials.</param>
        /// <param name="instrumentDataFileName">The instrument data file name.</param>
        public FixConfiguration(
            Broker broker,
            string configPath,
            FixCredentials credentials,
            string instrumentDataFileName)
        {
            Validate.NotNull(configPath, nameof(configPath));
            Validate.NotNull(credentials, nameof(credentials));
            Validate.NotNull(instrumentDataFileName, nameof(instrumentDataFileName));

            this.Broker = broker;
            this.ConfigPath = configPath;
            this.Credentials = credentials;
            this.InstrumentDataFileName = instrumentDataFileName;
        }

        /// <summary>
        /// Gets the FIX brokerage.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Gets the FIX account username.
        /// </summary>
        public string ConfigPath { get; }

        /// <summary>
        /// Gets the FIX account password.
        /// </summary>
        public FixCredentials Credentials { get; }

        /// <summary>
        /// Gets the FIX instrument data file name.
        /// </summary>
        public string InstrumentDataFileName { get; }
    }
}
