// -------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System;
    using Nautilus.BlackBox.Core.Enums;

    /// <summary>
    /// The <see cref="ILogger"/> interface. Sends log events to the <see cref="ILoggingAdapter"/>.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Sends the given log level and log text to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        void Log(LogLevel logLevel, string logText);

        /// <summary>
        /// Sends the given <see cref="Exception"/> to the <see cref="ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="ex">The exception.</param>
        void LogException(Exception ex);
    }
}