//--------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NodaTime;

    /// <summary>
    /// Represents an <see cref="ExecutionService"/> configuration.
    /// </summary>
    public sealed class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="configJson">The parsed configuration JSON.</param>
        /// <param name="symbolIndex">The parsed symbols index string.</param>
        public Configuration(
            ILoggingAdapter loggingAdapter,
            JObject configJson,
            string symbolIndex)
        {
            this.LoggingAdapter = loggingAdapter;

            // Network Settings
            this.CommandsPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["commandsPort"]);
            this.EventsPort = new NetworkPort((ushort)configJson[ConfigSection.Network]["eventsPort"]);
            this.CommandsPerSecond = (int)configJson[ConfigSection.Network]["commandsPerSecond"];
            this.NewOrdersPerSecond = (int)configJson[ConfigSection.Network]["newOrdersPerSecond"];

            // FIX Settings
            var fixConfigFile = (string)configJson[ConfigSection.Fix44]["configFile"];
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) !;
            var configPath = Path.GetFullPath(Path.Combine(assemblyDirectory, fixConfigFile));

            var fixSettings = ConfigReader.LoadConfig(configPath);
            var broker = new Brokerage(fixSettings["Brokerage"]);
            var accountType = fixSettings["AccountType"].ToEnum<AccountType>();
            var accountCurrency = fixSettings["AccountCurrency"].ToEnum<Currency>();
            var credentials = new FixCredentials(
                fixSettings["Account"],
                fixSettings["Username"],
                fixSettings["Password"]);
            var sendAccountTag = Convert.ToBoolean(fixSettings["SendAccountTag"]);

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
                accountType,
                accountCurrency,
                configPath,
                credentials,
                sendAccountTag,
                connectTime,
                disconnectTime);

            // Data Settings
            this.SymbolIndex = JsonConvert.DeserializeObject<ImmutableDictionary<string, string>>(symbolIndex);
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggingAdapter LoggingAdapter { get; }

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
        public ImmutableDictionary<string, string> SymbolIndex { get; }
    }
}
