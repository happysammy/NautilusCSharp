// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
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
        private readonly ConnectionMultiplexer connection;
        private readonly RedisBarClient barClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisBarRepository(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.connection = connection;
            this.barClient = new RedisBarClient(container, connection);
        }

        /// <inheritdoc />
        public int AllBarsCount()
        {
            return this.barClient.AllBarsCount();
        }

        /// <inheritdoc />
        public int BarsCount(BarType barType)
        {
            return this.barClient.BarsCount(barType);
        }

        /// <inheritdoc />
        public void Add(BarType barType, Bar bar)
        {
            this.barClient.AddBar(barType, bar);
        }

        /// <inheritdoc />
        public void Add(BarDataFrame barData)
        {
            this.barClient.AddBars(barData.BarType, barData.Bars);
        }

        /// <inheritdoc />
        public QueryResult<BarDataFrame> Find(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            var barsQuery = this.barClient.GetBars(barType, fromDateTime, toDateTime);

            return barsQuery.IsSuccess
                 ? QueryResult<BarDataFrame>.Ok(barsQuery.Value)
                 : QueryResult<BarDataFrame>.Fail(barsQuery.Message);
        }

        /// <summary>
        /// Finds and returns all bars matching the given bar type.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <returns>The query result of bars.</returns>
        public QueryResult<BarDataFrame> FindAll(BarType barType)
        {
            var barsQuery = this.barClient.GetAllBars(barType);

            return barsQuery.IsSuccess
                ? QueryResult<BarDataFrame>.Ok(barsQuery.Value)
                : QueryResult<BarDataFrame>.Fail(barsQuery.Message);
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            var barKeysQuery = this.barClient.GetAllSortedKeys(barType);

            if (barKeysQuery.IsFailure)
            {
                return QueryResult<ZonedDateTime>.Fail(barKeysQuery.Message);
            }

            var lastKey = barKeysQuery.Value.Last();
            var barsQuery = this.barClient.GetBarsByDay(lastKey);

            return barsQuery.IsSuccess
                ? QueryResult<ZonedDateTime>.Ok(barsQuery.Value.Last().Timestamp)
                : QueryResult<ZonedDateTime>.Fail(barsQuery.Message);
        }

        /// <inheritdoc />
        public void TrimToDays(BarStructure barStructure, int trimToDays)
        {
            var keys = this.barClient.GetSortedKeysBySymbolResolution(barStructure);
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
                    this.barClient.Delete(value[i]);
                }
            }
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            // Not implemented yet
        }
    }
}
