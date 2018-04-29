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
    using Nautilus.Common.Enums;

    /// <summary>
    /// The <see cref="ILogger"/> interface. Sends log events to the <see cref="ILoggingAdapter"/>.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Sends the given log level and log text to the <see cref="Akka.Event.ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logText">The log text.</param>
        void Log(LogLevel logLevel, string logText);

        /// <summary>
        /// Sends the given <see cref="Exception"/> to the <see cref="Akka.Event.ILoggingAdapter"/> to log.
        /// </summary>
        /// <param name="ex">The exception.</param>
        void LogException(Exception ex);
    }
}
