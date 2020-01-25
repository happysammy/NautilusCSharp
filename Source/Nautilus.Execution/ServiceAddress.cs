//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Messaging;

    /// <summary>
    /// Provides execution service component messaging addresses.
    /// </summary>
    public static class ServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="ExecutionService"/> component messaging address.
        /// </summary>
        public static Address ExecutionService { get; } = new Address(nameof(ExecutionService));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="TradingGateway"/> component messaging address.
        /// </summary>
        public static Address TradingGateway { get; } = new Address(nameof(TradingGateway));

        /// <summary>
        /// Gets the <see cref="CommandServer"/> component messaging address.
        /// </summary>
        public static Address CommandServer { get; } = new Address(nameof(CommandServer));

        /// <summary>
        /// Gets the <see cref="EventPublisher"/> component messaging address.
        /// </summary>
        public static Address EventPublisher { get; } = new Address(nameof(EventPublisher));

        /// <summary>
        /// Gets the <see cref="ExecutionEngine"/> component messaging address.
        /// </summary>
        public static Address ExecutionEngine { get; } = new Address(nameof(ExecutionEngine));
    }
}
