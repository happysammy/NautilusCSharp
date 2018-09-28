//--------------------------------------------------------------------------------------------------
// <copyright file="ILoggingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using Nautilus.Common.Enums;

    /// <summary>
    /// The interface between the abstract application logger and the logging framework.
    /// </summary>
    public interface ILoggingAdapter
    {
        /// <summary>
        /// Gets the logging frameworks name and version.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        string AssemblyVersion { get; }

        /// <summary>
        /// Logs the given message with a verbose log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Verbose(NautilusService service, string message);

        /// <summary>
        /// Logs the given message with a debug log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Debug(NautilusService service, string message);

        /// <summary>
        /// Logs the given message with an information log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Information(NautilusService service, string message);

        /// <summary>
        /// Logs the given message with a warning log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Warning(NautilusService service, string message);

        /// <summary>
        /// Logs the given message with an error log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        void Error(NautilusService service, string message);

        /// <summary>
        /// Logs the given message and exception with an error log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Error(NautilusService service, string message, Exception ex);

        /// <summary>
        /// Logs the given message and exception with a fatal log level.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Fatal(NautilusService service, string message, Exception ex);
    }
}
