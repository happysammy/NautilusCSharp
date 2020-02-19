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
    using System.Collections.Immutable;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution;
    using Nautilus.Fix;
    using Nautilus.Logging;
    using Nautilus.Network.Configuration;
    using NodaTime;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public sealed class Startup
    {
        private readonly ExecutionService executionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="environment">The hosting environment.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                .AddJsonFile("symbols.json", optional: false, reloadOnChange: true);

            this.Configuration = builder.Build();

            var logLevel = this.Configuration.GetSection(ConfigSection.Logging)["LogLevel"].ToEnum<LogEventLevel>();
            var loggingAdapter = new SerilogLogger(logLevel);

            var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;

            // Messaging Configuration
            var messagingConfigSection = this.Configuration.GetSection(ConfigSection.Messaging);
            FileManager.CopyAll(messagingConfigSection["KeysPath"], workingDirectory);

            var messagingConfiguration = new MessagingConfiguration(
                messagingConfigSection["Version"],
                messagingConfigSection["Compression"].ToEnum<CompressionCodec>(),
                messagingConfigSection["Encryption"].ToEnum<CryptographicAlgorithm>(),
                workingDirectory);

            // FIX Configuration
            var fixConfigSection = this.Configuration.GetSection(ConfigSection.FIX44);
            var fixConfigFileTarget = Path.Combine(fixConfigSection["ConfigPath"], fixConfigSection["ConfigFile"]);
            var fixConfigFile = Path.Combine(workingDirectory, fixConfigSection["ConfigFile"]);

            // Move configuration file to working directory
            FileManager.Copy(fixConfigFileTarget, workingDirectory);

            var fixSettings = ConfigReader.LoadConfig(fixConfigFile);
            var dataDictionary = Path.Combine(fixConfigSection["ConfigPath"], fixSettings["DataDictionary"]);

            // Move data dictionary to working directory
            FileManager.Copy(dataDictionary, workingDirectory);

            var broker = new Brokerage(fixSettings["Brokerage"]);
            var accountType = fixSettings["AccountType"].ToEnum<AccountType>();
            var accountCurrency = fixSettings["AccountCurrency"].ToEnum<Currency>();
            var credentials = new FixCredentials(
                fixSettings["Account"],
                fixSettings["Username"],
                fixSettings["Password"]);
            var sendAccountTag = Convert.ToBoolean(fixSettings["SendAccountTag"]);

            var connectionJob = fixConfigSection.GetSection("ConnectJob");
            var connectDay = connectionJob["Day"].ToEnum<IsoDayOfWeek>();
            var connectHour = int.Parse(connectionJob["Hour"]);
            var connectMinute = int.Parse(connectionJob["Minute"]);
            var connectTime = (connectDay, new LocalTime(connectHour, connectMinute));

            var disconnectionJob = fixConfigSection.GetSection("DisconnectJob");
            var disconnectDay = disconnectionJob["Day"].ToEnum<IsoDayOfWeek>();
            var disconnectHour = int.Parse(disconnectionJob["Hour"]);
            var disconnectMinute = int.Parse(disconnectionJob["Minute"]);
            var disconnectTime = (disconnectDay, new LocalTime(disconnectHour, disconnectMinute));

            var fixConfiguration = new FixConfiguration(
                broker,
                accountType,
                accountCurrency,
                fixConfigFileTarget,
                credentials,
                sendAccountTag,
                connectTime,
                disconnectTime);

            var symbolMap = this.Configuration
                .GetSection("SymbolMap")
                .AsEnumerable()
                .ToImmutableDictionary();

            var execConfig = new ServiceConfiguration(
                loggingAdapter,
                this.Configuration.GetSection(ConfigSection.Network),
                messagingConfiguration,
                fixConfiguration,
                symbolMap);

            this.executionService = ExecutionServiceFactory.Create(execConfig);
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
