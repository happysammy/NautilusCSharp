//--------------------------------------------------------------------------------------------------
// <copyright file="IFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
        /// <param name="session">The FIX session.</param>
        void ConnectSession(Session session);

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
        /// Submits an order.
        /// </summary>
        /// <param name="order">The order.</param>
        void SubmitOrder(Order order);

        /// <summary>
        /// Submits an atomic order.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        void SubmitOrder(AtomicOrder atomicOrder);

        /// <summary>
        /// Submits a modify stop-loss order.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(Order order, Price modifiedPrice);

        /// <summary>
        /// Submits a cancel order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);
    }
}
