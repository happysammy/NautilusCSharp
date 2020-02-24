//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nautilus.Data;
    using Serilog;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public sealed class Startup
    {
        private readonly DataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Startup(
            IHostingEnvironment env,
            IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            this.Environment = env;
            this.Configuration = config;

            var dataServiceConfig = DataServiceConfigurator.Build(loggerFactory, config);
            this.dataService = DataServiceFactory.Create(dataServiceConfig);

            this.dataService.Start();
        }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the ASP.NET Core web hosting services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Configures the ASP.NET Core web request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(this.OnShutdown);

            if (this.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSerilogRequestLogging();
            }
        }

        private void OnShutdown()
        {
            this.dataService.Stop().Wait();

            if (this.Environment.IsDevelopment())
            {
                Console.WriteLine("Press any key to close console...");
                Console.ReadKey();
            }
        }
    }
}
