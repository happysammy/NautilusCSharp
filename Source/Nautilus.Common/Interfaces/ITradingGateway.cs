//--------------------------------------------------------------------------------------------------
// <copyright file="ITradingGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from a trading gateway.
    /// </summary>
    public interface ITradingGateway
    {
        /// <summary>
        /// Gets the trading gateways messaging endpoint.
        /// </summary>
        Endpoint Endpoint { get; }

        /// <summary>
        /// Sends a account inquiry request message.
        /// </summary>
        void AccountInquiry();

        /// <summary>
        /// Sends a message to submit the given order.
        /// </summary>
        /// <param name="order">The new order.</param>
        void SubmitOrder(Order order);

        /// <summary>
        /// Sends a message to submit the given atomic order.
        /// </summary>
        /// <param name="atomicOrder">The new atomic order.</param>
        void SubmitOrder(AtomicOrder atomicOrder);

        /// <summary>
        /// Sends a message to modify the given order with the given price.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(Order order, Price modifiedPrice);

        /// <summary>
        /// Submits a cancel order FIX message to the brokerage.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);
    }
}
