//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceConfigurator.cs" company="Nautech Systems Pty Ltd">
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

namespace NautilusExecutor
{
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
                new Port(int.Parse(networkSection["CommandReqPort"])),
                new Port(int.Parse(networkSection["CommandResPort"])),
                new Port(int.Parse(networkSection["EventPubPort"])),
                int.Parse(networkSection["CommandsPerSecond"]),
                int.Parse(networkSection["NewOrdersPerSecond"]));

            return new ServiceConfiguration(
                loggerFactory,
                fixConfig,
                wireConfig,
                networkConfig);
        }
    }
}
