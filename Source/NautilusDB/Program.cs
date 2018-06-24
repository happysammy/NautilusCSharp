//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using Nautilus.Serilog;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore;
    using Nautilus.Common.Enums;
    using global::Serilog;

    /// <summary>
    /// The main entry point for the program.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            SerilogLogFactory.Create();
            var logger = new SerilogLogger();
            logger.Information(ServiceContext.AspCoreHost, "Building ASP.NET Core Web Host...");

            BuildWebHost(args).Run();

            logger.Information(ServiceContext.AspCoreHost, "Closing and flushing Serilog...");
            Log.CloseAndFlush();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
