//--------------------------------------------------------------------------------------------------
// <copyright file="ITradingGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Messaging;

namespace Nautilus.Common.Interfaces
{
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
        /// Subscribes to position events.
        /// </summary>
        void SubscribeToPositionEvents();

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
