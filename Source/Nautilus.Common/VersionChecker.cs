//--------------------------------------------------------------------------------------------------
// <copyright file="VersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common
{
    using System;
    using System.Reflection;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides a means of checking dependency versions and outputting to the log at system initialization.
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

            log.Information(NautilusService.Core, "---------------------------------------------------------------------------");
            log.Information(NautilusService.Core, $"{serviceTitle} (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            log.Information(NautilusService.Core, "Copyright (c) 2015-2019 by Nautech Systems Pty Ltd. All rights reserved.");
            log.Information(NautilusService.Core, "---------------------------------------------------------------------------");
            log.Information(NautilusService.Core, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            log.Information(NautilusService.Core, $"Is64BitProcess={Environment.Is64BitProcess}");
            log.Information(NautilusService.Core, $"OS {Environment.OSVersion}");
            log.Information(NautilusService.Core, $".NET Core v{GetNetCoreVersion()}");
            log.Information(NautilusService.Core, log.AssemblyVersion);
        }

        private static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
            {
                return assemblyPath[netCoreAppIndex + 1];
            }

            return string.Empty;
        }
    }
}
