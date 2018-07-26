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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Enums;
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
        public static void Run(ILoggingAdapter logger)
        {
            Validate.NotNull(logger, nameof(logger));

            logger.Information(NautilusService.BlackBox, "---------------------------------------------------------------------------");
            logger.Information(NautilusService.BlackBox, "NautilusBlackBox - Automated Algorithmic Trading Platform (version " + Assembly.GetExecutingAssembly().GetName().Version + ")");
            logger.Information(NautilusService.BlackBox, "Copyright (c) 2015-2018 by Nautech Systems Pty Ltd. All rights reserved.");
            logger.Information(NautilusService.BlackBox, "---------------------------------------------------------------------------");
            logger.Information(NautilusService.BlackBox, $"Is64BitOperatingSystem={Environment.Is64BitOperatingSystem}");
            logger.Information(NautilusService.BlackBox, $"Is64BitProcess={Environment.Is64BitProcess}");
            logger.Information(NautilusService.BlackBox, $"{Environment.OSVersion}");
            logger.Information(NautilusService.BlackBox, $"Microsoft.NET Framework (version {Environment.Version})");
        }
    }
}
