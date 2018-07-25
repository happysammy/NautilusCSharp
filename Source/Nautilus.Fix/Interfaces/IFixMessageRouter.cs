//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
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
        /// <param name="session">The FIX session.</param>
        void ConnectSession(Session session);

        /// <summary>
        /// Connects the FIX session md.
        /// </summary>
        /// <param name="sessionMd">The FIX session md.
        /// </param>
        void ConnectSessionMd(Session sessionMd);

        /// <summary>
        /// Sends a new collateral inquiry FIX message.
        /// </summary>
        void CollateralInquiry();

        /// <summary>
        /// Send a new trading session status FIX message.
        /// </summary>
        void TradingSessionStatus();

        /// <summary>
        /// The request all positions.
        /// </summary>
        void RequestAllPositions();

        /// <summary>
        /// Updates the instrument from the given symbol via a security status request FIX message.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Updates all instruments via a security status request FIX message.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void MarketDataRequestSubscribe(Symbol symbol);

        /// <summary>
        /// Subscribes to market data for all symbols.
        /// </summary>
        void MarketDataRequestSubscribeAll();

        /// <summary>
        /// Submits an entry limit stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        void SubmitEntryLimitStopOrder(AtomicOrder elsOrder);

        /// <summary>
        /// Submits an entry stop order.
        /// </summary>
        /// <param name="elsOrder">The ELS order.</param>
        void SubmitEntryStopOrder(AtomicOrder elsOrder);

        /// <summary>
        /// Submits a modify stop-loss order.
        /// </summary>
        /// <param name="orderModification">The order modification.</param>
        void ModifyOrder(KeyValuePair<Order, Price> orderModification);

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);

        /// <summary>
        /// Submits a request to close a position.
        /// </summary>
        /// <param name="position">The position to close.
        /// </param>
        void ClosePosition(Position position);
    }
}
