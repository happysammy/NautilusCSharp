//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nautilus.Common.Configuration;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Newtonsoft.Json.Linq;
    using Serilog.Events;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public class Startup
    {
        private NautilusExecutor? executionSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The hosting environment.</param>
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");

            this.Configuration = builder.Build();
            this.Environment = environment;
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
            var config = JObject.Parse(File.ReadAllText("config.json"));

            var host = this.Environment.IsDevelopment()
                ? NetworkAddress.LocalHost()
                : new NetworkAddress((string)config[ConfigSection.Execution]["host"]);

            var commandsPort = new NetworkPort((ushort)config[ConfigSection.Execution]["commandsPort"]);
            var eventsPort = new NetworkPort((ushort)config[ConfigSection.Execution]["eventsPort"]);

            var commandsPerSecond = (int)config[ConfigSection.Execution]["commandsPerSecond"];
            var newOrdersPerSecond = (int)config[ConfigSection.Execution]["newOrdersPerSecond"];

            var logLevel = this.Environment.IsDevelopment()
                ? LogEventLevel.Debug
                : ((string)config[ConfigSection.Logging]["logLevel"]).ToEnum<LogEventLevel>();

            var configFile = (string)config[ConfigSection.Fix44]["config"];
            var assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
            var configPath = Path.GetFullPath(Path.Combine(assemblyDirectory, configFile));

            var fixSettings = ConfigReader.LoadConfig(configPath);
            var broker = fixSettings["Brokerage"].ToEnum<Brokerage>();
            var credentials = new FixCredentials(
                account: fixSettings["Account"],
                username: fixSettings["Username"],
                password: fixSettings["Password"]);

            var fixConfig = new FixConfiguration(
                broker,
                configPath,
                credentials,
                fixSettings["InstrumentData"],
                Convert.ToBoolean(fixSettings["SendAccountTag"]),
                Convert.ToBoolean(fixSettings["UpdateInstruments"]));

            this.executionSystem = NautilusExecutorFactory.Create(
                logLevel,
                fixConfig,
                host,
                commandsPort,
                eventsPort,
                commandsPerSecond,
                newOrdersPerSecond);

            this.executionSystem?.Start();
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
            this.executionSystem?.Stop();
        }
    }
}
