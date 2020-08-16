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
        private readonly Dictionary<Symbol, List<QuoteTick>> quoteTickDatabase;
        private readonly Dictionary<Symbol, List<TradeTick>> tradeTickDatabase;
        private readonly Dictionary<BarType, List<Bar>> barsDatabase;
        private readonly IDataSerializer<QuoteTick> quoteSerializer;
        private readonly IDataSerializer<TradeTick> tradeSerializer;
        private readonly IDataSerializer<Bar> barSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockMarketDataRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="quoteSerializer">The quote tick serializer for the repository.</param>
        /// <param name="tradeSerializer">The trade tick serializer for the repository.</param>
        /// <param name="barSerializer">The bar serializer for the repository.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        public MockMarketDataRepository(
            IComponentryContainer container,
            IDataSerializer<QuoteTick> quoteSerializer,
            IDataSerializer<TradeTick> tradeSerializer,
            IDataSerializer<Bar> barSerializer,
            IDataBusAdapter dataBusAdapter)
        : base(container, dataBusAdapter)
        {
            this.quoteTickDatabase = new Dictionary<Symbol, List<QuoteTick>>();
            this.tradeTickDatabase = new Dictionary<Symbol, List<TradeTick>>();
            this.barsDatabase = new Dictionary<BarType, List<Bar>>();
            this.quoteSerializer = quoteSerializer;
            this.tradeSerializer = tradeSerializer;
            this.barSerializer = barSerializer;

            this.RegisterHandler<QuoteTick>(this.Ingest);
            this.Subscribe<QuoteTick>();
        }

        /// <inheritdoc />
        public void Ingest(QuoteTick tick)
        {
            this.Add(tick);
        }

        public void Add(QuoteTick tick)
        {
            if (this.quoteTickDatabase.TryGetValue(tick.Symbol, out var tickList))
            {
                tickList.Add(tick);
            }
            else
            {
                this.quoteTickDatabase[tick.Symbol] = new List<QuoteTick> { tick };
            }
        }

        public void Add(List<QuoteTick> ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var symbol = ticks[0].Symbol;
            if (this.quoteTickDatabase.TryGetValue(symbol, out var tickList))
            {
                tickList.AddRange(ticks);
            }
            else
            {
                this.quoteTickDatabase[symbol] = ticks;
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
            return this.quoteTickDatabase.ContainsKey(symbol);
        }

        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            return !this.quoteTickDatabase.ContainsKey(symbol) ? 0 : this.quoteTickDatabase[symbol].Count;
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
        public QuoteTick[] GetTicks(Symbol symbol, ZonedDateTime? fromDateTime = null, ZonedDateTime? toDateTime = null,
            long? limit = null)
        {
            return this.quoteTickDatabase.TryGetValue(symbol, out var tickList)
                ? tickList.ToArray()
                : new QuoteTick[]{};
        }

        /// <inheritdoc />
        public byte[][] ReadTickData(Symbol symbol, ZonedDateTime? fromDateTime = null,
            ZonedDateTime? toDateTime = null, long? limit = null)
        {
            return this.quoteTickDatabase.TryGetValue(symbol, out var tickList)
                ? this.quoteSerializer.Serialize(tickList.ToArray())
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
