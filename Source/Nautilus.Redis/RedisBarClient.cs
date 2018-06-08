// -------------------------------------------------------------------------------------------------
// <copyright file="RedisMarketDataClient.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Keys;
    using Nautilus.Database.Wranglers;
    using NodaTime;
    using ServiceStack.Redis;

    /// <summary>
    /// A client for accessing bar data from <see cref="Redis"/> with a
    /// <see cref="RedisNativeClient"/>. This client is not thread-safe and therefor should be
    /// encapsulated in a thread-safe environment for sequential operations on the
    /// <see cref="Redis"/> database.
    /// </summary>
    [Immutable]
    public class RedisBarClient
    {
        private readonly RedisNativeClient redisClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarClient"/> class.
        /// </summary>
        /// <param name="redisEndpoint">The <see cref="Redis"/> end point.</param>
        /// <param name="compressor">The data compressor.</param>
        public RedisBarClient(RedisEndpoint redisEndpoint, IDataCompressor compressor)
        {
            Validate.NotNull(redisEndpoint, nameof(redisEndpoint));
            Validate.NotNull(compressor, nameof(compressor));

            this.redisClient = new RedisNativeClient(redisEndpoint);
        }

        /// <summary>
        /// Warning: Flushes ALL data from the <see cref="Redis"/> database.
        /// </summary>
        /// <param name="areYouSure">The are you sure string.
        /// </param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        public CommandResult FlushAll(string areYouSure)
        {
            Validate.NotNull(areYouSure, nameof(areYouSure));

            if (areYouSure == "YES")
            {
                this.redisClient.FlushAll();

                return CommandResult.Ok();
            }

            return CommandResult.Fail("Database Flush not confirmed");
        }

        /// <summary>
        /// Returns a result indicating whether a <see cref="Redis"/> Key exists for the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="key">The <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool KeyExists(string key)
        {
            Validate.NotNull(key, nameof(key));

            return this.redisClient.Exists(key) == 1;
        }

        /// <summary>
        /// Returns a result indicating whether a <see cref="Redis"/> Key exists for the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="key">The <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool KeyExists(BarDataKey key)
        {
            Validate.NotDefault(key, nameof(key));

            return this.KeyExists(key.ToString());
        }

        /// <summary>
        /// Returns a count of all bars held within the <see cref="Redis"/> namespace 'market_date'.
        /// </summary>
        /// <returns>A <see cref="long"/>.</returns>
        public long AllKeysCount()
        {
            return this.redisClient.Keys(KeyProvider.BarsNamespaceWildcard).Length;
        }

        /// <summary>
        /// Returns a count of all bars held within <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <returns>A <see cref="long"/>.</returns>
        public long KeysCount(SymbolBarSpec barSpec)
        {
            Validate.NotNull(barSpec, nameof(barSpec));

            return this.redisClient.Keys(KeyProvider.GetBarsWildcardString(barSpec)).Length;
        }

        /// <summary>
        /// Returns a count of all bars held within <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <returns>A <see cref="long"/>.</returns>
        public long BarsCount(SymbolBarSpec barSpec)
        {
            Validate.NotNull(barSpec, nameof(barSpec));

            var allKeys = this.redisClient.Keys(KeyProvider.GetBarsWildcardString(barSpec));

            if (allKeys.Length == 0)
            {
                return 0;
            }

            return allKeys
                .Select(key => Encoding.Default.GetString(key))
                .ToList()
                .Select(this.GetBarsByDay)
                .Sum(k => k.Value.Count);
        }

        /// <summary>
        /// Returns a count of all bar strings held within the <see cref="Redis"/> namespace 'MarketData'.
        /// </summary>
        /// <returns>A <see cref="long"/>.</returns>
        public long AllBarsCount()
        {
            var allBarSpecKeys = this.redisClient.Keys(KeyProvider.BarsNamespaceWildcard);

            if (allBarSpecKeys.Length == 0)
            {
                return 0;
            }

            return allBarSpecKeys
                .Select(key => Encoding.Default.GetString(key))
                .ToList()
                .Select(this.GetBarsByDay)
                .Sum(k => k.Value.Count);
        }

        /// <summary>
        /// Returns a list of all market data keys based on the given bar specification.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <returns>A query result of <see cref="IReadOnlyList{T}"/> strings.</returns>
        public QueryResult<List<string>> GetAllSortedKeys(SymbolBarSpec barSpec)
        {
            Validate.NotNull(barSpec, nameof(barSpec));

            if (this.KeysCount(barSpec) == 0)
            {
                return QueryResult<List<string>>.Fail($"No market data found for {barSpec}");
            }

            var allKeysBytes = this.redisClient.Keys(KeyProvider.GetBarsWildcardString(barSpec));

            var keysCollection = allKeysBytes
                .Select(key => Encoding.Default.GetString(key))
                .ToList();

            keysCollection.Sort();

            return QueryResult<List<string>>.Ok(keysCollection);
        }

        /// <summary>
        /// Adds the given bars to the <see cref="Redis"/> Lists associated with their <see cref="BarDataKey"/>(s).
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <param name="bars">The bars to add.</param>
        /// <returns>A command result.</returns>
        [PerformanceOptimized]
        public CommandResult AddBars(SymbolBarSpec barSpec, Bar[] bars)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.EqualTo(1, nameof(barSpec.BarSpecification.Period), barSpec.BarSpecification.Period);
            Validate.CollectionNotNullOrEmpty(bars, nameof(bars));

            var barsIndex = BarWrangler.OrganizeBarsByDay(bars);
            var barsAddedCounter = 0;

            foreach (var barsToAddDictionary in barsIndex)
            {
                var key = new BarDataKey(barSpec, barsToAddDictionary.Key);
                var keyString = key.ToString();

                if (!this.KeyExists(key))
                {
                    foreach (var bar in barsToAddDictionary.Value)
                    {
                        this.redisClient.RPush(keyString, bar.ToUtf8Bytes());
                        barsAddedCounter++;
                    }

                    continue;
                }

                // The key should exist in Redis because it was just checked by KeyExists().
                var persistedBars = this.GetBarsByDay(keyString).Value;

                foreach (var bar in barsToAddDictionary.Value)
                {
                    if (bar.Timestamp.IsGreaterThan(persistedBars.Last().Timestamp))
                    {
                        this.redisClient.RPush(keyString, bar.ToUtf8Bytes());
                        barsAddedCounter++;
                    }
                }
            }

            return CommandResult.Ok(
                $"Added {barsAddedCounter} bars to {barSpec} (TotalCount={this.BarsCount(barSpec)})");
        }

        /// <summary>
        /// Returns all bars from <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barSpec">The specification of bars to get.</param>
        /// <returns>A read only collection of <see cref="Bar"/>(s).</returns>
        [PerformanceOptimized]
        public QueryResult<BarDataFrame> GetAllBars(SymbolBarSpec barSpec)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.EqualTo(1, nameof(barSpec.BarSpecification.Period), barSpec.BarSpecification.Period);

            var barKeysQuery = this.GetAllSortedKeys(barSpec);

            if (barKeysQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(barKeysQuery.Message);
            }

            var barsArray = barKeysQuery
                .Value
                .SelectMany(key => BarWrangler.ParseBars(this.redisClient.LRange(key, 0, -1)))
                .ToArray();

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barSpec, barsArray));
        }

        /// <summary>
        /// Returns all bars from <see cref="Redis"/> of the given <see cref="BarSpecification"/> within the given
        /// range of <see cref="ZonedDateTime"/> (inclusive).
        /// </summary>
        /// <param name="barSpec">The specification of bars to get.</param>
        /// <param name="fromDateTime">The from date time range.</param>
        /// <param name="toDateTime">The to date time range.</param>
        /// <returns>A read only collection of <see cref="Bar"/>(s).</returns>
        public QueryResult<BarDataFrame> GetBars(
            SymbolBarSpec barSpec,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(fromDateTime, nameof(fromDateTime));
            Validate.NotDefault(toDateTime, nameof(toDateTime));

            if (this.KeysCount(barSpec) == 0)
            {
                return QueryResult<BarDataFrame>.Fail($"No market data found for {barSpec}");
            }

            var barKeysQuery = KeyProvider.GetBarsKeyStrings(barSpec, fromDateTime, toDateTime);

            var barsArray = barKeysQuery
                .Select(key => this.redisClient.LRange(key, 0, -1))
                .SelectMany(BarWrangler.ParseBars)
                .Where(bar => bar.Timestamp.Compare(fromDateTime) >= 0 && bar.Timestamp.Compare(toDateTime) <= 0)
                .ToArray();

            if (barsArray.Length == 0)
            {
                return QueryResult<BarDataFrame>.Fail(
                    $"No market data found for {barSpec} in time range from " +
                    $"{fromDateTime.ToIsoString()} to " +
                    $"{toDateTime.ToIsoString()}");
            }

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barSpec, barsArray));
        }

        /// <summary>
        /// Returns a query result if success containing the requested bar, or failure containing
        /// a message.
        /// </summary>
        /// <param name="barSpec">The requested bars specification.</param>
        /// <param name="timestamp">The requested bars timestamp.</param>
        /// <returns>A query result of <see cref="Bar"/>.</returns>
        [PerformanceOptimized]
        public QueryResult<Bar> GetBar(SymbolBarSpec barSpec, ZonedDateTime timestamp)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(timestamp, nameof(timestamp));

            var key = new BarDataKey(barSpec, new DateKey(timestamp));
            var persistedBarsQuery = this.GetBarsByDay(key.ToString());

            if (persistedBarsQuery.IsFailure)
            {
                return QueryResult<Bar>.Fail(persistedBarsQuery.Message);
            }

            for (var i = 0; i < persistedBarsQuery.Value.Count; i++)
            {
                if (persistedBarsQuery.Value[i].Timestamp.Equals(timestamp))
                {
                    return QueryResult<Bar>.Ok(persistedBarsQuery.Value[i]);
                }
            }

            return QueryResult<Bar>.Fail(
                $"No market data found for {barSpec} at {timestamp.ToIsoString()}");
        }

        public QueryResult<List<Bar>> GetBarsByDay(string key)
        {
            Validate.NotNull(key, nameof(key));

            if (!this.KeyExists(key))
            {
                return QueryResult<List<Bar>>.Fail(
                    $"No market data found for {key}");
            }

            var barBytes = this.redisClient.LRange(key, 0, -1);

            return QueryResult<List<Bar>>.Ok(BarWrangler.ParseBars(barBytes));
        }
    }
}
