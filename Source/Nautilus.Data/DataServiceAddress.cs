//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides data service component messaging addresses.
    /// </summary>
    [PerformanceOptimized]
    public static class DataServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="DatabaseTaskManager"/> component messaging address.
        /// </summary>
        public static Address DatabaseTaskManager { get; } = new Address(nameof(DatabaseTaskManager));

        /// <summary>
        /// Gets the <see cref="BarAggregationController"/> component messaging address.
        /// </summary>
        public static Address BarAggregationController { get; } = new Address(nameof(BarAggregationController));

        /// <summary>
        /// Gets the <see cref="BarPublisher"/> component messaging address.
        /// </summary>
        public static Address BarPublisher { get; } = new Address(nameof(BarPublisher));

        /// <summary>
        /// Gets the <see cref="TickPublisher"/> component messaging address.
        /// </summary>
        public static Address TickPublisher { get; } = new Address(nameof(TickPublisher));
    }
}
