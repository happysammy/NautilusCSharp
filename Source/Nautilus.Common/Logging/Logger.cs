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
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Sends log events to the <see cref="ILoggingAdapter"/>.
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
        /// <param name="service">The service context.</param>
        /// <param name="component">The component name.</param>
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

        public void Verbose(string message)
        {
            Validate.NotNull(message, nameof(message));

            this.loggingAdapter.Verbose(this.service, $"{this.component}: {message}");
        }

        public void Information(string message)
        {
            Validate.NotNull(message, nameof(message));

            this.loggingAdapter.Information(this.service, $"{this.component}: {message}");
        }

        public void Debug(string message)
        {
            Validate.NotNull(message, nameof(message));

            this.loggingAdapter.Debug(this.service, $"{this.component}: {message}");
        }

        public void Warning(string message)
        {
            Validate.NotNull(message, nameof(message));

            this.loggingAdapter.Warning(this.service, $"{this.component}: {message}");
        }

        public void Error(string message, Exception ex)
        {
            Validate.NotNull(message, nameof(message));
            Validate.NotNull(ex, nameof(ex));

            this.loggingAdapter.Error(this.service, $"{this.component}: {message}", ex);
        }

        public void Fatal(string message, Exception ex)
        {
            Validate.NotNull(message, nameof(message));
            Validate.NotNull(ex, nameof(ex));

            this.loggingAdapter.Fatal(this.service, $"{this.component}: {message}", ex);
        }

        /// <summary>
        /// Sends the message of the given result at an appropriate to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="result">The result to handle.</param>
        public void Result(ResultBase result)
        {
            Validate.NotNull(result, nameof(result));

            if (result.IsSuccess)
            {
                this.Information(result.Message);
            }
            else
            {
                this.Warning(result.Message);
            }
        }
    }
}
