//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides system service messaging addresses.
    /// </summary>
    [PerformanceOptimized]
    public static class ServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="Scheduler"/> messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the alpha service messaging address.
        /// </summary>
        public static Address Alpha { get; } = new Address("AlphaService");

        /// <summary>
        /// Gets the data service messaging address.
        /// </summary>
        public static Address Data { get; } = new Address("DataService");

        /// <summary>
        /// Gets the risk service messaging address.
        /// </summary>
        public static Address Risk { get; } = new Address("RiskService");

        /// <summary>
        /// Gets the portfolio service messaging address.
        /// </summary>
        public static Address Portfolio { get; } = new Address("PortfolioService");

        /// <summary>
        /// Gets the execution service messaging address.
        /// </summary>
        public static Address Execution { get; } = new Address("ExecutionService");
    }
}
