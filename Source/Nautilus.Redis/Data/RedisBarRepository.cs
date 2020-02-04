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
    using System.Linq;
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
    using Nautilus.Redis.Data.Base;
    using Nautilus.Redis.Data.Internal;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Bar"/>s with Redis.
    /// </summary>
    public sealed class RedisBarRepository : RedisTimeSeriesRepository, IBarRepository
    {
        private readonly IDataSerializer<Bar> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="serializer">The bar serializer.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisBarRepository(
            IComponentryContainer container,
            IDataSerializer<Bar> serializer,
            ConnectionMultiplexer connection)
            : base(container, connection)
        {
            this.serializer = serializer;
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
            this.RedisDatabase.ListRightPush(key, bar.ToString());

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
                        this.RedisDatabase.ListRightPush(key, bar.ToString());
                        barsAddedCounter++;
                    }

                    continue;
                }

                // The key should exist in Redis because it was just checked by KeyExists()
                var lastBar = this.GetLastBar(key).Value;
                foreach (var bar in barsIndex[dateKey])
                {
                    if (bar.Timestamp.IsGreaterThan(lastBar.Timestamp))
                    {
                        this.RedisDatabase.ListRightPush(key, bar.ToString());
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
            this.TrimToDays(
                KeyProvider.GetBarsPattern().ToString(),
                3,
                4,
                trimToDays,
                barStructure.ToString());
        }

        /// <inheritdoc />
        public long BarsCount(BarType barType)
        {
            var allKeys = this.RedisServer.Keys(pattern: KeyProvider.GetBarsPattern(barType)).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.RedisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        public long BarsCount()
        {
            var allKeys = this.RedisServer.Keys(pattern: KeyProvider.GetBarsPattern()).ToArray();
            return allKeys.Length > 0
                ? allKeys.Select(key => this.RedisDatabase.ListLength(key)).Sum()
                : 0;
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<BarDataFrame> GetBars(BarType barType, int limit = 0)
        {
            var dataQuery = this.GetBarData(barType, limit);
            if (dataQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(dataQuery.Message);
            }

            var bars = this.serializer.Deserialize(dataQuery.Value);

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
        public QueryResult<BarDataFrame> GetBars(
            BarType barType,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            var dataQuery = this.GetBarData(barType, fromDate, toDate, limit);
            if (dataQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(dataQuery.Message);
            }

            var bars = this.serializer.Deserialize(dataQuery.Value);

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, bars));
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<byte[][]> GetBarData(
            BarType barType,
            int limit = 0)
        {
            var keysQuery = this.GetKeysSorted(KeyProvider.GetBarsPattern(barType));
            if (keysQuery.IsFailure)
            {
                return QueryResult<byte[][]>.Fail(keysQuery.Message);
            }

            var data = new List<byte[]>();
            foreach (var key in keysQuery.Value)
            {
                data.AddRange(this.ReadDataToBytesArray(key, limit));
            }

            var dataArray = data.ToArray();
            if (dataArray.Length == 0)
            {
                return QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType}");
            }

            var startIndex = Math.Max(0, dataArray.Length - limit);

            return QueryResult<byte[][]>.Ok(dataArray[startIndex..]);
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public QueryResult<byte[][]> GetBarData(
            BarType barType,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0)
        {
            Debug.True(fromDate.CompareTo(toDate) <= 0, "fromDate.CompareTo(toDate) <= 0");

            var keys = KeyProvider.GetBarsKeys(barType, fromDate, toDate);
            var data = new List<byte[]>();
            foreach (var key in keys)
            {
                data.AddRange(this.ReadDataToBytesArray(key, limit));
            }

            var dataArray = data.ToArray();
            if (dataArray.Length == 0)
            {
                return QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType} between {fromDate} to {toDate}");
            }

            var startIndex = Math.Max(0, dataArray.Length - limit);

            return QueryResult<byte[][]>.Ok(dataArray[startIndex..]);
        }

        private QueryResult<Bar> GetLastBar(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            var barQuery = this.RedisDatabase.ListRange(key);
            return barQuery.Length > 0
                ? QueryResult<Bar>.Ok(Bar.FromString(barQuery[^1].ToString()))
                : QueryResult<Bar>.Fail("Cannot find bar data.");
        }
    }
}
