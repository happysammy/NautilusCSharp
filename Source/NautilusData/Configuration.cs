//--------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nautilus.Common.Configuration;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
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
        /// <param name="symbolsIndex">The parsed symbols index string.</param>
        /// <param name="isDevelopment">The flag indicating whether the hosting environment is development.</param>
        public Configuration(
            JObject configJson,
            string symbolsIndex,
            bool isDevelopment)
        {
            // Log Settings
            this.LogLevel = ((string)configJson[ConfigSection.Logging]["logLevel"]).ToEnum<LogEventLevel>();

            // Network Settings
            this.ServerAddress = isDevelopment
                ? NetworkAddress.LocalHost()
                : new NetworkAddress((string)configJson[ConfigSection.Network]["serverAddress"]);
            this.TickPublisherPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["tickPublisherPort"]);
            this.BarPublisherPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["barPublisherPort"]);
            this.HistoricalBarsPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["historicalBarsPort"]);

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
            this.SymbolIndex = JsonConvert.DeserializeObject<Dictionary<string, string>>(symbolsIndex);

            var symbols = (JArray)configJson[ConfigSection.Data]["symbols"];
            this.Symbols = symbols
                .Select(s => s.ToString())
                .Distinct();

            var barSpecs = (JArray)configJson[ConfigSection.Data]["barSpecifications"];
            this.BarSpecifications = barSpecs
                .Select(bs => bs.ToString())
                .Distinct()
                .Select(BarSpecificationFactory.Create);

            this.BarRollingWindowDays = (int)configJson[ConfigSection.Data]["barDataRollingWindowDays"];
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
        /// Gets the configuration tick publisher port.
        /// </summary>
        public NetworkPort TickPublisherPort { get; }

        /// <summary>
        /// Gets the configuration bar publisher port.
        /// </summary>
        public NetworkPort BarPublisherPort { get; }

        /// <summary>
        /// Gets the configuration historical bars publisher port.
        /// </summary>
        public NetworkPort HistoricalBarsPort { get; }

        /// <summary>
        /// Gets the FIX configuration.
        /// </summary>
        public FixConfiguration FixConfiguration { get; }

        /// <summary>
        /// Gets the symbol conversion index.
        /// </summary>
        public IReadOnlyDictionary<string, string> SymbolIndex { get; }

        /// <summary>
        /// Gets the configuration symbols.
        /// </summary>
        public IEnumerable<string> Symbols { get; }

        /// <summary>
        /// Gets the configuration bar specifications.
        /// </summary>
        public IEnumerable<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Gets the database bar rolling window days for trimming.
        /// </summary>
        public int BarRollingWindowDays { get; }
    }
}
