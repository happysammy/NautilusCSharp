//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NautilusExecutor.Logging;
    using Serilog;
    using Serilog.Debugging;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: false, reloadOnChange: true)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile("symbols.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables()
                .Build();

            var logTemplate = "{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} " +
                              "[{ThreadId:000}][{Level:u3}] " +
                              "[{SourceContext}] " +
                              "[{EventId}] " +
                              "{Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: logTemplate)
                .CreateLogger();

            SelfLog.Enable(msg => Log.Logger.Debug(msg));

            AppDomain.CurrentDomain.DomainUnload += (o, e) => Log.CloseAndFlush();

            try
            {
                CreateHostBuilder(configuration, args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs ex)
        {
            Console.WriteLine(ex.ExceptionObject.ToString());
            Console.WriteLine("Press ENTER to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }

        private static IWebHostBuilder CreateHostBuilder(IConfiguration config, string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddEventSourceLogger();
                    logging.AddSerilog(Log.Logger);
                })
                .UseSerilog()
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel();
        }
    }
}
