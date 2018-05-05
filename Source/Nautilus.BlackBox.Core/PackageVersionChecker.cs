//--------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionChecker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using System.Reflection;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
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
        public static void Run(ILogger log)
        {
            Validate.NotNull(log, nameof(log));

            log.Information("---------------------------------------------------------------------------");
            log.Information("NautilusBlackBox - Automated Algorithmic Trading Platform (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            log.Information("---------------------------------------------------------------------------");
            log.Information($"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            log.Information($"Is64BitProcess={Environment.Is64BitProcess}");
            log.Information($"{Environment.OSVersion}");
            log.Information($"Microsoft.NET Framework (version {Environment.Version})");
        }
    }
}
