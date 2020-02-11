//--------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogger.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
    public sealed class SerilogLogger : ILoggingAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="logLevel">The threshold level for the logger.</param>
        public SerilogLogger(LogEventLevel logLevel)
        {
            SerilogLogFactory.Create(logLevel);

            var assembly = Assembly.GetAssembly(typeof(Log))!;
            this.AssemblyVersion = $"Serilog v{assembly.GetName().Version}";
        }

        /// <inheritdoc />
        public string AssemblyVersion { get; }

        /// <inheritdoc />
        public void Verbose(string message)
        {
            Log.Verbose(message);
        }

        /// <inheritdoc />
        public void Debug(string message)
        {
            Log.Debug(message);
        }

        /// <inheritdoc />
        public void Information(string message)
        {
            Log.Information(message);
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            Log.Warning(message);
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            Log.Error(message);
        }

        /// <inheritdoc />
        public void Error(string message, Exception ex)
        {
            Log.Error(ex, message);
        }

        /// <inheritdoc />
        public void Fatal(string message, Exception ex)
        {
            Log.Fatal(ex, message);
        }
    }
}
