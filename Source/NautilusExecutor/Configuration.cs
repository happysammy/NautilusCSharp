//--------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nautilus.Common.Configuration;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serilog.Events;

    /// <summary>
    /// Represents a data system configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="configJson">The parsed configuration JSON.</param>
        /// <param name="symbolIndex">The parsed symbols index string.</param>
        /// <param name="isDevelopment">The flag indicating whether the hosting environment is development.</param>
        public Configuration(
            JObject configJson,
            string symbolIndex,
            bool isDevelopment)
        {
            // Log Settings
            this.LogLevel = isDevelopment
                ? LogEventLevel.Debug
                : ((string)configJson[ConfigSection.Logging]["logLevel"]).ToEnum<LogEventLevel>();

            // Network Settings
            this.ServerAddress = isDevelopment
                ? NetworkAddress.LocalHost()
                : new NetworkAddress((string)configJson[ConfigSection.Network]["serverAddress"]);
            this.CommandsPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["commandsPort"]);
            this.EventsPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["eventsPort"]);
            this.CommandsPerSecond = (int)configJson[ConfigSection.Network]["commandsPerSecond"];
            this.NewOrdersPerSecond = (int)configJson[ConfigSection.Network]["newOrdersPerSecond"];

            // FIX Settings
            var fixConfigFile = (string)configJson[ConfigSection.Fix44]["configFile"];
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
            var configPath = Path.GetFullPath(Path.Combine(assemblyDirectory, fixConfigFile));

            var fixSettings = ConfigReader.LoadConfig(configPath);
            var broker = fixSettings["Brokerage"].ToEnum<Brokerage>();
            var credentials = new FixCredentials(
                fixSettings["Account"],
                fixSettings["Username"],
                fixSettings["Password"]);

            this.FixConfiguration = new FixConfiguration(
                broker,
                configPath,
                credentials,
                Convert.ToBoolean(fixSettings["SendAccountTag"]));

            // Data Settings
            this.SymbolIndex = JsonConvert.DeserializeObject<Dictionary<string, string>>(symbolIndex);
        }

        /// <summary>
        /// Gets the configuration log level.
        /// </summary>
        public LogEventLevel LogLevel { get; }

        /// <summary>
        /// Gets the configuration server address.
        /// </summary>
        public NetworkAddress ServerAddress { get; }

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
        public IReadOnlyDictionary<string, string> SymbolIndex { get; }
    }
}
