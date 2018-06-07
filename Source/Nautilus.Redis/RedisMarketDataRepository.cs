// -------------------------------------------------------------------------------------------------
// <copyright file="RedisMarketDataRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System;
    using System.Linq;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using ServiceStack.Redis;

    public sealed class RedisMarketDataRepository : IMarketDataRepository
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly RedisMarketDataClient marketDataClient;
        private readonly TimeSpan operationsExpiry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisMarketDataRepository"/> class.
        /// </summary>
        /// <param name="clientsManager">The clients manager.</param>
        /// <param name="endpoint">The <see cref="Redis"/> endpoint.</param>
        /// <param name="operationsExpiry">The operations expiry.</param>
        /// <param name="compressor">The data compressor.</param>
        public RedisMarketDataRepository(
            IRedisClientsManager clientsManager,
            RedisEndpoint endpoint,
            Duration operationsExpiry,
            IDataCompressor compressor)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));
            Validate.NotNull(endpoint, nameof(endpoint));
            Validate.NotDefault(operationsExpiry, nameof(operationsExpiry));

            this.clientsManager = clientsManager;
            this.operationsExpiry = operationsExpiry.ToTimeSpan();
            this.marketDataClient = new RedisMarketDataClient(endpoint, compressor);
        }

        /// <summary>
        /// Warning: Flushes ALL data from the <see cref="Redis"/> database.
        /// </summary>
        /// <param name="areYouSure">The are you sure string.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CommandResult FlushAll(string areYouSure)
        {
            Validate.NotNull(areYouSure, nameof(areYouSure));

            if (areYouSure == "YES")
            {
                this.marketDataClient.FlushAll("YES");

                return CommandResult.Ok();
            }

            return CommandResult.Fail("Database Flush not confirmed");
        }

        /// <summary>
        /// Returns the total count of bars persisted within the database.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public long AllBarsCount()
        {
            return this.marketDataClient.AllBarsCount();
        }

        /// <summary>
        /// Returns the count of bars persisted within the database with the given
        /// <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="symbolBarSpec">The bar specification.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public long BarsCount(SymbolBarSpec symbolBarSpec)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            return this.marketDataClient.BarsCount(symbolBarSpec);
        }

        /// <summary>
        /// Adds the given bar(s) to the repository.
        /// </summary>
        /// <param name="marketData">The market data.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public CommandResult Add(MarketDataFrame marketData)
        {
            Validate.NotNull(marketData, nameof(marketData));

            return this.marketDataClient.AddBars(marketData.SymbolBarSpec, marketData.Bars);
        }

        /// <summary>
        /// Returns a market data frame populated with the given bar info specification.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>A <see cref="QueryResult{MarketDataFrame}"/>.</returns>
        public QueryResult<MarketDataFrame> Find(
            SymbolBarSpec symbolBarSpec,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            var barsQuery = this.marketDataClient.GetBars(symbolBarSpec, fromDateTime, toDateTime);

            return barsQuery.IsSuccess
                 ? QueryResult<MarketDataFrame>.Ok(barsQuery.Value)
                 : QueryResult<MarketDataFrame>.Fail(barsQuery.Message);
        }

        public QueryResult<MarketDataFrame> FindAll(SymbolBarSpec barSpec)
        {
            var barsQuery = this.marketDataClient.GetAllBars(barSpec);

            return barsQuery.IsSuccess
                ? QueryResult<MarketDataFrame>.Ok(barsQuery.Value)
                : QueryResult<MarketDataFrame>.Fail(barsQuery.Message);
        }

        /// <summary>
        /// Returns a query result containing the <see cref="ZonedDateTime"/> timestamp of the last
        /// bar within <see cref="Redis"/> for the given <see cref="BarSpecification"/> (if successful).
        /// </summary>
        /// <param name="barSpec">The requested bar specification.</param>
        /// <returns>A <see cref="QueryResult{T}"/> containing the <see cref="Bar"/>.</returns>
        public QueryResult<ZonedDateTime> LastBarTimestamp(SymbolBarSpec symbolBarSpec)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            var barKeysQuery = this.marketDataClient.GetAllSortedKeys(symbolBarSpec);

            if (barKeysQuery.IsFailure)
            {
                return QueryResult<ZonedDateTime>.Fail(barKeysQuery.Message);
            }

            var lastKey = barKeysQuery.Value.Last();

            var barsQuery = this.marketDataClient.GetBarsByDay(lastKey);

            if (barsQuery.IsFailure)
            {
                return QueryResult<ZonedDateTime>.Fail(barsQuery.Message);
            }

            return QueryResult<ZonedDateTime>.Ok(barsQuery.Value.Last().Timestamp);
        }
    }
}
