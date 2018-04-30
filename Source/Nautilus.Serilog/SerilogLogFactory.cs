//--------------------------------------------------------------------------------------------------
// <copyright file="SerilogLogFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Serilog
{
    using global::Serilog;

    /// <summary>
    /// The log factory.
    /// </summary>
    public static class SerilogLogFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="logDatabaseName">
        /// The log Database Name.
        /// </param>
        public static void Create(string logDatabaseName)
        {
            {
                const string logTemplateDefault = "{Timestamp:yyyy/MM/dd HH:mm:ss.fff} [{ThreadId:00}][{Level:u3}] {Message}{NewLine}{Exception}";

                Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .Enrich.With(new ThreadIdEnricher())
                   .WriteTo.Console(outputTemplate: logTemplateDefault)
                   .WriteTo.RollingFile("Log/NautilusBlackBox-Log-{Date}.txt", outputTemplate: logTemplateDefault)
                   .CreateLogger();
            }
        }
    }
}
