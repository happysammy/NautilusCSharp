//--------------------------------------------------------------------------------------------------
// <copyright file="StartupVersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Configuration
{
    using System;
    using System.Reflection;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Core.Enums;

    [Immutable]
    public static class StartupVersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="log">The logger.</param>
        public static void Run(ILoggingAdapter log)
        {
            Validate.NotNull(log, nameof(log));

            log.Information(DatabaseService.NautilusDatabase, "Running StartupVersionChecker...");
            log.Information(DatabaseService.NautilusDatabase, "----------------------------------------------------------------");
            log.Information(DatabaseService.NautilusDatabase, "NautilusDB - Financial Market Database Service (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            log.Information(DatabaseService.NautilusDatabase, "Copyright (c) 2018 by Nautech Systems Pty Ltd. All rights reserved.");
            log.Information(DatabaseService.NautilusDatabase, "----------------------------------------------------------------");
            log.Information(DatabaseService.NautilusDatabase, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            log.Information(DatabaseService.NautilusDatabase, $"Is64BitProcess={Environment.Is64BitProcess}");
            log.Information(DatabaseService.NautilusDatabase, $"OS {Environment.OSVersion}");
            log.Information(DatabaseService.NautilusDatabase, $".NET Core v{GetNetCoreVersion()}");
            log.Information(DatabaseService.NautilusDatabase, $"Akka.NET v1.3.5");
            log.Information(DatabaseService.NautilusDatabase, $"ServiceStack v5.0.2");
            log.Information(DatabaseService.NautilusDatabase, log.AssemblyVersion);
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
