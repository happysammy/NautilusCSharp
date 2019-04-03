//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides execution service component messaging addresses.
    /// </summary>
    [PerformanceOptimized]
    internal static class ExecutionServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="MessageServer"/> component messaging address.
        /// </summary>
        internal static Address MessageServer { get; } = new Address(nameof(MessageServer));

        /// <summary>
        /// Gets the <see cref="OrderManager"/> component messaging address.
        /// </summary>
        internal static Address OrderManager { get; } = new Address(nameof(OrderManager));
    }
}
