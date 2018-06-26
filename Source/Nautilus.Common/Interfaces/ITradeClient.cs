//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides an interface for a concrete trade client implementation.
    /// </summary>
    public interface ITradeClient : IDataClient
    {
        /// <summary>
        /// Submits an entry order with a stop-loss and profit target to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryLimitStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits an entry order with a stop-loss to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits a request to modify the stop-loss of an existing order.
        /// </summary>
        /// <param name="stoplossModification">The stop-loss modification.</param>
        void ModifyStoplossOrder(KeyValuePair<Order, Price> stoplossModification);

        /// <summary>
        /// Submits a request to cancel the given order.
        /// </summary>
        /// <param name="order">The order.</param>
        void CancelOrder(Order order);

        /// <summary>
        /// Submits a request to close the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        void ClosePosition(Position position);
    }
}
