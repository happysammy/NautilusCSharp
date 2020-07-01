//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
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
using QuickFix;

namespace Nautilus.Fix.Interfaces
{
    /// <summary>
    /// The adapter for FIX message routers.
    /// </summary>
    public interface IFixMessageRouter
    {
        /// <summary>
        /// Connects the FIX session.
        /// </summary>
        /// <param name="newSession">The new FIX session.</param>
        void InitializeSession(Session newSession);

        /// <summary>
        /// Sends a CollateralInquiry FIX message.
        /// </summary>
        void CollateralInquiry();

        /// <summary>
        /// Sends a TradingSessionStatusRequest FIX message.
        /// </summary>
        void TradingSessionStatusRequest();

        /// <summary>
        /// Sends a RequestForPositions FIX message for all open positions and subscription for
        /// updates.
        /// </summary>
        void RequestForOpenPositionsSubscribe();

        /// <summary>
        /// Sends a RequestForPositions FIX message for all closed positions and subscription for
        /// updates.
        /// </summary>
        void RequestForClosedPositionsSubscribe();

        /// <summary>
        /// Sends a SecurityListRequest FIX message for the given symbol with subscription for
        /// updates.
        /// </summary>
        /// <param name="symbol">
        /// The symbol for the request.
        /// </param>
        void SecurityListRequestSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a SecurityListRequest FIX message for all broker instruments with subscription for
        /// updates.
        /// </summary>
        void SecurityListRequestSubscribeAll();

        /// <summary>
        /// Sends a MarketDataRequest FIX message for the given symbol with subscription for updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void MarketDataRequestSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a NewOrderSingle FIX message.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="positionIdBroker">The optional broker position identifier for the order.</param>
        void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker);

        /// <summary>
        /// Sends a NewOrderList FIX message.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        void NewOrderList(AtomicOrder atomicOrder);

        /// <summary>
        /// Sends an OrderCancelReplaceRequest FIX message.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedQuantity">The modified order quantity.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice);

        /// <summary>
        /// Sends an OrderCancelRequest FIX message.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void OrderCancelRequest(Order order);
    }
}
