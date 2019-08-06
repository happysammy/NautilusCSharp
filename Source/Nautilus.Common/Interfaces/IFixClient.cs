//--------------------------------------------------------------------------------------------------
// <copyright file="IFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides an adapter for a FIX client.
    /// </summary>
    public interface IFixClient
    {
        /// <summary>
        /// Gets the name of the FIX brokerage.
        /// </summary>
        Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        AccountId AccountId { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX client is connected to a session.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

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
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Send an update and subscribe request message for all instruments.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Sends an update and subscribe request message for the instrument of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the instrument to update.</param>
        void RequestMarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a market data subscribe request message for all symbols.
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
