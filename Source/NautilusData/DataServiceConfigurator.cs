//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceConfigurator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Configuration;
using Nautilus.Common.Enums;
using Nautilus.Core.Extensions;
using Nautilus.Core.Types;
using Nautilus.Data;
using Nautilus.Data.Configuration;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Fix;
using Nautilus.Network;
using Nautilus.Network.Configuration;
using Nautilus.Network.Encryption;
using NodaTime;

namespace NautilusData
{
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
                configuration["Messaging:Encryption"].ToEnum<EncryptionAlgorithm>(),
                publicKeyEncoded,
                secretKeyEncoded);

            var compression = configuration["Messaging:Compression"];
            if (compression is null || compression.Equals(string.Empty))
            {
                compression = "None";
            }

            var wireConfig = new WireConfiguration(
                configuration["Messaging:ApiVersion"],
                compression.ToEnum<CompressionCodec>(),
                encryptionConfig,
                new Label(configuration["Messaging:ServiceName"]));

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
                new Port(int.Parse(networkSection["DataReqPort"])),
                new Port(int.Parse(networkSection["DataResPort"])),
                new Port(int.Parse(networkSection["DataPubPort"])),
                new Port(int.Parse(networkSection["TickPubPort"])));

            // Data Configuration
            var dataSection = configuration.GetSection(ConfigSection.Data);
            var subscribingSymbols = dataSection.GetSection("Symbols")
                .AsEnumerable()
                .Select(x => x.Value)
                .Where(x => x != null)
                .Distinct()
                .Select(Symbol.FromString)
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
                tickDataTrimTime,
                barDataTrimTime,
                tickDataTrimWindowDays,
                barDataTrimWindowDays);

            return new ServiceConfiguration(
                loggerFactory,
                fixConfig,
                wireConfig,
                networkConfig,
                dataConfig);
        }
    }
}
