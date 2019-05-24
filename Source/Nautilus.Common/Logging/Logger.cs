//--------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a logger with sends log events to the <see cref="ILoggingAdapter"/>.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private readonly ILoggingAdapter loggingAdapter;
        private readonly Label component;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="component">The component name.</param>
        public Logger(ILoggingAdapter loggingAdapter, Label component)
        {
            this.loggingAdapter = loggingAdapter;
            this.component = component;
        }

        /// <inheritdoc />
        public void Verbose(string message)
        {
            this.loggingAdapter.Verbose($"{this.component}: {message}");
        }

        /// <inheritdoc />
        public void Information(string message)
        {
            this.loggingAdapter.Information($"{this.component}: {message}");
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            this.loggingAdapter.Debug($"{this.component}: {message}");
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            this.loggingAdapter.Warning($"{this.component}: {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            this.loggingAdapter.Error($"{this.component}: {message}");
        }

        /// <inheritdoc />
        public void Error(string message, Exception ex)
        {
            this.loggingAdapter.Error($"{this.component}: {message}", ex);
        }

        /// <inheritdoc />
        public void Fatal(string message, Exception ex)
        {
            this.loggingAdapter.Fatal($"{this.component}: {message}", ex);
        }
    }
}
