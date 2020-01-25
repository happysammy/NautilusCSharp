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
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTickStore"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        public InMemoryTickStore(IComponentryContainer container, IDataBusAdapter dataBusAdapter)
        : base(container, dataBusAdapter)
        {
            this.tickStore = new Dictionary<Symbol, List<Tick>>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);

            this.Subscribe<Tick>();
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
        public CommandResult Add(Tick tick)
        {
            if (this.tickStore.TryGetValue(tick.Symbol, out var tickList))
            {
                tickList.Add(tick);
            }
            else
            {
                this.tickStore[tick.Symbol] = new List<Tick> { tick };
            }

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult Add(List<Tick> ticks)
        {
            Debug.NotEmpty(ticks, nameof(ticks));

            if (ticks.Count == 0)
            {
                return CommandResult.Fail("No ticks were add");
            }

            var symbol = ticks[0].Symbol;
            if (this.tickStore.TryGetValue(symbol, out var tickList))
            {
                tickList.AddRange(ticks);
            }
            else
            {
                this.tickStore[symbol] = ticks;
            }

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public QueryResult<List<Tick>> Find(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            if (this.tickStore.TryGetValue(symbol, out var tickList))
            {
                return QueryResult<List<Tick>>.Ok(
                    tickList
                        .Where(t => t.Timestamp.IsGreaterThanOrEqualTo(fromDateTime) &&
                                    t.Timestamp.IsLessThanOrEqualTo(toDateTime))
                        .ToList());
            }

            return QueryResult<List<Tick>>.Fail($"Tick data not found for {symbol}");
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol)
        {
            return this.tickStore.TryGetValue(symbol, out var tickList)
                ? QueryResult<ZonedDateTime>.Ok(tickList.Last().Timestamp)
                : QueryResult<ZonedDateTime>.Fail($"Tick data not found for {symbol}");
        }

        /// <inheritdoc />
        public CommandResult TrimFrom(ZonedDateTime trimFrom)
        {
            foreach (var symbol in this.tickStore.Keys.ToList())
            {
                var trimmedTicks = this.tickStore[symbol]
                    .Where(t => t.Timestamp.IsGreaterThanOrEqualTo(trimFrom))
                    .ToList();

                this.tickStore[symbol] = trimmedTicks;
            }

            return CommandResult.Ok();
        }

        private void OnMessage(Tick tick)
        {
            this.Add(tick).OnFailure(result => this.Log.Error(result.Message));
        }

        private void OnMessage(TrimTickData message)
        {
            this.TrimFrom(message.TrimFrom);
        }
    }
}
