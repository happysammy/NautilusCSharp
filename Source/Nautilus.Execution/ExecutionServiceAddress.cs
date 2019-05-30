//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides execution service module messaging addresses.
    /// </summary>
    [PerformanceOptimized]
    public static class ExecutionServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="Core"/> module messaging address.
        /// </summary>
        public static Address Core { get; } = new Address(nameof(Core));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> module messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="FixGateway"/> module messaging address.
        /// </summary>
        public static Address FixGateway { get; } = new Address(nameof(FixGateway));

        /// <summary>
        /// Gets the <see cref="CommandServer"/> module messaging address.
        /// </summary>
        public static Address CommandServer { get; } = new Address(nameof(CommandServer));

        /// <summary>
        /// Gets the <see cref="EventServer"/> module messaging address.
        /// </summary>
        public static Address EventServer { get; } = new Address(nameof(EventServer));

        /// <summary>
        /// Gets the <see cref="OrderManager"/> module messaging address.
        /// </summary>
        public static Address OrderManager { get; } = new Address(nameof(OrderManager));
    }
}
