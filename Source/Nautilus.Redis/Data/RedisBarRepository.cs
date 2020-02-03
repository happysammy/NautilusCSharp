// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepository.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Bar"/>s with Redis.
    /// </summary>
    public sealed class RedisBarRepository : Component, IBarRepository
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisBarRepository(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
        }

        /// <summary>
        /// Organizes the given bars array into a dictionary of bar lists indexed by a date key.
        /// </summary>
        /// <param name="bars">The bars array.</param>
        /// <returns>The organized dictionary.</returns>
        [PerformanceOptimized]
        public static Dictionary<DateKey, List<Bar>> OrganizeBarsByDay(Bar[] bars)
        {
            var barsDictionary = new Dictionary<DateKey, List<Bar>>();
            for (var i = 0; i < bars.Length; i++)
            {
                var dateKey = new DateKey(bars[i].Timestamp);
                if (!barsDictionary.ContainsKey(dateKey))
                {
                    barsDictionary.Add(dateKey, new List<Bar>());
                }

                barsDictionary[dateKey].Add(bars[i]);
            }

            return barsDictionary;
        }

        /// <inheritdoc />
        public void Add(BarDataFrame barData)
        {
            this.Add(barData.BarType, barData.Bars);
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public void Add(BarType barType, Bar bar)
        {
            var key = KeyProvider.GetBarsKey(barType, new DateKey(bar.Timestamp));
            this.redisDatabase.ListRightPush(key, bar.ToString());

            this.Log.Debug($"Added 1 bar to {barType}");
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public void Add(BarType barType, Bar[] bars)
        {
            Debug.NotEmpty(bars, nameof(bars));

            var barsAddedCounter = 0;
            var barsIndex = OrganizeBarsByDay(bars);
            foreach (var dateKey in barsIndex.Keys)
            {
                var key = KeyProvider.GetBarsKey(barType, dateKey);
                if (this.KeyDoesNotExist(key))
                {
                    foreach (var bar in barsIndex[dateKey])
                    {
                        this.redisDatabase.ListRightPush(key, bar.ToString());
                        barsAddedCounter++;
                    }

                    continue;
                }

                // The key should exist in Redis because it was just checked by KeyExists()
                var persistedBars = this.GetBarsByDay(key).Value;
                foreach (var bar in barsIndex[dateKey])
                {
                    if (bar.Timestamp.IsGreaterThan(persistedBars[^1].Timestamp))
                    {
                        this.redisDatabase.ListRightPush(key, bar.ToString());
                        barsAddedCounter++;
                    }
                }
            }

            this.Log.Debug(
                $"Added {barsAddedCounter} bars to {barType}, TotalCount={this.BarsCount(barType)}");
        }

        /// <inheritdoc />
        public void TrimToDays(BarStructure barStructure, int trimToDays)
        {
            var keys = this.GetKeysSorted(barStructure);
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

        /// <summary>
        /// Returns a count of all bars held within <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <returns>A <see cref="long"/>.</returns>
        public long BarsCount(BarType barType)
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetBarsWildcardKey(barType)).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.redisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <summary>
        /// Returns a count of all bar strings held within the <see cref="Redis"/> namespace 'MarketData'.
        /// </summary>
        /// <returns>A <see cref="long"/>.</returns>
        public long BarsCount()
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetBarsWildcardKey()).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.redisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            var keysQuery = this.GetKeysSorted(barType);
            if (keysQuery.IsFailure)
            {
                return QueryResult<ZonedDateTime>.Fail(keysQuery.Message);
            }

            var lastKey = keysQuery.Value.Last();
            var barsQuery = this.GetBarsByDay(lastKey);

            return barsQuery.IsSuccess
                ? QueryResult<ZonedDateTime>.Ok(barsQuery.Value.Last().Timestamp)
                : QueryResult<ZonedDateTime>.Fail(barsQuery.Message);
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<BarDataFrame> GetBars(BarType barType, int limit = 0)
        {
            var keysQuery = this.GetKeysSorted(barType);
            if (keysQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(keysQuery.Message);
            }

            var bars = keysQuery
                .Value
                .SelectMany(key => this.redisDatabase.ListRange(key))
                .Select(value => Bar.FromString(value))
                .ToArray();

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, bars));
        }

        /// <summary>
        /// Returns all bars from <see cref="Redis"/> of the given <see cref="BarType"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="barType">The type of bars to get.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="limit">The optional limit for a count of bars.</param>
        /// <returns>The result of the query.</returns>
        public QueryResult<BarDataFrame> GetBars(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            var dataQuery = this.GetBarData(barType, fromDate, toDate, limit);
            if (dataQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(dataQuery.Message);
            }

            #pragma warning disable CS8604
            var bars = dataQuery.Value
                    .Select(value => Bar.FromString(value.ToString()))
                    .ToArray();

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, bars));
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<byte[][]> GetBarData(BarType barType, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            if (this.KeyDoesNotExist(KeyProvider.GetBarsWildcardKey(barType)))
            {
                return QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType}");
            }

            var keys = KeyProvider.GetBarsKeys(barType, fromDate, toDate);
            var data = new List<byte[]>();
            foreach (var key in keys)
            {
                data.AddRange(this.ReadDataToBytes(key));
            }

            if (data.Count == 0)
            {
                return QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType} between {fromDate} to {toDate}");
            }

            if (limit > 0)
            {
                var segment = new byte[Math.Min(data.Count, limit)][];
                for (var i = segment.Length - 1; i >= 0; i--)
                {
                    segment[i] = data[i];
                }

                return QueryResult<byte[][]>.Ok(segment);
            }

            return QueryResult<byte[][]>.Ok(data.ToArray());
        }

        /// <summary>
        /// Return the sorted keys for the given bar type.
        /// </summary>
        /// <param name="barType">The query bar type.</param>
        /// <returns>The sorted key strings.</returns>
        public QueryResult<string[]> GetKeysSorted(BarType barType)
        {
            var keysQuery = this.redisServer.Keys(pattern: KeyProvider.GetBarsWildcardKey(barType))
                .Select(key => key.ToString())
                .ToList();
            keysQuery.Sort();

            return keysQuery.Count > 0
                ? QueryResult<string[]>.Ok(keysQuery.ToArray())
                : QueryResult<string[]>.Fail($"No {barType} bar data found");
        }

        /// <summary>
        /// Return keys sorted by symbol for the given bar structure.
        /// </summary>
        /// <param name="barStructure">The query bar structure.</param>
        /// <returns>The sorted key strings.</returns>
        [PerformanceOptimized]
        public Dictionary<string, List<string>> GetKeysSorted(BarStructure barStructure)
        {
            var keysQuery = this.redisServer.Keys(pattern: KeyProvider.GetBarsWildcardKey());
            var barStructureString = barStructure.ToString();

            var keysOfResolution = new Dictionary<string, List<string>>();
            foreach (var redisKey in keysQuery)
            {
                var key = redisKey.ToString();
                if (!key.Contains(barStructureString))
                {
                    // Found resolution not applicable
                    continue;
                }

                var splitKey = key.Split(':');
                var symbolKey = splitKey[3] + ":" + splitKey[4];

                if (!keysOfResolution.ContainsKey(symbolKey))
                {
                    keysOfResolution.Add(symbolKey, new List<string>());
                }

                keysOfResolution[symbolKey].Add(key);
                keysOfResolution[symbolKey].Sort();
            }

            return keysOfResolution;
        }

        private QueryResult<Bar[]> GetBarsByDay(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return QueryResult<Bar[]>.Ok(Array.ConvertAll(this.ReadDataToStrings(key), Bar.FromString));
        }

        private string[] ReadDataToStrings(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return Array.ConvertAll(this.redisDatabase.ListRange(key), x => (string)x);
        }

        [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Local", Justification = "Consistent API.")]
        private byte[][] ReadDataToBytes(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return Array.ConvertAll(this.redisDatabase.ListRange(key), x => (byte[])x);
        }

        private bool KeyExists(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return this.redisDatabase.KeyExists(key);
        }

        private bool KeyDoesNotExist(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return !this.redisDatabase.KeyExists(key);
        }

        private void Delete(string key)
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
