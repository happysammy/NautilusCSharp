// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepository.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis.Data.Base;
    using Nautilus.Redis.Data.Internal;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Tick"/>s with Redis.
    /// </summary>
    public sealed class RedisTickRepository : RedisTimeSeriesRepository, ITickRepository
    {
        private readonly IDataSerializer<Tick> serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTickRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The tick serializer.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisTickRepository(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Tick> serializer,
            ConnectionMultiplexer connection)
            : base(container, dataBusAdapter, connection)
        {
            this.serializer = serializer;

            this.RegisterHandler<Tick>(this.OnData);
            this.RegisterHandler<TrimTickData>(this.OnMessage);

            this.Subscribe<Tick>();
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
            this.Logger.LogInformation(LogId.Database, $"Trimming tick data to {trimToDays} rolling days. ");

            this.TrimToDays(
                KeyProvider.GetTicksPattern(),
                3,
                4,
                trimToDays);

            this.Logger.LogInformation(LogId.Database, $"Trim job complete. TicksCount={this.TicksCount()}.");
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

        private void OnData(Tick tick)
        {
            this.Add(tick);
        }

        private void OnMessage(TrimTickData message)
        {
            this.Logger.LogInformation(LogId.Database, $"Received {message}.");

            this.TrimToDays(message.RollingDays);
        }
    }
}
