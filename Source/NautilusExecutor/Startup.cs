//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Configuration;
    using Nautilus.Core.Extensions;
    using Nautilus.Execution;
    using Nautilus.Logging;
    using Newtonsoft.Json.Linq;
    using Serilog;
    using Serilog.Events;

#pragma warning disable CS8602
    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public sealed class Startup
    {
        private readonly ExecutionService executionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The hosting environment.</param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            this.Environment = environment;

            var configJson = JObject.Parse(File.ReadAllText("config.json"));
            var logLevel = ((string)configJson[ConfigSection.Logging]["logLevel"] !).ToEnum<LogEventLevel>() !;
            var loggingAdapter = new SerilogLogger(logLevel);
            var symbolIndex = File.ReadAllText("symbols.json");

            var config = new Configuration(
                loggingAdapter,
                configJson,
                symbolIndex);

            VersionChecker.Run(loggingAdapter, "NautilusExecutor - Algorithmic Trading Execution Service");

            this.executionService = ExecutionServiceFactory.Create(config);
            this.executionService.Start();
        }

        /// <summary>
        /// Gets the ASP.NET Core configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the ASP.NET Core hosting environment.
        /// </summary>
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// Configures the ASP.NET Core web hosting services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
                loggingBuilder.AddSerilog();
            });

            AppDomain.CurrentDomain.DomainUnload += (o, e) => Log.CloseAndFlush();
        }

        /// <summary>
        /// Configures the ASP.NET Core web request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        /// <param name="env">The hosting environment.</param>
        public void Configure(
            IApplicationBuilder app,
            IApplicationLifetime appLifetime,
            IHostingEnvironment env)
        {
            appLifetime.ApplicationStopping.Register(this.OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private void OnShutdown()
        {
            this.executionService.Stop();

            Task.Delay(2000).Wait();
        }
    }
}
