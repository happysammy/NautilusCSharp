//--------------------------------------------------------------------------------------------------
// <copyright file="IDataGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from a data gateway.
    /// </summary>
    public interface IDataGateway
    {
        /// <summary>
        /// Gets the gateways brokerage name.
        /// </summary>
        Brokerage Broker { get; }

        /// <summary>
        /// Gets a value indicating whether the gateway is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Sends an update and subscribe request message for the instrument of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the instrument to update.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Send an update and subscribe request message for all instruments.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Sends a market data subscribe request message for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void MarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a market data subscribe request message for all symbols.
        /// </summary>
        void MarketDataSubscribeAll();

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the tick publisher and bar aggregation
        /// controller.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        void OnTick(
            Symbol symbol,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp);

        /// <summary>
        /// Updates the given instruments in the instrument repository.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        void OnInstrumentsUpdate(
            IEnumerable<Instrument> instruments,
            string responseId,
            string result);

        /// <summary>
        /// Event handler for receiving FIX business messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnBusinessMessage(string message);
    }
}
