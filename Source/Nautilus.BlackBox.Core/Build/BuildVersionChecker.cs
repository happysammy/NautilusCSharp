//--------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Build
{
    using System;
    using System.Reflection;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The immutable static <see cref="BuildVersionChecker"/> class.
    /// </summary>
    [Immutable]
    public static class BuildVersionChecker
    {
        /// <summary>
        /// Runs the version checker which produces log events.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void Run(ILogger logger)
        {
            Validate.NotNull(logger, nameof(logger));

            logger.Information("---------------------------------------------------------------------------");
            logger.Information("NautilusBlackBox - Automated Algorithmic Trading Platform (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            logger.Information("---------------------------------------------------------------------------");
            logger.Information($"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            logger.Information($"Is64BitProcess={Environment.Is64BitProcess}");
            logger.Information($"{Environment.OSVersion}");
            logger.Information($"Microsoft.NET Framework (version {Environment.Version})");
        }
    }
}
