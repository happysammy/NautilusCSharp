// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis.Data.Internal;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Tick"/>s with Redis.
    /// </summary>
    public class RedisTickRepository : Component, ITickRepository
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTickRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisTickRepository(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
        }

        /// <inheritdoc />
        public void Add(Tick tick)
        {
            var key = KeyProvider.GetTicksKey(tick.Symbol, new DateKey(tick.Timestamp));
            this.redisDatabase.ListRightPush(key, tick.ToString(), flags: CommandFlags.FireAndForget);
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
            var keys = this.GetKeysSorted();
            foreach (var value in keys.Values)
            {
                var keyCount = value.Count;
                if (keyCount <= trimToDays)
                {
                    continue;
                }

                var difference = keyCount - trimToDays;
                for (var i = 0; i < difference; i++)
                {
                    this.Delete(value[i]);
                }
            }
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            this.redisServer.Save(SaveType.BackgroundSave, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetTicksPattern(symbol)).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.redisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        public long TicksCount()
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetTicksPattern()).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.redisDatabase.ListLength(key)).Sum()
                : 0;
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

#pragma warning disable CS8604
            var ticks = new Tick[dataQuery.Value.Length];
            for (var i = 0; i < dataQuery.Value.Length; i++)
            {
                ticks[i] = Tick.FromString(symbol, Encoding.UTF8.GetString(dataQuery.Value[i]));
            }

            return QueryResult<Tick[]>.Ok(ticks);
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(
            Symbol symbol,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            var keys = KeyProvider.GetTicksKeys(symbol, fromDate, toDate);
            var data = new List<byte[]>();
            foreach (var key in keys)
            {
                data.AddRange(this.ReadDataToBytes(key));
            }

            var dataArray = data.ToArray();
            if (dataArray.Length == 0)
            {
                return QueryResult<byte[][]>.Fail($"Cannot find tick data for {symbol.Value} between {fromDate} to {toDate}");
            }

            var startIndex = Math.Max(0, dataArray.Length - limit);

            return QueryResult<byte[][]>.Ok(dataArray[startIndex..]);
        }

        /// <summary>
        /// Return the sorted keys for the given tick symbol.
        /// </summary>
        /// <param name="symbol">The query symbol.</param>
        /// <returns>The sorted key strings.</returns>
        public QueryResult<List<string>> GetKeysSorted(Symbol symbol)
        {
            var keys = this.redisServer.Keys(pattern: KeyProvider.GetTicksPattern(symbol))
                .Select(x => x.ToString())
                .ToList();
            keys.Sort();

            return keys.Count > 0
                ? QueryResult<List<string>>.Ok(keys)
                : QueryResult<List<string>>.Fail($"No {symbol.Value} tick data found");
        }

        /// <summary>
        /// Return keys sorted by symbol.
        /// </summary>
        /// <returns>The sorted key strings.</returns>
        [PerformanceOptimized]
        public Dictionary<string, List<string>> GetKeysSorted()
        {
            var keysQuery = this.redisServer.Keys(pattern: KeyProvider.GetTicksPattern());

            var keysBySymbol = new Dictionary<string, List<string>>();
            foreach (var key in keysQuery)
            {
                var splitKey = key.ToString().Split(':');
                var symbolKey = splitKey[3] + ":" + splitKey[4];

                if (!keysBySymbol.ContainsKey(symbolKey))
                {
                    keysBySymbol.Add(symbolKey, new List<string>());
                }

                keysBySymbol[symbolKey].Add(key);
                keysBySymbol[symbolKey].Sort();
            }

            return keysBySymbol;
        }

        [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Local", Justification = "Consistent API.")]
        private byte[][] ReadDataToBytes(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return Array.ConvertAll(this.redisDatabase.ListRange(key), x => (byte[])x);
        }

        private bool KeyExists(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return this.redisDatabase.KeyExists(key);
        }

        private bool KeyDoesNotExist(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return !this.KeyExists(key);
        }

        private void Delete(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.KeyExists(key))
            {
                this.Log.Error($"Cannot find {key} to delete in the database");
            }

            this.redisDatabase.KeyDelete(key);

            this.Log.Information($"Deleted {key} from the database");
        }
    }
}
