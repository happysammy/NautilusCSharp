//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceConfigurator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Extensions;
    using Nautilus.Data;
    using Nautilus.Data.Configuration;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Nautilus.Network.Configuration;
    using Nautilus.Network.Encryption;
    using NodaTime;

    /// <summary>
    /// Provides the <see cref="NautilusData"/> service configuration.
    /// </summary>
    public static class DataServiceConfigurator
    {
        /// <summary>
        /// Builds the service configuration.
        /// </summary>
        /// <param name="loggerFactory">The logging adapter.</param>
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
            var connectTime = (connectDay, new LocalTime(connectHour, connectMinute));

            var disconnectionJob = fixConfigSection.GetSection("DisconnectJob");
            var disconnectDay = disconnectionJob["Day"].ToEnum<IsoDayOfWeek>();
            var disconnectHour = int.Parse(disconnectionJob["Hour"]);
            var disconnectMinute = int.Parse(disconnectionJob["Minute"]);
            var disconnectTime = (disconnectDay, new LocalTime(disconnectHour, disconnectMinute));

            var fixConfig = new FixConfiguration(
                broker,
                accountType,
                accountCurrency,
                fixConfigFile,
                credentials,
                sendAccountTag,
                connectTime,
                disconnectTime);

            var networkSection = configuration.GetSection(ConfigSection.Network);
            var networkConfig = new NetworkConfiguration(
                new NetworkPort(ushort.Parse(networkSection["TickRouterPort"])),
                new NetworkPort(ushort.Parse(networkSection["TickPubPort"])),
                new NetworkPort(ushort.Parse(networkSection["BarRouterPort"])),
                new NetworkPort(ushort.Parse(networkSection["BarPubPort"])),
                new NetworkPort(ushort.Parse(networkSection["InstrumentRouterPort"])),
                new NetworkPort(ushort.Parse(networkSection["InstrumentPubPort"])));

            // Data Configuration
            var dataSection = configuration.GetSection(ConfigSection.Data);
            var subscribingSymbols = dataSection.GetSection("Symbols")
                .AsEnumerable()
                .Select(x => x.Value)
                .Where(x => x != null)
                .Distinct()
                .Select(Symbol.FromString)
                .ToImmutableList();

            var barSpecifications = dataSection.GetSection("BarSpecifications")
                .AsEnumerable()
                .Select(x => x.Value)
                .Where(x => x != null)
                .Distinct()
                .Select(BarSpecification.FromString)
                .ToImmutableList();

            // Trim Job Ticks
            var trimJobTicks = dataSection.GetSection("TrimJobBars");
            var tickTrimHour = int.Parse(trimJobTicks["Hour"]);
            var tickTrimMinute = int.Parse(trimJobTicks["Minute"]);
            var tickDataTrimTime = new LocalTime(tickTrimHour, tickTrimMinute);
            var tickDataTrimWindowDays = int.Parse(trimJobTicks["WindowDays"]);

            // Trim Job Bars
            var trimJobBars = dataSection.GetSection("TrimJobBars");
            var barTrimHour = int.Parse(trimJobBars["Hour"]);
            var barTrimMinute = int.Parse(trimJobBars["Minute"]);
            var barDataTrimTime = new LocalTime(barTrimHour, barTrimMinute);
            var barDataTrimWindowDays = int.Parse(trimJobBars["WindowDays"]);

            var dataConfig = new DataConfiguration(
                subscribingSymbols,
                barSpecifications,
                tickDataTrimTime,
                barDataTrimTime,
                tickDataTrimWindowDays,
                barDataTrimWindowDays);

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
                dataConfig,
                symbolMap);
        }
    }
}
