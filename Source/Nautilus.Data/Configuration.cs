//--------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NodaTime;

    /// <summary>
    /// Represents a <see cref="DataService"/> configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="configJson">The parsed configuration JSON.</param>
        /// <param name="symbolsIndex">The parsed symbols index string.</param>
        /// <param name="isDevelopment">The flag indicating whether the hosting environment is development.</param>
        public Configuration(
            ILoggingAdapter loggingAdapter,
            JObject configJson,
            string symbolsIndex,
            bool isDevelopment)
        {
            this.LoggingAdapter = loggingAdapter;

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

            var connectDay = configJson[ConfigSection.Fix44]["connectDay"].ToString().ToEnum<IsoDayOfWeek>();
            var connectHour = (int)configJson[ConfigSection.Fix44]["connectHour"];
            var connectMinute = (int)configJson[ConfigSection.Fix44]["connectMinute"];
            var connectTime = (connectDay, new LocalTime(connectHour, connectMinute));

            var disconnectDay = configJson[ConfigSection.Fix44]["disconnectDay"].ToString().ToEnum<IsoDayOfWeek>();
            var disconnectHour = (int)configJson[ConfigSection.Fix44]["disconnectHour"];
            var disconnectMinute = (int)configJson[ConfigSection.Fix44]["disconnectMinute"];
            var disconnectTime = (disconnectDay, new LocalTime(disconnectHour, disconnectMinute));

            this.FixConfiguration = new FixConfiguration(
                broker,
                configPath,
                credentials,
                Convert.ToBoolean(fixSettings["SendAccountTag"]),
                connectTime,
                disconnectTime);

            // Data Settings
            this.SymbolIndex = JsonConvert.DeserializeObject<Dictionary<string, string>>(symbolsIndex);

            var symbols = (JArray)configJson[ConfigSection.Data]["symbols"];
            this.SubscribingSymbols = symbols
                .Select(s => new Symbol(s.ToString(), fixSettings["Brokerage"].ToEnum<Venue>()))
                .Distinct()
                .ToImmutableList();

            var barSpecs = (JArray)configJson[ConfigSection.Data]["barSpecifications"];
            this.BarSpecifications = barSpecs
                .Select(bs => bs.ToString())
                .Distinct()
                .Select(BarSpecificationFactory.Create)
                .ToImmutableList();

            var trimDay = configJson[ConfigSection.Data]["barDataTrimDay"].ToString().ToEnum<IsoDayOfWeek>();
            var trimHour = (int)configJson[ConfigSection.Data]["barDataTrimHour"];
            var trimMinute = (int)configJson[ConfigSection.Data]["barDataTrimMinute"];
            this.BarDataTrimTime = (trimDay, new LocalTime(trimHour, trimMinute));
            this.BarDataTrimWindowDays = (int)configJson[ConfigSection.Data]["barDataTrimWindowDays"];
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggingAdapter LoggingAdapter { get; }

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
        /// Gets the subscribing symbols.
        /// </summary>
        public IReadOnlyCollection<Symbol> SubscribingSymbols { get; }

        /// <summary>
        /// Gets the configuration bar specifications.
        /// </summary>
        public IReadOnlyCollection<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Gets the time to trim the bar data.
        /// </summary>
        public (IsoDayOfWeek, LocalTime) BarDataTrimTime { get; }

        /// <summary>
        /// Gets the database bar rolling trim window in days.
        /// </summary>
        public int BarDataTrimWindowDays { get; }
    }
}
