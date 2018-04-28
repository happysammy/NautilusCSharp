//--------------------------------------------------------------
// <copyright file="PackageVersionChecker.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using System.Reflection;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The immutable static <see cref="PackageVersionChecker"/> class.
    /// </summary>
    [Immutable]
    public static class PackageVersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void Run(ILogger logger)
        {
            Validate.NotNull(logger, nameof(logger));

            logger.Log(LogLevel.Information, "---------------------------------------------------------------------------");
            logger.Log(LogLevel.Information, "NautilusBlackBox - Automated Algorithmic Trading Platform (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            logger.Log(LogLevel.Information, "---------------------------------------------------------------------------");
            logger.Log(LogLevel.Information, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            logger.Log(LogLevel.Information, $"Is64BitProcess={Environment.Is64BitProcess}");
            logger.Log(LogLevel.Information, $"{Environment.OSVersion}");
            logger.Log(LogLevel.Information, $"Microsoft.NET Framework (version {Environment.Version})");
        }
    }
}
