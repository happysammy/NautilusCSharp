//--------------------------------------------------------------------------------------------------
// <copyright file="StartupVersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Build
{
    using System;
    using System.Reflection;
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Enums;

    /// <summary>
    /// Provides a startup version checker with logging.
    /// </summary>
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

            log.Information(DatabaseService.Core, "Running startup version checker...");
            log.Information(DatabaseService.Core, "------------------------------------------------------------------------");
            log.Information(DatabaseService.Core, "NautilusDB - Financial Market Database Service (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            log.Information(DatabaseService.Core, "Copyright (c) 2015-2018 by Nautech Systems Pty Ltd. All rights reserved.");
            log.Information(DatabaseService.Core, "------------------------------------------------------------------------");
            log.Information(DatabaseService.Core, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            log.Information(DatabaseService.Core, $"Is64BitProcess={Environment.Is64BitProcess}");
            log.Information(DatabaseService.Core, $"OS {Environment.OSVersion}");
            log.Information(DatabaseService.Core, $".NET Core v{GetNetCoreVersion()}");
            log.Information(DatabaseService.Core, $"Akka.NET v{Assembly.GetAssembly(typeof(ReceiveActor)).GetName().Version}");
            log.Information(DatabaseService.Core, $"ServiceStack v{Assembly.GetAssembly(typeof(ServiceStack.Service)).GetName().Version}");
            log.Information(DatabaseService.Core, log.AssemblyVersion);
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
