//--------------------------------------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using Nautilus.Common.Interfaces;
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
            this.loggingAdapter = loggingAdapter;
        }

        /// <inheritdoc />
        public ILogger Create(Label component)
        {
            return new Logger(this.loggingAdapter, component);
        }
    }
}
