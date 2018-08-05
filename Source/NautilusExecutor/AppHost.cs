//--------------------------------------------------------------------------------------------------
// <copyright file="AppHost.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using Funq;
    using global::NautilusExecutor.Service;
    using ServiceStack;

    /// <summary>
    /// Provides a <see cref="ServiceStack"/> application host.
    /// </summary>
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppHost"/> class.
        /// </summary>
        public AppHost()
            : base("NautilusExecutor", typeof(NautilusExecutorService).Assembly)
        {
        }

        /// <summary>
        /// Configures the <see cref="Funq"/> dependency injection container.
        /// </summary>
        /// <param name="container">The dependency injection container.</param>
        public override void Configure(Container container)
        {
        }
    }
}
