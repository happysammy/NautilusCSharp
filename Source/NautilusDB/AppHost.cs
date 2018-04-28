//--------------------------------------------------------------
// <copyright file="AppHost.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using Funq;
using NautilusDB.Service;
using ServiceStack;
using ServiceStack.Redis;

namespace NautilusDB
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppHost"/> class.
        /// </summary>
        public AppHost()
            : base("NautilusDB", typeof(MarketDataService).Assembly)
        {
        }

        /// <summary>
        /// Configures the <see cref="Funq"/> dependency injection container.
        /// </summary>
        /// <param name="container">The Funq container.</param>
        public override void Configure(Container container)
        {
            container.Register<IRedisClientsManager>(c =>
                new RedisManagerPool((string)this.AppSettings.Get("REDIS_HOST", "localhost")));
        }
    }
}
