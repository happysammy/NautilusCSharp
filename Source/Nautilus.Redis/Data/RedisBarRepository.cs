// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepository.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Commands;
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
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The bar serializer.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisBarRepository(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Bar> serializer,
            ConnectionMultiplexer connection)
            : base(container, dataBusAdapter, connection)
        {
            this.serializer = serializer;

            this.RegisterHandler<BarData>(this.OnData);
            this.RegisterHandler<BarDataFrame>(this.OnData);
            this.RegisterHandler<TrimBarData>(this.OnMessage);

            this.Subscribe<BarData>();
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

            this.Logger.LogDebug(LogId.Database, $"Added 1 bar to {barType}");
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

            this.Logger.LogDebug(
                LogId.Database,
                $"Added {barsAddedCounter} bars to {barType}, BarsCount={this.BarsCount(barType)}");
        }

        /// <inheritdoc />
        public void TrimToDays(BarStructure barStructure, int trimToDays)
        {
            this.Logger.LogInformation($"Trimming {barStructure} bar data to {trimToDays} rolling days. ");

            this.TrimToDays(
                KeyProvider.GetBarsPattern().ToString(),
                3,
                4,
                trimToDays,
                barStructure.ToString());

            this.Logger.LogInformation(LogId.Database, $"Trim job complete. BarsCount={this.BarsCount()}.");
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
            Debug.NotNegativeInt32(limit, nameof(limit));

            var dataQuery = this.GetKeysSorted(KeyProvider.GetBarsPattern(barType));
            if (dataQuery.IsFailure)
            {
                return QueryResult<byte[][]>.Fail(dataQuery.Message);
            }

            var data = this.ReadDataToBytesArray(dataQuery.Value, limit);

            return data.Length > 0
                ? QueryResult<byte[][]>.Ok(data)
                : QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType}");
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
            Debug.NotNegativeInt32(limit, nameof(limit));

            var keys = KeyProvider.GetBarsKeys(barType, fromDate, toDate);
            var data = this.ReadDataToBytesArray(keys, limit);

            return data.Length > 0
                ? QueryResult<byte[][]>.Ok(data)
                : QueryResult<byte[][]>.Fail($"Cannot find bar data for {barType}");
        }

        private QueryResult<Bar> GetLastBar(RedisKey key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            var barQuery = this.RedisDatabase.ListRange(key);
            return barQuery.Length > 0
                ? QueryResult<Bar>.Ok(Bar.FromString(barQuery[^1].ToString()))
                : QueryResult<Bar>.Fail("Cannot find bar data.");
        }

        private void OnData(BarData data)
        {
            this.Add(data.BarType, data.Bar);

            this.Logger.LogDebug(LogId.Database, $"Received {data}");
        }

        private void OnData(BarDataFrame data)
        {
            this.Add(data);
        }

        private void OnMessage(TrimBarData message)
        {
            this.Logger.LogInformation(LogId.Database, $"Received {message}.");

            foreach (var structure in message.BarStructures)
            {
                this.TrimToDays(structure, message.RollingDays);
            }
        }
    }
}
