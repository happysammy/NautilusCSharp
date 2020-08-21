//--------------------------------------------------------------------------------------------------
// <copyright file="BitMEXDataGateway.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messaging;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.BitMEX
{
    /// <summary>
    /// Provides a data gateway for the BitMEX exchange.
    /// </summary>
    // ReSharper disable once InconsistentNaming (correct name)
    public class BitMEXDataGateway : MessageBusConnected, IDataGateway
    {
        /// <inheritdoc />
        public BitMEXDataGateway(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter)
            : base(container, messagingAdapter)
        {
            this.Brokerage = new Brokerage("BitMEX");
        }

        /// <summary>
        /// Gets the name of the brokerage.
        /// </summary>
        public Brokerage Brokerage { get; }

        /// <summary>
        /// Returns a result indicating whether the data gateway is connected.
        /// </summary>
        public bool IsConnected => false;

        /// <inheritdoc />
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void UpdateInstrumentsSubscribeAll()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void MarketDataSubscribe(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void OnData(QuoteTick tick)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void OnData(TradeTick tick)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void OnData(IEnumerable<Instrument> instruments)
        {
            throw new System.NotImplementedException();
        }
    }
}
