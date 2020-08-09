//--------------------------------------------------------------------------------------------------
// <copyright file="MockTickRepository.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Common.Data;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.Data.Interfaces;
using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.TestSuite.TestKit.Mocks
{
    /// <summary>
    /// Provides an in memory <see cref="Tick"/> store.
    /// </summary>
    public sealed class MockMarketDataRepository : DataBusConnected, ITickRepository, IBarRepository
    {
        private readonly Dictionary<Symbol, List<Tick>> tickDatabase;
        private readonly Dictionary<BarType, List<Bar>> barsDatabase;
        private readonly IDataSerializer<Tick> tickSerializer;
        private readonly IDataSerializer<Bar> barSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockMarketDataRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="tickSerializer">The tick serializer for the repository.</param>
        /// <param name="barSerializer">The bar serializer for the repository.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        public MockMarketDataRepository(
            IComponentryContainer container,
            IDataSerializer<Tick> tickSerializer,
            IDataSerializer<Bar> barSerializer,
            IDataBusAdapter dataBusAdapter)
        : base(container, dataBusAdapter)
        {
            this.tickDatabase = new Dictionary<Symbol, List<Tick>>();
            this.barsDatabase = new Dictionary<BarType, List<Bar>>();
            this.tickSerializer = tickSerializer;
            this.barSerializer = barSerializer;

            this.RegisterHandler<Tick>(this.Ingest);
            this.Subscribe<Tick>();
        }

        /// <inheritdoc />
        public void Ingest(Tick tick)
        {
            this.Add(tick);
        }

        public void Add(Tick tick)
        {
            if (this.tickDatabase.TryGetValue(tick.Symbol, out var tickList))
            {
                tickList.Add(tick);
            }
            else
            {
                this.tickDatabase[tick.Symbol] = new List<Tick> { tick };
            }
        }

        public void Add(List<Tick> ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var symbol = ticks[0].Symbol;
            if (this.tickDatabase.TryGetValue(symbol, out var tickList))
            {
                tickList.AddRange(ticks);
            }
            else
            {
                this.tickDatabase[symbol] = ticks;
            }
        }

        public void Add(BarType barType, Bar bar)
        {
            if (!this.barsDatabase.ContainsKey(barType))
            {
                this.barsDatabase.Add(barType, new List<Bar>());
            }

            this.barsDatabase[barType].Add(bar);
        }

        public void Add(BarType barType, Bar[] bars)
        {
            foreach (var bar in bars)
            {
                this.Add(barType, bar);
            }
        }

        public void Add(BarDataFrame barData)
        {
            foreach (var bar in barData.Bars)
            {
                this.Add(barData.BarType, bar);
            }
        }

        /// <inheritdoc />
        public bool TicksExist(Symbol symbol)
        {
            return this.tickDatabase.ContainsKey(symbol);
        }

        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            return !this.tickDatabase.ContainsKey(symbol) ? 0 : this.tickDatabase[symbol].Count;
        }

        /// <inheritdoc />
        public bool BarsExist(BarType barType)
        {
            return this.barsDatabase.ContainsKey(barType);
        }

        /// <inheritdoc />
        public long BarsCount(BarType barType)
        {
            return this.barsDatabase[barType].Count;
        }

        /// <inheritdoc />
        public Tick[] GetTicks(Symbol symbol, ZonedDateTime? fromDateTime = null, ZonedDateTime? toDateTime = null,
            long? limit = null)
        {
            return this.tickDatabase.TryGetValue(symbol, out var tickList)
                ? tickList.ToArray()
                : new Tick[]{};
        }

        /// <inheritdoc />
        public byte[][] ReadTickData(Symbol symbol, ZonedDateTime? fromDateTime = null,
            ZonedDateTime? toDateTime = null, long? limit = null)
        {
            return this.tickDatabase.TryGetValue(symbol, out var tickList)
                ? this.tickSerializer.Serialize(tickList.ToArray())
                : new byte[][]{};
        }

        public BarDataFrame GetBars(
            BarType barType,
            ZonedDateTime? fromDateTime,
            ZonedDateTime? toDateTime,
            long? limit = 0)
        {
            return new BarDataFrame(barType, this.barsDatabase[barType].ToArray());
        }

        public byte[][] ReadBarData(
            BarType barType,
            ZonedDateTime? fromDateTime,
            ZonedDateTime? toDateTime,
            long? limit = 0)
        {

            return this.barsDatabase.TryGetValue(barType, out var barList)
                ? this.barSerializer.Serialize(barList.ToArray())
                : new byte[][]{};
        }
    }
}
