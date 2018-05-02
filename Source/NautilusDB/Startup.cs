//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Database.Compression;
    using Newtonsoft.Json.Linq;
    using ServiceStack;
    using Nautilus.Database.Core;
    using Nautilus.Database.Core.Build;
    using Nautilus.Database.Core.Configuration;
    using Nautilus.Database.Core.Temp;
    using Nautilus.DataProviders.Dukascopy;
    using Nautilus.Serilog;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public class Startup
    {
        private NautilusDatabase nautilusDB;
        private MarketDataProviderConfig dukasConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class. Starts the ASP.NET Core
        /// application.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The hosting environment.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Validate.NotNull(configuration, nameof(configuration));

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");

            this.Configuration = builder.Build();
            this.Environment = env;
        }

        /// <summary>
        /// Gets the ASP.NET Core configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the ASP.NEt Core hosting environment.
        /// </summary>
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// Configures the ASP.NET Core web hosting services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public void ConfigureServices(IServiceCollection services)
        {
            Validate.NotNull(services, nameof(services));

            var config = JObject.Parse(File.ReadAllText("config.json"));

            Licensing.RegisterLicense((string)config[ConfigSection.ServiceStack]["licenseKey"]);

            var configCsvPath = string.Empty;
            var initialFromDateSpecified = (bool)config[ConfigSection.Dukascopy]["initialFromDateSpecified"];
            var initialFromDateString = string.Empty;
            var collectionOffsetMinutes = (int)config[ConfigSection.Dukascopy]["collectionSchedule"]["collectionOffsetMinutes"];

            if (initialFromDateSpecified)
            {
                initialFromDateString = (string)config[ConfigSection.Dukascopy]["initialFromDate"];
            }

            if (this.Environment.IsDevelopment())
            {
                configCsvPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "TestData\\" + "historicConfig.csv";

                this.dukasConfig = new MarketDataProviderConfig(
                    (bool)config[ConfigSection.Dukascopy]["run"],
                    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\")) + "TestData\\",
                    new[] { "AUDUSD", "GBPUSD" },
                    new[] { "Minute" },
                    (string)config[ConfigSection.Dukascopy]["timestampParsePattern"],
                    (int)config[ConfigSection.Dukascopy]["volumeMultiple"],
                    (bool)config[ConfigSection.Dukascopy]["checkBarDataIntegrity"]);
            }

            if (this.Environment.IsProduction())
            {
                var currencyPairs = (JArray)config[ConfigSection.Dukascopy]["currencyPairs"];
                var barResolutions = (JArray)config[ConfigSection.Dukascopy]["barResolutions"];
                configCsvPath = (string)config[ConfigSection.Dukascopy]["configCsvPath"];

                this.dukasConfig = new MarketDataProviderConfig(
                    (bool)config[ConfigSection.Dukascopy]["run"],
                    (string)config[ConfigSection.Dukascopy]["csvDataDirectory"],
                    currencyPairs.Select(cp => (string)cp).ToArray(),
                    barResolutions.Select(br => (string)br).ToArray(),
                    (string)config[ConfigSection.Dukascopy]["timestampParsePattern"],
                    (int)config[ConfigSection.Dukascopy]["volumeMultiple"],
                    (bool)config[ConfigSection.Dukascopy]["checkBarDataIntegrity"]);
            }

            var compressor = DataCompressorFactory.Create(
                (bool)config[ConfigSection.Database]["compression"],
                (string)config[ConfigSection.Database]["compressionCodec"]);

            this.nautilusDB = NautilusDatabaseFactory.Create(
                new SerilogLogger("Logs"),
                (JObject)config[ConfigSection.Dukascopy]["collectionSchedule"],
                new MockMarketDataRepository(),
                new MockEconomicEventRepository(),
                new DukascopyBarDataProvider(
                    this.dukasConfig,
                    initialFromDateString,
                    collectionOffsetMinutes));

            Task.Run(() => this.nautilusDB.Start());
        }

        /// <summary>
        /// Configures the ASP.NET Core web request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        /// <param name="env">The hosting environment.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public void Configure(
            IApplicationBuilder app,
            IApplicationLifetime appLifetime,
            IHostingEnvironment env)
        {
            Validate.NotNull(app, nameof(app));
            Validate.NotNull(appLifetime, nameof(appLifetime));
            Validate.NotNull(env, nameof(env));

            appLifetime.ApplicationStopping.Register(this.OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
                                    {
                                        AppSettings = new NetCoreAppSettings(this.Configuration)
                                    });
        }

        private void OnShutdown()
        {
            this.nautilusDB.Shutdown();
        }
    }
}
