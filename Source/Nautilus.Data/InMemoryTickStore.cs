//--------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTickStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an in memory <see cref="Tick"/> store.
    /// </summary>
    public sealed class InMemoryTickStore : Component, ITickRepository
    {
        private readonly Dictionary<Symbol, List<Tick>> tickStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTickStore"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public InMemoryTickStore(IComponentryContainer container)
        : base(container, State.Running)
        {
            this.tickStore = new Dictionary<Symbol, List<Tick>>();

            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<TrimTickData>(this.OnMessage);
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
            if (!this.tickStore.ContainsKey(tick.Symbol))
            {
                this.tickStore[tick.Symbol] = new List<Tick>();
            }

            this.tickStore[tick.Symbol].Add(tick);

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult Add(IEnumerable<Tick> ticks)
        {
            foreach (var tick in ticks)
            {
                this.Add(tick);
            }

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public QueryResult<List<Tick>> Find(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            if (!this.tickStore.ContainsKey(symbol))
            {
                return QueryResult<List<Tick>>.Fail($"No tick data for {symbol}");
            }

            var ticks = this.tickStore[symbol]
                .Where(t => t.Timestamp.IsGreaterThanOrEqualTo(fromDateTime)
                            && t.Timestamp.IsLessThanOrEqualTo(toDateTime))
                .ToList();

            return QueryResult<List<Tick>>.Ok(ticks);
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol)
        {
            return !this.tickStore.ContainsKey(symbol)
                ? QueryResult<ZonedDateTime>.Fail($"No tick data for {symbol}")
                : QueryResult<ZonedDateTime>.Ok(this.tickStore[symbol].Last().Timestamp);
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
            this.Add(tick)
                .OnSuccess(result => this.Log.Information(result.Message))
                .OnFailure(result => this.Log.Error(result.Message));
        }

        private void OnMessage(TrimTickData message)
        {
            this.TrimFrom(message.TrimFrom);
        }
    }
}
