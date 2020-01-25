//--------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Types;

    /// <summary>
    /// Provides a logger with sends log events to the <see cref="ILoggingAdapter"/>.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private readonly ILoggingAdapter loggingAdapter;
        private readonly string componentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="component">The component name.</param>
        public Logger(ILoggingAdapter loggingAdapter, Label component)
        {
            this.loggingAdapter = loggingAdapter;
            this.componentName = component.Value;
        }

        /// <inheritdoc />
        public void Verbose(string message)
        {
            this.loggingAdapter.Verbose($"{this.componentName}: {message}");
        }

        /// <inheritdoc />
        public void Information(string message)
        {
            this.loggingAdapter.Information($"{this.componentName}: {message}");
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            this.loggingAdapter.Debug($"{this.componentName}: {message}");
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            this.loggingAdapter.Warning($"{this.componentName}: {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            this.loggingAdapter.Error($"{this.componentName}: {message}");
        }

        /// <inheritdoc />
        public void Error(string message, Exception ex)
        {
            this.loggingAdapter.Error($"{this.componentName}: {message}", ex);
        }

        /// <inheritdoc />
        public void Fatal(string message, Exception ex)
        {
            this.loggingAdapter.Fatal($"{this.componentName}: {message}", ex);
        }
    }
}
