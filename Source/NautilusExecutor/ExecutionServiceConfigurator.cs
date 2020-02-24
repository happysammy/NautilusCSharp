//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceConfigurator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution;
    using Nautilus.Execution.Configuration;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Nautilus.Network.Configuration;
    using Nautilus.Network.Encryption;
    using NodaTime;

    /// <summary>
    /// Provides the <see cref="NautilusExecutor"/> service configuration.
    /// </summary>
    public static class ExecutionServiceConfigurator
    {
        /// <summary>
        /// Builds the service configuration.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The configuration.</returns>
        public static ServiceConfiguration Build(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;

            // Load keys
            var keysPath = configuration["Messaging:KeysPath"];
            var publicKeyPath = Path.Combine(keysPath, "server.key");
            var secretKeyPath = Path.Combine(keysPath, "server.key_secret");

            var publicKeyFileSplit = File.ReadAllText(publicKeyPath).Split("public-key = ");
            var secretKeyFileSplit = File.ReadAllText(secretKeyPath).Split("secret-key = ");
            var publicKey = publicKeyFileSplit[1].TrimStart('"').TrimEnd().TrimEnd('"');
            var secretKey = secretKeyFileSplit[1].TrimStart('"').TrimEnd().TrimEnd('"');

            var publicKeyEncoded = Z85Encoder.FromZ85String(publicKey);
            var secretKeyEncoded = Z85Encoder.FromZ85String(secretKey);

            var encryptionConfig = new EncryptionSettings(
                configuration["Messaging:Encryption"].ToEnum<CryptographicAlgorithm>(),
                publicKeyEncoded,
                secretKeyEncoded);

            var wireConfig = new WireConfiguration(
                configuration["Messaging:Version"],
                configuration["Messaging:Compression"].ToEnum<CompressionCodec>(),
                encryptionConfig);

            // FIX Configuration
            var fixConfigSection = configuration.GetSection(ConfigSection.FIX44);
            var fixConfigFileTarget = Path.Combine(fixConfigSection["ConfigPath"], fixConfigSection["ConfigFile"]);
            var fixConfigFile = Path.Combine(workingDirectory, fixConfigSection["ConfigFile"]);

            // Move configuration file to working directory
            FileManager.Copy(fixConfigFileTarget, workingDirectory);

            var fixSettings = ConfigReader.LoadConfig(fixConfigFile);
            var dataDictionary = Path.Combine(fixConfigSection["ConfigPath"], fixSettings["DataDictionary"]);

            // Move data dictionary to working directory
            FileManager.Copy(dataDictionary, workingDirectory);

            var broker = new Brokerage(fixSettings["Brokerage"]);
            var accountType = fixSettings["AccountType"].ToEnum<AccountType>();
            var accountCurrency = fixSettings["AccountCurrency"].ToEnum<Currency>();
            var credentials = new FixCredentials(
                fixSettings["Account"],
                fixSettings["Username"],
                fixSettings["Password"]);
            var sendAccountTag = Convert.ToBoolean(fixSettings["SendAccountTag"]);

            var connectionJob = fixConfigSection.GetSection("ConnectJob");
            var connectDay = connectionJob["Day"].ToEnum<IsoDayOfWeek>();
            var connectHour = int.Parse(connectionJob["Hour"]);
            var connectMinute = int.Parse(connectionJob["Minute"]);
            var connectWeeklyTime = new WeeklyTime(connectDay, new LocalTime(connectHour, connectMinute));

            var disconnectionJob = fixConfigSection.GetSection("DisconnectJob");
            var disconnectDay = disconnectionJob["Day"].ToEnum<IsoDayOfWeek>();
            var disconnectHour = int.Parse(disconnectionJob["Hour"]);
            var disconnectMinute = int.Parse(disconnectionJob["Minute"]);
            var disconnectWeeklyTime = new WeeklyTime(disconnectDay, new LocalTime(disconnectHour, disconnectMinute));

            var fixConfig = new FixConfiguration(
                broker,
                accountType,
                accountCurrency,
                fixConfigFile,
                credentials,
                sendAccountTag,
                connectWeeklyTime,
                disconnectWeeklyTime);

            var networkSection = configuration.GetSection(ConfigSection.Network);
            var networkConfig = new NetworkConfiguration(
                new Port(int.Parse(networkSection["CommandsPort"])),
                new Port(int.Parse(networkSection["EventsPort"])),
                int.Parse(networkSection["CommandsPerSecond"]),
                int.Parse(networkSection["NewOrdersPerSecond"]));

            // TODO: Refactor below
            var tempSymbolMap = configuration
                .GetSection("SymbolMap")
                .AsEnumerable()
                .ToImmutableDictionary();

            var symbolMap2 = new Dictionary<string, string>();
            foreach (var (key, value) in tempSymbolMap)
            {
                var strippedKey = key.Replace("SymbolMap:", string.Empty);
                symbolMap2.Add(strippedKey, value);
            }

            var symbolMap = symbolMap2.ToImmutableDictionary();

            return new ServiceConfiguration(
                loggerFactory,
                fixConfig,
                wireConfig,
                networkConfig,
                symbolMap);
        }
    }
}
