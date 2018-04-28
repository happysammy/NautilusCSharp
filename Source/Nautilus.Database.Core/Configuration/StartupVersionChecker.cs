//--------------------------------------------------------------
// <copyright file="StartupVersionChecker.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core.Configuration
{
    using System;
    using System.Reflection;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;

    [Immutable]
    public static class StartupVersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void Run(ILoggingAdapter logger)
        {
            Validate.NotNull(logger, nameof(logger));

            logger.Information(LogLevel.Information, $"Running StartupVersionChecker...");
            logger.Information(LogLevel.Information, "----------------------------------------------------------------");
            logger.Information(LogLevel.Information, "NautilusDB - Financial Market Database Service (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            logger.Information(LogLevel.Information, "Copyright (c) 2018 by Nautech Systems Pty Ltd. All rights reserved.");
            logger.Information(LogLevel.Information, "----------------------------------------------------------------");
            logger.Information(LogLevel.Information, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            logger.Information(LogLevel.Information, $"Is64BitProcess={Environment.Is64BitProcess}");
            logger.Information(LogLevel.Information, $"OS {Environment.OSVersion}");
            logger.Information(LogLevel.Information, $".NET Core v{GetNetCoreVersion()}");
            logger.Information(LogLevel.Information, $"Akka.NET v1.3.5");
            logger.Information(LogLevel.Information, $"ServiceStack v5.0.2");
            logger.Information(LogLevel.Information, logger.AssemblyVersion);
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
