//--------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using Nautilus.Common.Data;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Common.Messaging;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Messaging;
using Nautilus.Service;
using NodaTime;

namespace Nautilus.Data
{
    /// <summary>
    /// Provides a data service.
    /// </summary>
    public sealed class DataService : NautilusServiceBase
    {
        private readonly DataBusAdapter dataBus;
        private readonly List<Address> managedComponents;
        private readonly IDataGateway dataGateway;
        private readonly IReadOnlyCollection<Symbol> subscribingSymbols;
        private readonly LocalTime trimTimeTicks;
        private readonly int trimWindowDaysTicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataService"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="dataGateway">The data gateway.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        public DataService(
            IComponentryContainer container,
            MessageBusAdapter messagingAdapter,
            DataBusAdapter dataBusAdapter,
            IDataGateway dataGateway,
            ServiceConfiguration config)
            : base(
                container,
                messagingAdapter,
                config.FixConfig)
        {
            this.dataBus = dataBusAdapter;
            this.dataGateway = dataGateway;
            this.managedComponents = new List<Address>
            {
                ComponentAddress.DataServer,
                ComponentAddress.DataPublisher,
                ComponentAddress.MarketDataRepository,
                ComponentAddress.TickPublisher,
                ComponentAddress.TickProvider,
                ComponentAddress.BarProvider,
                ComponentAddress.InstrumentRepository,
                ComponentAddress.InstrumentProvider,
            };

            this.subscribingSymbols = config.DataConfig.SubscribingSymbols;

            this.trimTimeTicks = config.DataConfig.TickDataTrimTime;
            this.trimWindowDaysTicks = config.DataConfig.TickDataTrimWindowDays;

            this.RegisterConnectionAddress(ComponentAddress.DataGateway);
        }

        /// <inheritdoc />
        protected override void OnServiceStart(Start start)
        {
            // Forward start message
            this.Send(start, this.managedComponents);
        }

        /// <inheritdoc />
        protected override void OnServiceStop(Stop stop)
        {
            // Forward stop message
            this.Send(stop, this.managedComponents);

            // Message bus already stopping in service base
            this.dataBus.Stop();
        }

        /// <inheritdoc />
        protected override void OnConnected()
        {
            this.dataGateway.UpdateInstrumentsSubscribeAll();

            foreach (var symbol in this.subscribingSymbols)
            {
                this.dataGateway.MarketDataSubscribe(symbol);
            }
        }
    }
}
