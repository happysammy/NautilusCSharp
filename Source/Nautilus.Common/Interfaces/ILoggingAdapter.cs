//--------------------------------------------------------------
// <copyright file="ILoggerAdapter.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using NautechSystems.CSharp.CQS;
    using Nautilus.Common.Enums;

    /// <summary>
    /// The interface between the abstract application logger and the logging framework.
    /// </summary>
    public interface ILoggingAdapter
    {
        /// <summary>
        /// Gets the logging frameworks name and version.
        /// </summary>
        /// <returns></returns>
        string AssemblyVersion { get; }

        /// <summary>
        /// Logs the given message with a verbose <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Verbose(Enum service, string message);

        /// <summary>
        /// Logs the given message with a debug <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Debug(Enum service, string message);

        /// <summary>
        /// Logs the given message with an information <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Information(Enum service, string message);

        /// <summary>
        /// Logs the given message with a warning <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Warning(Enum service, string message);

        /// <summary>
        /// Logs the given message and exception with an error <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Error(Enum service, string message, Exception ex);

        /// <summary>
        /// Logs the given message and exception with a fatal <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Fatal(Enum service, string message, Exception ex);

        /// <summary>
        /// Logs the given result with a <see cref="LogLevel"/> of Information is success, or
        /// Warning if failure.
        /// </summary>
        /// <param name="result">The command result.</param>
        void LogResult(ResultBase result);
    }
}
