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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a gateway to the brokers trading network.
    /// </summary>
    public interface ITradingGateway
    {
        /// <summary>
        /// Gets the trading gateways messaging endpoint.
        /// </summary>
        Endpoint Endpoint { get; }

        /// <summary>
        /// Sends an account inquiry request message.
        /// </summary>
        void AccountInquiry();

        /// <summary>
        /// Sends a message to submit the given order.
        /// </summary>
        /// <param name="order">The new order.</param>
        /// <param name="positionIdBroker">The optional broker position identifier for the order.</param>
        void SubmitOrder(Order order, PositionIdBroker? positionIdBroker);

        /// <summary>
        /// Sends a message to submit the given atomic order.
        /// </summary>
        /// <param name="atomicOrder">The new atomic order.</param>
        void SubmitOrder(AtomicOrder atomicOrder);

        /// <summary>
        /// Sends a message to modify the given order with the given modified quantity and price.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedQuantity">The modified order quantity.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(Order order, Quantity modifiedQuantity, Price modifiedPrice);

        /// <summary>
        /// Submits a message to cancel the given order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);
    }
}
