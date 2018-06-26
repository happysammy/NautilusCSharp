//--------------------------------------------------------------------------------------------------
// <copyright file="IFixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides various clients for the system from the given inputs.
    /// </summary>
    public interface IFixClientFactory
    {
        /// <summary>
        /// Creates a new <see cref="IDataClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        /// <returns>The FIX data client.</returns>
        IDataClient DataClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickProcessor tickProcessor);

        /// <summary>
        /// Creates and returns a new <see cref="ITradeGateway"/> from the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        /// <returns>The FIX trading client.</returns>
        ITradeClient TradeClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickProcessor tickProcessor);
    }
}
