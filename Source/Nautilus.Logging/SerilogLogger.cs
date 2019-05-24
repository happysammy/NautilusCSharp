//--------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Logging
{
    using System;
    using System.Reflection;
    using Nautilus.Common.Interfaces;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// The <see cref="Nautilus.Logging"/> adapter.
    /// </summary>
    public class SerilogLogger : ILoggingAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="logLevel">The threshold level for the logger.</param>
        public SerilogLogger(LogEventLevel logLevel)
        {
            SerilogLogFactory.Create(logLevel);

            this.AssemblyVersion = $"Serilog v{Assembly.GetAssembly(typeof(Log)).GetName().Version}";
        }

        /// <summary>
        /// Gets the logging adapters assembly library and version.
        /// </summary>
        public string AssemblyVersion { get; }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Verbose(string message)
        {
            Log.Verbose(message);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Debug(string message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// The information.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Information(string message)
        {
            Log.Information(message);
        }

        /// <summary>
        /// The warning.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Warning(string message)
        {
            Log.Warning(message);
        }

        /// <summary>
        /// Creates an error log event.
        /// </summary>
        /// <param name="message">The log message.</param>
        public void Error(string message)
        {
            Log.Error(message);
        }

        /// <summary>
        /// Creates an error log event including an exception.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The exception.</param>
        public void Error(string message, Exception ex)
        {
            Log.Error(ex, message);
        }

        /// <summary>
        /// Creates a fatal log event.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="ex">The fatal exception.</param>
        public void Fatal(string message, Exception ex)
        {
            Log.Fatal(ex, message);
        }
    }
}
