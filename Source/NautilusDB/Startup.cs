//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Compression;
    using Nautilus.Core.Validation;
    using Nautilus.Database;
    using Nautilus.Database.Build;
    using Nautilus.Database.Configuration;
    using Newtonsoft.Json.Linq;
    using ServiceStack;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using Serilog.Events;
    using ServiceStack.Redis;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        // ReSharper disable once InconsistentNaming
        private Database nautilusDB;

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
        // ReSharper disable once MemberCanBePrivate.Global
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the ASP.NEt Core hosting environment.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
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
            RedisServiceStack.ConfigureServiceStack();

            if (this.Environment.IsDevelopment())
            {

            }

            if (this.Environment.IsProduction())
            {

            }

            var logLevelString = (string)config[ConfigSection.Database]["logLevel"];
            var logLevel = logLevelString.ToEnum<LogEventLevel>();

            var isCompression = (bool)config[ConfigSection.Database]["compression"];
            var compressionCodec = (string)config[ConfigSection.Database]["compressionCodec"];
            var compressor = CompressorFactory.Create(isCompression, compressionCodec);

            var username = (string)config[ConfigSection.Fxcm]["username"];;
            var password = (string)config[ConfigSection.Fxcm]["password"];;
            var accountNumber = (string)config[ConfigSection.Fxcm]["accountNumber"];;

            var currencyPairs = (JArray)config[ConfigSection.Fxcm]["currencyPairs"];
            var barResolutions = (JArray)config[ConfigSection.Fxcm]["barResolutions"];

            var clientManager = new BasicRedisClientManager(
                new[] { RedisConstants.LocalHost },
                new[] { RedisConstants.LocalHost });

            var loggingAdapter = new SerilogLogger(logLevel);

            var fixClientFactory = new FxcmFixClientFactory(
                username,
                password,
                accountNumber);

            var publisherFactory = new RedisChannelPublisherFactory(clientManager);

            var barRepository = new RedisBarRepository(
                clientManager,
                compressor);

            var instrumentRepository = new RedisInstrumentRepository(clientManager);

            this.nautilusDB = DatabaseFactory.Create(
                loggingAdapter,
                fixClientFactory,
                publisherFactory,
                barRepository,
                instrumentRepository);

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
