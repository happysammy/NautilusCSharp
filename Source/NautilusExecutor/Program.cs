//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System.IO;
    using global::Serilog;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nautilus.Common.Enums;
    using Nautilus.Serilog;
    using Serilog.Events;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public static void Main(string[] args)
        {
            var logger = new SerilogLogger(LogEventLevel.Information);
            logger.Debug(NautilusService.AspCoreHost, "Building ASP.NET Core Web Host...");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
                .Build();

            BuildWebHost(config, args).Run();

            logger.Information(NautilusService.AspCoreHost, "Closing and flushing Serilog...");
            Log.CloseAndFlush();
        }

        private static IWebHost BuildWebHost(IConfiguration config, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseKestrel()
                .UseSerilog()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
    }
}
