//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nautilus.Data;
using Serilog;

namespace NautilusData
{
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
        }
    }
}
