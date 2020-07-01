//--------------------------------------------------------------------------------------------------
// <copyright file="VersionChecker.cs" company="Nautech Systems Pty Ltd">
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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Nautilus.Core.Correctness;

namespace Nautilus.Common.Configuration
{
    /// <summary>
    /// Provides a means of checking dependency versions and outputting to the log at service initialization.
    /// </summary>
    public static class VersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="logger">The logger for the version check.</param>
        /// <param name="serviceHeader">The service header string.</param>
        public static void Run(ILogger logger, string serviceHeader)
        {
            Condition.NotEmptyOrWhiteSpace(serviceHeader, nameof(serviceHeader));

            logger.LogInformation("=================================================================");
            logger.LogInformation(@"   _   _   ___   _   _  _____  _____  _      _   _  _____   ");
            logger.LogInformation(@"  | \ | | / _ \ | | | ||_   _||_   _|| |    | | | |/  ___|  ");
            logger.LogInformation(@"  |  \| |/ /_\ \| | | |  | |    | |  | |    | | | |\ `--.   ");
            logger.LogInformation(@"  | . ` ||  _  || | | |  | |    | |  | |    | | | | `--. \  ");
            logger.LogInformation(@"  | |\  || | | || |_| |  | |   _| |_ | |____| |_| |/\__/ /  ");
            logger.LogInformation(@"  \_| \_/\_| |_/ \___/   \_/   \___/ \_____/ \___/ \____/   ");
            logger.LogInformation("                                                             ");
            logger.LogInformation($" {serviceHeader}");
            logger.LogInformation(" by Nautech Systems Pty Ltd.");
            logger.LogInformation(" Copyright (C) 2015-2020 All rights reserved.");
            logger.LogInformation("=================================================================");
            logger.LogInformation(" SYSTEM SPECIFICATION");
            logger.LogInformation("=================================================================");
            logger.LogInformation($"CPU architecture: {RuntimeInformation.ProcessArchitecture}");
            logger.LogInformation($"CPU(s): {Environment.ProcessorCount}");
            logger.LogInformation($"RAM-Avail: {Math.Round((decimal)Environment.WorkingSet / 1000000, 2)} GB");
            logger.LogInformation($"OS: {Environment.OSVersion}");
            logger.LogInformation($"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            logger.LogInformation($"Is64BitProcess={Environment.Is64BitProcess}");
            logger.LogInformation("=================================================================");
            logger.LogInformation(" VERSIONING");
            logger.LogInformation("=================================================================");
            logger.LogInformation($"{GetNetCoreVersion()}");
            logger.LogInformation($"Nautilus {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
            logger.LogInformation("=================================================================");
        }

        private static string GetNetCoreVersion()
        {
            return Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName ?? string.Empty;
        }
    }
}
