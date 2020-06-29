//--------------------------------------------------------------------------------------------------
// <copyright file="IDataGateway.cs" company="Nautech Systems Pty Ltd">
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
    using System.Collections.Generic;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a gateway to the data providers network.
    /// </summary>
    public interface IDataGateway
    {
        /// <summary>
        /// Gets the data gateways messaging endpoint.
        /// </summary>
        Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the data gateways brokerage name.
        /// </summary>
        Brokerage Brokerage { get; }

        /// <summary>
        /// Gets a value indicating whether the gateway is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Sends a request to receive the instrument for the given symbol and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol for the instrument.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a request to receive all instruments and subscribe to updates.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Sends a request to subscribe to market data for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol for the market data.</param>
        void MarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Sends a request to subscribe to market data for all symbols.
        /// </summary>
        void MarketDataSubscribeAll();

        /// <summary>
        /// Handles received ticks.
        /// </summary>
        /// <param name="tick">The tick.</param>
        void OnData(Tick tick);

        /// <summary>
        /// Handles the collection of received instruments.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        void OnData(IEnumerable<Instrument> instruments);

        /// <summary>
        /// Handles received general messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnMessage(string message);
    }
}
