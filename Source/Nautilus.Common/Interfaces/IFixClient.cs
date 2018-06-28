//--------------------------------------------------------------------------------------------------
// <copyright file="IDataFeedClient.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
        /// Connects to the FIX session.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects from the FIX session.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Returns a value indicating whether the FIX session is connected.
        /// </summary>
        /// <returns>A <see cref="bool"/>.</returns>
        bool IsConnected { get; }

        /// <summary>
        /// Initializes the FIX gateway.
        /// </summary>
        /// <param name="gateway">The FIX gateway.</param>
        void InitializeGateway(IFixGateway gateway);

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
        /// Submits an entry order with a stop-loss and profit target to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryLimitStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits an entry order with a stop-loss to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits a request to modify the stop-loss of an existing order.
        /// </summary>
        /// <param name="stoplossModification">The stop-loss modification.</param>
        void ModifyStoplossOrder(KeyValuePair<Order, Price> stoplossModification);

        /// <summary>
        /// Submits a request to cancel the given order.
        /// </summary>
        /// <param name="order">The order.</param>
        void CancelOrder(Order order);

        /// <summary>
        /// Submits a request to close the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        void ClosePosition(Position position);
    }
}
