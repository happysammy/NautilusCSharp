//--------------------------------------------------------------------------------------------------
// <copyright file="VersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides a means of checking dependency versions and outputting to the log at service initialization.
    /// </summary>
    public static class VersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="log">The logger.</param>
        /// <param name="serviceTitle">The service title string.</param>
        public static void Run(ILoggingAdapter log, string serviceTitle)
        {
            Condition.NotEmptyOrWhiteSpace(serviceTitle, nameof(serviceTitle));

            log.Information("=================================================================");
            log.Information(@"  _   _           _    _  _______  _____  _      _    _   _____ ");
            log.Information(@" | \ | |    /\   | |  | ||__   __||_   _|| |    | |  | | / ____|");
            log.Information(@" |  \| |   /  \  | |  | |   | |     | |  | |    | |  | || (___  ");
            log.Information(@" | . ` |  / /\ \ | |  | |   | |     | |  | |    | |  | | \___ \ ");
            log.Information(@" | |\  | / ____ \| |__| |   | |    _| |_ | |____| |__| | ____) |");
            log.Information(@" |_| \_|/_/    \_\\____/    |_|   |_____||______|\____/ |_____/ ");
            log.Information("                                                                 ");
            log.Information($" {serviceTitle}");
            log.Information(" by Nautech Systems Pty Ltd.");
            log.Information(" Copyright (C) 2015-2020 All rights reserved.");
            log.Information("=================================================================");
            log.Information(" SYSTEM SPECIFICATION");
            log.Information("=================================================================");
            log.Information($"CPU architecture: {RuntimeInformation.ProcessArchitecture}");
            log.Information($"CPU(s): {Environment.ProcessorCount}");
            log.Information($"RAM-Avail: {Math.Round((decimal)Environment.WorkingSet / 1000000, 2)} GB");
            log.Information($"OS: {Environment.OSVersion}");
            log.Information($"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            log.Information($"Is64BitProcess={Environment.Is64BitProcess}");
            log.Information("=================================================================");
            log.Information(" VERSIONING");
            log.Information("=================================================================");
            log.Information($"{GetNetCoreVersion()}");
            log.Information($"Nautilus {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
            log.Information("=================================================================");
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
