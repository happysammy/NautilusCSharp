//--------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="Logger"/> class. Sends log events to the
    /// <see cref="ILoggingAdapter"/>.
    /// </summary>
    [Immutable]
    public sealed class Logger : ILogger
    {
        private readonly ILoggingAdapter loggingAdapter;
        private readonly Enum service;
        private readonly Label component;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="service">The black box service context.</param>
        /// <param name="component">The component label.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null.</exception>
        public Logger(
            ILoggingAdapter loggingAdapter,
            Enum service,
            Label component)
        {
            Validate.NotNull(loggingAdapter, nameof(loggingAdapter));
            Validate.NotNull(component, nameof(component));

            this.loggingAdapter = loggingAdapter;
            this.service = service;
            this.component = component;
        }

        /// <summary>
        /// Sends the given log level and log text to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        /// <exception cref="ValidationException">Throws if the log text is null or white space.</exception>
        /// <exception cref="InvalidOperationException">Throws if the log level is not recognized.</exception>
        public void Log(LogLevel logLevel, string logText)
        {
            Validate.NotNull(logText, nameof(logText));

            switch (logLevel)
            {
                case LogLevel.Verbose:
                    this.loggingAdapter.Verbose(this.service, $"{this.component}: {logText}");
                    break;

                case LogLevel.Information:
                    this.loggingAdapter.Information(this.service, $"{this.component}: {logText}");
                    break;

                case LogLevel.Debug:
                    this.loggingAdapter.Debug(this.service, $"{this.component}: {logText}");
                    break;

                case LogLevel.Warning:
                    this.loggingAdapter.Warning(this.service, $"{this.component}: {logText}");
                    break;

                case LogLevel.Error:
                    this.loggingAdapter.Error(this.service, $"{this.component}: {logText}", null);
                    break;

                case LogLevel.Fatal:
                    this.loggingAdapter.Fatal(this.service, $"{this.component}: {logText}", null);
                    break;
                default:
                    throw new InvalidOperationException(
                        $"The {nameof(logLevel)} log level not recognized by the logger.");
            }
        }

        /// <summary>
        /// Sends the given <see cref="Exception"/> to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <exception cref="ValidationException">Throws if the exception is null.</exception>
        public void LogException(Exception ex)
        {
            Validate.NotNull(ex, nameof(Exception));

            if (ex is ValidationException)
            {
                this.loggingAdapter.Error(this.service, $"{this.component}: {ex.Message} StackTrace: {ex.StackTrace}", ex);

                return;
            }

            this.loggingAdapter.Fatal(this.service, $"{this.component}: {ex.Message} StackTrace: {ex.StackTrace}", ex);
        }
    }
}
