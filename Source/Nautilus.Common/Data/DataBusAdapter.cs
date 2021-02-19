// -------------------------------------------------------------------------------------------------
// <copyright file="DataBusAdapter.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Annotations;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Messaging;
using Nautilus.Messaging.Interfaces;
using NodaTime;

namespace Nautilus.Common.Data
{
    /// <summary>
    /// Provides a means for components to subscribe to and unsubscribe from various data types.
    /// </summary>
    [Immutable]
    public sealed class DataBusAdapter : IDataBusAdapter
    {
        private readonly Dictionary<Type, IEndpoint> endpoints;
        private readonly DataBus<QuoteTick> quoteBus;
        private readonly DataBus<TradeTick> tradeBus;
        private readonly DataBus<BarData> barBus;
        private readonly DataBus<Instrument> instrumentBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBusAdapter"/> class.
        /// </summary>
        /// <param name="quoteBus">The quote tick data bus.</param>
        /// <param name="tradeBus">The trade tick data bus.</param>
        /// <param name="barBus">The bar data bus.</param>
        /// <param name="instrumentBus">The instrument data bus.</param>
        public DataBusAdapter(
            DataBus<QuoteTick> quoteBus,
            DataBus<TradeTick> tradeBus,
            DataBus<BarData> barBus,
            DataBus<Instrument> instrumentBus)
        {
            this.endpoints = new Dictionary<Type, IEndpoint>
            {
                { quoteBus.BusType, quoteBus.Endpoint },
                { tradeBus.BusType, tradeBus.Endpoint },
                { barBus.BusType, barBus.Endpoint },
                { instrumentBus.BusType, instrumentBus.Endpoint },
            };

            this.quoteBus = quoteBus;
            this.tradeBus = tradeBus;
            this.barBus = barBus;
            this.instrumentBus = instrumentBus;
        }

        /// <inheritdoc />
        public void SendToBus(QuoteTick data)
        {
            this.quoteBus.PostData(data);
        }

        /// <inheritdoc />
        public void SendToBus(TradeTick data)
        {
            this.tradeBus.PostData(data);
        }

        /// <inheritdoc />
        public void SendToBus(BarData data)
        {
            this.barBus.PostData(data);
        }

        /// <inheritdoc />
        public void SendToBus(Instrument data)
        {
            this.instrumentBus.PostData(data);
        }

        /// <inheritdoc />
        public void Subscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
        {
            var subscription = typeof(T);
            var message = new Subscribe<Type>(
                subscription,
                subscriber,
                id,
                timestamp);

            this.Send(subscription, message);
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
        {
            var subscription = typeof(T);
            var message = new Unsubscribe<Type>(
                subscription,
                subscriber,
                id,
                timestamp);

            this.Send(subscription, message);
        }

        /// <summary>
        /// Starts the data bus.
        /// </summary>
        public void Start()
        {
            Task.WaitAll(this.quoteBus.Start(), this.barBus.Start(), this.instrumentBus.Start());
        }

        /// <summary>
        /// Stops the data bus.
        /// </summary>
        public void Stop()
        {
            Task.WaitAll(this.quoteBus.Stop(), this.barBus.Stop(), this.instrumentBus.Stop());
        }

        private void Send(Type subscription, Message message)
        {
            if (this.endpoints.TryGetValue(subscription, out var endpoint))
            {
                endpoint.Send(message);
            }
            else
            {
                throw new InvalidOperationException($"Invalid data type for subscription ({subscription}).");
            }
        }
    }
}
