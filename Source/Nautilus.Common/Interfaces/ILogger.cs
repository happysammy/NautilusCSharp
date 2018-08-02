//--------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using Nautilus.Core.CQS;

    /// <summary>
    /// The adapter interface for logging with the <see cref="ILoggingAdapter"/>.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Sends the given verbose message to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Verbose(string message);

        /// <summary>
        /// Sends the given information message to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Information(string message);

        /// <summary>
        /// Sends the given debug message to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Sends the given warning message to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Sends the given error message and exception to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception to log.</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// Sends the given fatal message and exception to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="ex">The exception to log.</param>
        void Fatal(string message, Exception ex);

        /// <summary>
        /// Sends the message of the given result at an appropriate to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="result">The result to handle.</param>
        void Result(ResultBase result);
    }
}
