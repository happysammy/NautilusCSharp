// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTimeSeriesRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using StackExchange.Redis;

    /// <summary>
    /// The base class for all redis time series repositories.
    /// </summary>
    public class RedisTimeSeriesRepository : Component, IRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTimeSeriesRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The clients manager.</param>
        protected RedisTimeSeriesRepository(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.RedisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.RedisDatabase = connection.GetDatabase();
        }

        /// <summary>
        /// Gets the redis server.
        /// </summary>
        protected IServer RedisServer { get; }

        /// <summary>
        /// Gets the redis database.
        /// </summary>
        protected IDatabase RedisDatabase { get; }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            this.RedisServer.Save(SaveType.BackgroundSave, CommandFlags.FireAndForget);
        }

        /// <summary>
        /// Return the sorted keys for the given bar type.
        /// </summary>
        /// <param name="pattern">The pattern for the keys scan.</param>
        /// <returns>The sorted key strings.</returns>
        [PerformanceOptimized]
        internal QueryResult<RedisKey[]> GetKeysSorted(RedisValue pattern)
        {
            var keys = this.RedisServer.Keys(pattern: pattern)
                .Select(x => x.ToString())
                .ToList();
            keys.Sort();  // TODO: Check sorting edge cases

            return keys.Count > 0
                ? QueryResult<RedisKey[]>.Ok(Array.ConvertAll(keys.ToArray(), x => (RedisKey)x))
                : QueryResult<RedisKey[]>.Fail($"No data found for {pattern}");
        }

        /// <summary>
        /// Return keys sorted by symbol for the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern for the keys scan.</param>
        /// <param name="keyIndexCode">The key index for the symbol code.</param>
        /// <param name="keyIndexVenue">The key index for the symbol venue.</param>
        /// <param name="filter">The optional filter pattern for the key scan.</param>
        /// <returns>The sorted key strings.</returns>
        [PerformanceOptimized]
        internal Dictionary<string, List<string>> GetKeysSortedBySymbol(
            RedisValue pattern,
            int keyIndexCode,
            int keyIndexVenue,
            string? filter = null)
        {
            var keysQuery = this.RedisServer.Keys(pattern: pattern);

            var keysOfSymbol = new Dictionary<string, List<string>>();
            foreach (var redisKey in keysQuery)
            {
                var key = redisKey.ToString();
                if (filter != null && !key.Contains(filter))
                {
                    // Found key not applicable
                    continue;
                }

                var splitKey = redisKey.ToString().Split(':');
                var symbolKey = splitKey[keyIndexCode] + ":" + splitKey[keyIndexVenue];

                if (!keysOfSymbol.ContainsKey(symbolKey))
                {
                    keysOfSymbol.Add(symbolKey, new List<string>());
                }

                keysOfSymbol[symbolKey].Add(key);
                keysOfSymbol[symbolKey].Sort();
            }

            return keysOfSymbol;
        }

        /// <summary>
        /// Trim the data with the given parameters.
        /// </summary>
        /// <param name="pattern">The pattern to scan the keys on.</param>
        /// <param name="keyIndexCode">The index of the symbol code in the key parts.</param>
        /// <param name="keyIndexVenue">The index of the venue in the key parts.</param>
        /// <param name="trimToDays">The number of day keys to trim to.</param>
        /// <param name="filter">The optional key filter (key must contain this).</param>
        protected void TrimToDays(
            string pattern,
            int keyIndexCode,
            int keyIndexVenue,
            int trimToDays,
            string? filter = null)
        {
            var keyGroups = this.GetKeysSortedBySymbol(
                pattern,
                keyIndexCode,
                keyIndexVenue,
                filter);

            foreach (var symbolKeys in keyGroups.Values)
            {
                var keyCount = symbolKeys.Count;
                if (keyCount <= trimToDays)
                {
                    continue;
                }

                var difference = keyCount - trimToDays;
                for (var i = 0; i < difference; i++)
                {
                    this.Delete(symbolKeys[i]);
                }
            }
        }

        /// <summary>
        /// Returns the data at the given key to the given limit.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="limit">The limit of elements to return.</param>
        /// <returns>The read data bytes array.</returns>
        protected byte[][] ReadDataToBytesArray(RedisKey key, int limit)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));
            Debug.NotNegativeInt32(limit, nameof(limit));

            var data = this.ReadData(key);

            return limit > 0 & limit < data.Length
                ? data
                : data.SliceToLimitFromEnd(limit);
        }

        /// <summary>
        /// Returns the data at the given keys concatenated in key order to the given limit.
        /// </summary>
        /// <param name="keys">The keys for the data.</param>
        /// <param name="limit">The limit of elements to return.</param>
        /// <returns>The read data bytes array.</returns>
        protected byte[][] ReadDataToBytesArray(RedisKey[] keys, int limit)
        {
            Debug.NotEmpty(keys, nameof(keys));
            Debug.NotNegativeInt32(limit, nameof(limit));

            var dataList = new List<byte[]>();
            foreach (var key in keys)
            {
                Debug.NotEmptyOrWhiteSpace(key, nameof(key));

                dataList.AddRange(this.ReadData(key));
            }

            var data = dataList.ToArray();

            return limit > 0 & limit < data.Length
                ? data
                : data.SliceToLimitFromEnd(limit);
        }

        /// <summary>
        /// Returns a value indicating whether the given key exists in the database.
        /// </summary>
        /// <param name="key">The pattern key.</param>
        /// <returns>True is key exists, otherwise false.</returns>
        protected bool KeyExists(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return this.RedisDatabase.KeyExists(key);
        }

        /// <summary>
        /// Returns a value indicating whether the given key exists in the database.
        /// </summary>
        /// <param name="key">The pattern key.</param>
        /// <returns>True is key does not exist, otherwise false.</returns>
        protected bool KeyDoesNotExist(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return !this.KeyExists(key);
        }

        /// <summary>
        /// Deletes the given key from the database. Logs an error if the key does not exist.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        protected void Delete(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.KeyExists(key))
            {
                this.Log.Error($"Cannot find {key} to delete in the database");
            }

            this.RedisDatabase.KeyDelete(key);

            this.Log.Information($"Deleted {key} from the database");
        }

        /// <summary>
        /// Returns the data at the given key.
        /// </summary>
        /// <param name="key">The pattern key.</param>
        /// <returns>The read data array.</returns>
        private byte[][] ReadData(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return Array.ConvertAll(this.RedisDatabase.ListRange(key), x => (byte[])x);
        }
    }
}
