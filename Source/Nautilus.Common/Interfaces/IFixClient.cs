//--------------------------------------------------------------------------------------------------
// <copyright file="IFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// The adapter for FIX data feed client.
    /// </summary>
    public interface IFixClient
    {
        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        Brokerage Broker { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

        /// <summary>
        /// Registers the given receiver for brokerage connection events.
        /// </summary>
        /// <param name="receiver">The event receiver.</param>
        void RegisterConnectionEventReceiver(IEndpoint receiver);

        /// <summary>
        /// Connects to the FIX session.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        void InitializeGateway(IFixGateway gateway);

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        IEnumerable<Symbol> GetAllSymbols();

        /// <summary>
        /// Subscribes to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void RequestMarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Subscribes to market data for all symbols.
        /// </summary>
        void RequestMarketDataSubscribeAll();

        /// <summary>
        /// Requests a collateral report.
        /// </summary>
        void CollateralInquiry();

        /// <summary>
        /// Requests the trading session status.
        /// </summary>
        void TradingSessionStatus();

        /// <summary>
        /// Requests all positions.
        /// </summary>
        void RequestAllPositions();

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

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
        /// Submits a request to modify an order to a new price.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(Order order, Price modifiedPrice);

        /// <summary>
        /// Submits a request to cancel an order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);
    }
}
