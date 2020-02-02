//--------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTickStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an in memory <see cref="Tick"/> store.
    /// </summary>
    public sealed class InMemoryTickStore : DataBusConnected, ITickRepository
    {
        private readonly Dictionary<Symbol, List<Tick>> tickStore;
        private readonly IDataSerializer<Tick> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTickStore"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The serializer for the repository.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        public InMemoryTickStore(
            IComponentryContainer container,
            IDataSerializer<Tick> serializer,
            IDataBusAdapter dataBusAdapter)
        : base(container, dataBusAdapter)
        {
            this.tickStore = new Dictionary<Symbol, List<Tick>>();
            this.serializer = serializer;

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);

            this.Subscribe<Tick>();
        }

        /// <inheritdoc />
        public void Add(Tick tick)
        {
            if (this.tickStore.TryGetValue(tick.Symbol, out var tickList))
            {
                this.tickStore[tick.Symbol].Add(tick);
            }
            else
            {
                this.tickStore[tick.Symbol] = new List<Tick> { tick };
            }
        }

        /// <inheritdoc />
        public void Add(List<Tick> ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var symbol = ticks[0].Symbol;
            if (this.tickStore.TryGetValue(symbol, out var tickList))
            {
                tickList.AddRange(ticks);
            }
            else
            {
                this.tickStore[symbol] = ticks;
            }
        }

        /// <inheritdoc />
        public int TicksCount(Symbol symbol)
        {
            return !this.tickStore.ContainsKey(symbol) ? 0 : this.tickStore[symbol].Count;
        }

        /// <inheritdoc />
        public int AllTicksCount()
        {
            return this.tickStore.Select(symbol => symbol.Value).Count();
        }

        /// <inheritdoc />
        public QueryResult<Tick[]> GetTicks(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            return QueryResult<Tick[]>.Ok(this.tickStore[symbol].ToArray());
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            return this.tickStore.TryGetValue(symbol, out var tickList)
                ? QueryResult<byte[][]>.Ok(this.serializer.Serialize(tickList.ToArray()))
                : QueryResult<byte[][]>.Fail($"Cannot find any {symbol} tick data");
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol)
        {
            return this.tickStore.TryGetValue(symbol, out var tickList)
                ? QueryResult<ZonedDateTime>.Ok(tickList.Last().Timestamp)
                : QueryResult<ZonedDateTime>.Fail($"Cannot find any {symbol} tick data");
        }

        /// <inheritdoc />
        public void TrimFrom(ZonedDateTime trimFrom)
        {
            // Not implemented for mock
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            // Not implemented for mock
        }

        private void OnMessage(Tick tick)
        {
            this.Add(tick);
        }

        private void OnMessage(TrimTickData message)
        {
            this.TrimFrom(message.TrimFrom);
        }
    }
}
