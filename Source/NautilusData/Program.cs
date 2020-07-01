//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;

namespace NautilusData
{
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
                .Enrich.WithThreadId()
                .WriteTo.RollingFile(
                    "Logs/Nautilus-Log-{Date}.txt",
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    outputTemplate: logTemplate)
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
                Environment.Exit(1);
            }
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs ex)
        {
            Log.Fatal("Unhandled exception from background thread", ex, sender);
            Log.CloseAndFlush();
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
