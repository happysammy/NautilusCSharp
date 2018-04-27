// -------------------------------------------------------------------------------------------------
// <copyright file="ILoggingAdapter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System;
    using Nautilus.BlackBox.Core.Enums;

    /// <summary>
    /// The <see cref="ILoggingAdapter"/> interface.
    /// </summary>
    public interface ILoggingAdapter
    {
        /// <summary>
        /// Logs the given message with a verbose <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        void Verbose(BlackBoxService service, string message);

        /// <summary>
        /// Logs the given message with a debug <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        void Debug(BlackBoxService service, string message);

        /// <summary>
        /// Logs the given message with an information <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        void Information(BlackBoxService service, string message);

        /// <summary>
        /// Logs the given message with a warning <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        void Warning(BlackBoxService service, string message);

        /// <summary>
        /// Logs the given message and exception with an error <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Error(BlackBoxService service, string message, Exception ex);

        /// <summary>
        /// Logs the given message and exception with a fatal <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Fatal(BlackBoxService service, string message, Exception ex);
    }
}