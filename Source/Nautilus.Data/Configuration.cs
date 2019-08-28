//--------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
    using Nautilus.DomainModel.Identifiers;
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
                ? NetworkAddress.LocalHost
                : new NetworkAddress((string)configJson[ConfigSection.Network]["serverAddress"]);
            this.TickRequestPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["tickReqPort"]);
            this.TickSubscribePort = new NetworkPort((ushort)configJson[ConfigSection.Network]["tickSubPort"]);
            this.BarRequestPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["barReqPort"]);
            this.BarSubscribePort = new NetworkPort((ushort)configJson[ConfigSection.Network]["barSubPort"]);
            this.InstrumentRequestPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["instrumentReqPort"]);
            this.InstrumentSubscribePort = new NetworkPort((ushort)configJson[ConfigSection.Network]["instrumentSubPort"]);

            // FIX Settings
            var fixConfigFile = (string)configJson[ConfigSection.Fix44]["configFile"];
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
            var configPath = Path.GetFullPath(Path.Combine(assemblyDirectory, fixConfigFile));

            var fixSettings = ConfigReader.LoadConfig(configPath);
            var broker = new Brokerage(fixSettings["Brokerage"]);
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
            this.SymbolIndex =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(symbolsIndex);

            var symbols = (JArray)configJson[ConfigSection.Data]["symbols"];
            this.SubscribingSymbols = symbols
                .Select(s => new Symbol(s.ToString(), fixSettings["Brokerage"]))
                .Distinct()
                .ToImmutableList();

            var barSpecs = (JArray)configJson[ConfigSection.Data]["barSpecifications"];
            this.BarSpecifications = barSpecs
                .Select(bs => bs.ToString())
                .Distinct()
                .Select(BarSpecification.FromString)
                .ToImmutableList();

            var tickTrimDay = configJson[ConfigSection.Data]["tickDataTrimDay"].ToString().ToEnum<IsoDayOfWeek>();
            var tickTrimHour = (int)configJson[ConfigSection.Data]["tickDataTrimHour"];
            var tickTrimMinute = (int)configJson[ConfigSection.Data]["tickDataTrimMinute"];
            this.TickDataTrimTime = (tickTrimDay, new LocalTime(tickTrimHour, tickTrimMinute));
            this.TickDataTrimWindowDays = (int)configJson[ConfigSection.Data]["tickDataTrimWindowDays"];

            var barTrimDay = configJson[ConfigSection.Data]["barDataTrimDay"].ToString().ToEnum<IsoDayOfWeek>();
            var barTrimHour = (int)configJson[ConfigSection.Data]["barDataTrimHour"];
            var barTrimMinute = (int)configJson[ConfigSection.Data]["barDataTrimMinute"];
            this.BarDataTrimTime = (barTrimDay, new LocalTime(barTrimHour, barTrimMinute));
            this.BarDataTrimWindowDays = (int)configJson[ConfigSection.Data]["barDataTrimWindowDays"];
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggingAdapter LoggingAdapter { get; }

        /// <summary>
        /// Gets the network configuration server address.
        /// </summary>
        public NetworkAddress ServerAddress { get; }

        /// <summary>
        /// Gets the network configuration tick request port.
        /// </summary>
        public NetworkPort TickRequestPort { get; }

        /// <summary>
        /// Gets the network configuration tick subscribe port.
        /// </summary>
        public NetworkPort TickSubscribePort { get; }

        /// <summary>
        /// Gets the network configuration bar request port.
        /// </summary>
        public NetworkPort BarRequestPort { get; }

        /// <summary>
        /// Gets the network configuration bar subscribe port.
        /// </summary>
        public NetworkPort BarSubscribePort { get; }

        /// <summary>
        /// Gets the network configuration instrument request port.
        /// </summary>
        public NetworkPort InstrumentRequestPort { get; }

        /// <summary>
        /// Gets the network configuration instrument subscribe port.
        /// </summary>
        public NetworkPort InstrumentSubscribePort { get; }

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
        /// Gets the time to trim the tick data.
        /// </summary>
        public (IsoDayOfWeek, LocalTime) TickDataTrimTime { get; }

        /// <summary>
        /// Gets the time to trim the bar data.
        /// </summary>
        public (IsoDayOfWeek, LocalTime) BarDataTrimTime { get; }

        /// <summary>
        /// Gets the tick data rolling trim window in days.
        /// </summary>
        public int TickDataTrimWindowDays { get; }

        /// <summary>
        /// Gets the bar data rolling trim window in days.
        /// </summary>
        public int BarDataTrimWindowDays { get; }
    }
}
