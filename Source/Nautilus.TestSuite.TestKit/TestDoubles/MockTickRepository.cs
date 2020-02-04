//--------------------------------------------------------------------------------------------------
// <copyright file="MockTickRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
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

    /// <summary>
    /// Provides an in memory <see cref="Tick"/> store.
    /// </summary>
    public sealed class MockTickRepository : DataBusConnected, ITickRepository
    {
        private readonly Dictionary<Symbol, List<Tick>> database;
        private readonly IDataSerializer<Tick> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockTickRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The serializer for the repository.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        public MockTickRepository(
            IComponentryContainer container,
            IDataSerializer<Tick> serializer,
            IDataBusAdapter dataBusAdapter)
        : base(container, dataBusAdapter)
        {
            this.database = new Dictionary<Symbol, List<Tick>>();
            this.serializer = serializer;

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);

            this.Subscribe<Tick>();
        }

        /// <inheritdoc />
        public void Add(Tick tick)
        {
            if (this.database.TryGetValue(tick.Symbol, out var tickList))
            {
                this.database[tick.Symbol].Add(tick);
            }
            else
            {
                this.database[tick.Symbol] = new List<Tick> { tick };
            }
        }

        /// <inheritdoc />
        public void Add(List<Tick> ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            var symbol = ticks[0].Symbol;
            if (this.database.TryGetValue(symbol, out var tickList))
            {
                tickList.AddRange(ticks);
            }
            else
            {
                this.database[symbol] = ticks;
            }
        }

        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            return !this.database.ContainsKey(symbol) ? 0 : this.database[symbol].Count;
        }

        /// <inheritdoc />
        public long TicksCount()
        {
            return this.database.Select(symbol => symbol.Value).Count();
        }

        /// <inheritdoc/>
        public QueryResult<Tick[]> GetTicks(Symbol symbol, int limit = 0)
        {
            return QueryResult<Tick[]>.Ok(this.database[symbol].ToArray());
        }

        /// <inheritdoc />
        public QueryResult<Tick[]> GetTicks(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            return QueryResult<Tick[]>.Ok(this.database[symbol].ToArray());
        }

        /// <inheritdoc/>
        public QueryResult<byte[][]> GetTickData(Symbol symbol, int limit = 0)
        {
            return this.database.TryGetValue(symbol, out var tickList)
                ? QueryResult<byte[][]>.Ok(this.serializer.Serialize(tickList.ToArray()))
                : QueryResult<byte[][]>.Fail($"Cannot find any {symbol} tick data");
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            return this.database.TryGetValue(symbol, out var tickList)
                ? QueryResult<byte[][]>.Ok(this.serializer.Serialize(tickList.ToArray()))
                : QueryResult<byte[][]>.Fail($"Cannot find any {symbol} tick data");
        }

        /// <inheritdoc />
        public void TrimToDays(int days)
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
            this.TrimToDays(message.RollingDays);
        }
    }
}
