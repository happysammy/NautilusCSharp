//--------------------------------------------------------------------------------------------------
// <copyright file="IFixClient.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides an adapter for a FIX client.
    /// </summary>
    public interface IFixClient
    {
        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        AccountId AccountId { get; }

        /// <summary>
        /// Gets the FIX brokerage.
        /// </summary>
        Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the FIX account number.
        /// </summary>
        AccountNumber AccountNumber { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX client is connected to a session.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX client is disconnected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsDisconnected { get; }

        /// <summary>
        /// Connects to a FIX session.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from a FIX session.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Initializes the FIX data gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        void InitializeGateway(IDataGateway gateway);

        /// <summary>
        /// Initializes the FIX trading gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        void InitializeGateway(ITradingGateway gateway);

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void SecurityListRequestSubscribe(Symbol symbol);

        /// <summary>
        /// Send an update and subscribe request message for all instruments.
        /// </summary>
        void SecurityListRequestSubscribeAll();

        /// <summary>
        /// Sends an update and subscribe request message for the instrument of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the instrument to update.</param>
        void MarketDataRequestSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a market data subscribe request message for all symbols.
        /// </summary>
        void MarketDataRequestSubscribeAll();

        /// <summary>
        /// Requests a collateral inquiry.
        /// </summary>
        void CollateralInquiry();

        /// <summary>
        /// Requests all positions and subscribes to position reports.
        /// </summary>
        void RequestForAllPositionsSubscribe();

        /// <summary>
        /// Requests the trading session status.
        /// </summary>
        void TradingSessionStatusRequest();

        /// <summary>
        /// Submits an order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="positionIdBroker">The optional broker position identifier for the order.</param>
        void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker);

        /// <summary>
        /// Submits an atomic order.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        void NewOrderList(AtomicOrder atomicOrder);

        /// <summary>
        /// Submits a request to modify an order to a new price.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedQuantity">The modified order quantity.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice);

        /// <summary>
        /// Submits a request to cancel an order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void OrderCancelRequest(Order order);
    }
}
