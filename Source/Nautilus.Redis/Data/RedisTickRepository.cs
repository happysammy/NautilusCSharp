// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis.Data.Base;
    using Nautilus.Redis.Data.Internal;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Tick"/>s with Redis.
    /// </summary>
    public class RedisTickRepository : RedisTimeSeriesRepository, ITickRepository
    {
        private readonly IDataSerializer<Tick> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTickRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The tick serializer.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisTickRepository(
            IComponentryContainer container,
            IDataSerializer<Tick> serializer,
            ConnectionMultiplexer connection)
            : base(container, connection)
        {
            this.serializer = serializer;
        }

        /// <inheritdoc />
        public void Add(Tick tick)
        {
            var key = KeyProvider.GetTicksKey(tick.Symbol, new DateKey(tick.Timestamp));
            this.RedisDatabase.ListRightPush(key, tick.ToString(), flags: CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void Add(List<Tick> ticks)
        {
            ticks.Sort();  // Ensure monotonically increasing

            foreach (var tick in ticks)
            {
                this.Add(tick);
            }
        }

        /// <inheritdoc />
        public void TrimToDays(int trimToDays)
        {
            this.TrimToDays(
                KeyProvider.GetTicksPattern(),
                3,
                4,
                trimToDays);
        }

        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            var allKeys = this.RedisServer.Keys(pattern: KeyProvider.GetTicksPattern(symbol)).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.RedisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        public long TicksCount()
        {
            var allKeys = this.RedisServer.Keys(pattern: KeyProvider.GetTicksPattern()).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.RedisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<Tick[]> GetTicks(Symbol symbol, int limit = 0)
        {
            var dataQuery = this.GetTickData(symbol, limit);
            if (dataQuery.IsFailure)
            {
                return QueryResult<Tick[]>.Fail(dataQuery.Message);
            }

            var ticks = this.serializer.Deserialize(dataQuery.Value, symbol);

            return QueryResult<Tick[]>.Ok(ticks);
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<Tick[]> GetTicks(
            Symbol symbol,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            var dataQuery = this.GetTickData(symbol, fromDate, toDate, limit);
            if (dataQuery.IsFailure)
            {
                return QueryResult<Tick[]>.Fail(dataQuery.Message);
            }

            var ticks = this.serializer.Deserialize(dataQuery.Value, symbol);

            return QueryResult<Tick[]>.Ok(ticks);
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(
            Symbol symbol,
            int limit = 0)
        {
            Debug.NotNegativeInt32(limit, nameof(limit));

            var keys = this.GetKeysSorted(KeyProvider.GetTicksPattern(symbol));
            var data = this.ReadDataToBytesArray(keys.Value, limit);

            return data.Length > 0
                ? QueryResult<byte[][]>.Ok(data)
                : QueryResult<byte[][]>.Fail($"Cannot find tick data for {symbol.Value}");
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(
            Symbol symbol,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");
            Debug.NotNegativeInt32(limit, nameof(limit));

            var keys = KeyProvider.GetTicksKeys(symbol, fromDate, toDate);
            var data = this.ReadDataToBytesArray(keys, limit);

            return data.Length > 0
                ? QueryResult<byte[][]>.Ok(data)
                : QueryResult<byte[][]>.Fail($"Cannot find tick data for {symbol.Value}");
        }
    }
}
