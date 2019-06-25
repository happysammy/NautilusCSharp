//--------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Logging
{
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Provides a factory for the <see cref="Nautilus.Logging"/> logger.
    /// </summary>
    public static class SerilogLogFactory
    {
        /// <summary>
        /// Creates a new global static <see cref="Serilog"/> logger.
        /// </summary>
        /// <param name="logLevel">The threshold level for the logger.</param>
        public static void Create(LogEventLevel logLevel)
        {
            {
                const string logTemplateDefault = "{Timestamp:yyyy/MM/dd HH:mm:ss.fff} [{ThreadId:00}][{Level:u3}] {Message}{NewLine}{Exception}";

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .Enrich.With(new ThreadIdEnricher())
                    .WriteTo.Console(restrictedToMinimumLevel: logLevel, logTemplateDefault)
                    .WriteTo.RollingFile(
                        "Logs/Nautilus-Log-{Date}.txt",
                        restrictedToMinimumLevel: logLevel,
                        outputTemplate: logTemplateDefault)
                    .CreateLogger();
            }
        }
    }
}
