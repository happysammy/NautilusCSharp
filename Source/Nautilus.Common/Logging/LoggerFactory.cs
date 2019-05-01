//--------------------------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory for creating <see cref="Logger"/>s.
    /// </summary>
    public sealed class LoggerFactory : ILoggerFactory
    {
        private readonly ILoggingAdapter loggingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactory"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The logging adapter.</param>
        public LoggerFactory(ILoggingAdapter loggingAdapter)
        {
            Validate.NotNull(loggingAdapter, nameof(loggingAdapter));

            this.loggingAdapter = loggingAdapter;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ILogger"/> from the given inputs.
        /// </summary>
        /// <param name="service">The service context.</param>
        /// <param name="component">The component label.</param>
        /// <returns>The logger.</returns>
        public ILogger Create(NautilusService service, Label component)
        {
            Validate.NotNull(component, nameof(component));

            return new Logger(this.loggingAdapter, service, component);
        }
    }
}
