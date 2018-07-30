//--------------------------------------------------------------------------------------------------
// <copyright file="IFixClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The adapter for FIX data feed client.
    /// </summary>
    public interface IFixClient
    {
        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        Broker Broker { get; }

        /// <summary>
        /// Gets a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

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
        void InitializeGateway(IExecutionGateway gateway);

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        IReadOnlyCollection<Symbol> GetAllSymbols();

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
        void SubmitOrder(IOrder order);

        /// <summary>
        /// Submits an atomic order
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        void SubmitOrder(IAtomicOrder atomicOrder);

        /// <summary>
        /// Submits a request to modify an order to a new price.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(IOrder order, Price modifiedPrice);

        /// <summary>
        /// Submits a request to cancel an order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(IOrder order);

        /// <summary>
        /// Submits a request to close a position.
        /// </summary>
        /// <param name="position">The position to close.</param>
        void ClosePosition(IPosition position);
    }
}
