//--------------------------------------------------------------------------------------------------
// <copyright file="ILoggingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;

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
        /// <param name="message">The log message.</param>
        void Verbose(string message);

        /// <summary>
        /// Logs the given message with a debug log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        void Debug(string message);

        /// <summary>
        /// Logs the given message with an information log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        void Information(string message);

        /// <summary>
        /// Logs the given message with a warning log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        void Warning(string message);

        /// <summary>
        /// Logs the given message with an error log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        void Error(string message);

        /// <summary>
        /// Logs the given message and exception with an error log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// Logs the given message and exception with a fatal log level.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        void Fatal(string message, Exception ex);
    }
}
