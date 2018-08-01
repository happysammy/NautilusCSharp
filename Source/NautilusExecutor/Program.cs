//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using Nautilus.Serilog;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore;
    using Nautilus.Common.Enums;
    using Serilog.Events;
    using global::Serilog;

    /// <summary>
    /// The main entry point for the program.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var logger = new SerilogLogger(LogEventLevel.Information);
            logger.Information(NautilusService.AspCoreHost, "Building ASP.NET Core Web Host...");

            BuildWebHost(args).Run();

            logger.Information(NautilusService.AspCoreHost, "Closing and flushing Serilog...");
            Log.CloseAndFlush();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
