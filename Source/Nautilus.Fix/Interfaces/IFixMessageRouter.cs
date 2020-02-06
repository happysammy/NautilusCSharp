//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using QuickFix;

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
        /// Sends a MarketDataRequest FIX message for all broker instruments with subscription for
        /// updates.
        /// </summary>
        void MarketDataRequestSubscribeAll();

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
