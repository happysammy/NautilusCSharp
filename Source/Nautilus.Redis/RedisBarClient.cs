// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarClient.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Keys;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a thread-safe client for accessing bar data from <see cref="Redis"/>.
    /// </summary>
    public class RedisBarClient
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarClient"/> class.
        /// </summary>
        /// <param name="connection">The redis connection multiplexer.</param>
        public RedisBarClient(ConnectionMultiplexer connection)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
        }

        /// <summary>
        /// Returns a result indicating whether a given key exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool KeyExists(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            return this.redisDatabase.KeyExists(key);
        }

        /// <summary>
        /// Returns a result indicating whether a <see cref="Redis"/> Key exists for the given
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="key">The <see cref="BarDataKey"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public bool KeyExists(BarDataKey key)
        {
            Debug.NotDefault(key, nameof(key));

            return this.KeyExists(key.ToString());
        }

        /// <summary>
        /// Returns a count of all bars held within the <see cref="Redis"/> namespace 'market_date'.
        /// </summary>
        /// <returns>A <see cref="long"/>.</returns>
        public long AllKeysCount()
        {
            return this.redisServer.Keys(pattern: KeyProvider.BarsNamespaceWildcard).Count();
        }

        /// <summary>
        /// Returns a count of all bars held within <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <returns>A <see cref="long"/>.</returns>
        public long KeysCount(BarType barType)
        {
            return this.redisServer.Keys(pattern: KeyProvider.GetBarTypeWildcardString(barType)).Count();
        }

        /// <summary>
        /// Returns a count of all bars held within <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <returns>A <see cref="long"/>.</returns>
        public long BarsCount(BarType barType)
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetBarTypeWildcardString(barType)).ToArray();

            if (allKeys.Length == 0)
            {
                return 0;
            }

            return allKeys
                .Select(key => this.GetBarsByDay(key))
                .Sum(bars => bars.Value.Length);
        }

        /// <summary>
        /// Returns a count of all bar strings held within the <see cref="Redis"/> namespace 'MarketData'.
        /// </summary>
        /// <returns>A <see cref="long"/>.</returns>
        public long AllBarsCount()
        {
            var allKeys = this.redisServer.Keys(pattern: KeyProvider.BarsNamespaceWildcard).ToArray();

            if (allKeys.Length == 0)
            {
                return 0;
            }

            return allKeys
                .Select(key => this.GetBarsByDay(key))
                .Sum(bars => bars.Value.Length);
        }

        /// <summary>
        /// Returns a list of all market data keys based on the given bar specification.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <returns>A query result of <see cref="IReadOnlyList{T}"/> strings.</returns>
        public QueryResult<List<string>> GetAllSortedKeys(BarType barType)
        {
            if (this.KeysCount(barType) == 0)
            {
                return QueryResult<List<string>>.Fail($"No market data found for {barType}.");
            }

            var allKeys = this.redisServer.Keys(pattern: KeyProvider.GetBarTypeWildcardString(barType));

            var keyStrings = allKeys
                .Select(key => key.ToString())
                .ToList();
            keyStrings.Sort();

            return QueryResult<List<string>>.Ok(keyStrings);
        }

        /// <summary>
        /// Returns a list of all market data keys based on the given bar specification.
        /// </summary>
        /// <param name="resolution">The bar resolution keys.</param>
        /// <returns>The result of the query.</returns>
        [PerformanceOptimized]
        public Dictionary<string, List<string>> GetSortedKeysBySymbolResolution(Resolution resolution)
        {
            var allKeysBytes = this.redisServer.Keys(pattern: KeyProvider.GetBarsWildcardString());

            var keysCollection = allKeysBytes
                .Select(key => key.ToString())
                .ToList();

            var keysOfResolution = new Dictionary<string, List<string>>();

            foreach (var key in keysCollection)
            {
                if (!key.Contains(resolution.ToString().ToLower()))
                {
                    // Found resolution not applicable.
                    continue;
                }

                var splitKey = key.Split(':');
                var symbolKey = splitKey[1] + ":" + splitKey[2];

                if (!keysOfResolution.ContainsKey(symbolKey))
                {
                    keysOfResolution.Add(symbolKey, new List<string>());
                }

                keysOfResolution[symbolKey].Add(key);
                keysOfResolution[symbolKey].Sort();
            }

            return keysOfResolution;
        }

        /// <summary>
        /// Adds the given bar to the <see cref="Redis"/> List associated with the
        /// <see cref="BarDataKey"/>.
        /// </summary>
        /// <param name="barType">The bar type to add.</param>
        /// <param name="bar">The bar to add.</param>
        /// <returns>A command result.</returns>
        [PerformanceOptimized]
        public CommandResult AddBar(BarType barType, Bar bar)
        {
            var key = new BarDataKey(barType, new DateKey(bar.Timestamp)).ToString();
            this.redisDatabase.ListRightPush(key, bar.ToString());

            return CommandResult.Ok($"Added 1 bar to {barType}.");
        }

        /// <summary>
        /// Adds the given bars to the <see cref="Redis"/> Lists associated with their
        /// <see cref="BarDataKey"/>(s).
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="bars">The bars to add.</param>
        /// <returns>A command result.</returns>
        [PerformanceOptimized]
        public CommandResult AddBars(BarType barType, Bar[] bars)
        {
            Debug.EqualTo(barType.Specification.Period, 1, nameof(barType.Specification.Period));
            Debug.NotEmpty(bars, nameof(bars));

            var barsAddedCounter = 0;
            var barsIndex = this.OrganizeBarsByDay(bars);

            foreach (var (dateKey, value) in barsIndex)
            {
                var key = new BarDataKey(barType, dateKey).ToString();

                if (!this.KeyExists(key))
                {
                    foreach (var bar in value)
                    {
                        this.redisDatabase.ListRightPush(key, bar.ToString());
                        barsAddedCounter++;
                    }

                    continue;
                }

                // The key should exist in Redis because it was just checked by KeyExists().
                var persistedBars = this.GetBarsByDay(key).Value;

                foreach (var bar in value)
                {
                    if (bar.Timestamp.IsGreaterThan(persistedBars.Last().Timestamp))
                    {
                        this.redisDatabase.ListRightPush(key, bar.ToString());
                        barsAddedCounter++;
                    }
                }
            }

            return CommandResult.Ok(
                $"Added {barsAddedCounter} bars to {barType} (TotalCount={this.BarsCount(barType)}).");
        }

        /// <summary>
        /// Returns all bars from <see cref="Redis"/> of the given <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barType">The specification of bars to get.</param>
        /// <returns>A read only collection of <see cref="Bar"/>(s).</returns>
        [PerformanceOptimized]
        public QueryResult<BarDataFrame> GetAllBars(BarType barType)
        {
            Debug.EqualTo(barType.Specification.Period, 1, nameof(barType.Specification.Period));

            var barKeysQuery = this.GetAllSortedKeys(barType);

            if (barKeysQuery.IsFailure)
            {
                return QueryResult<BarDataFrame>.Fail(barKeysQuery.Message);
            }

            var barsArray = barKeysQuery
                .Value
                .SelectMany(key => this.redisDatabase.ListRange(key))
                .Select(value => value.ToString())
                .Select(BarFactory.Create)
                .ToArray();

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, barsArray));
        }

        /// <summary>
        /// Returns all bars from <see cref="Redis"/> of the given <see cref="BarSpecification"/> within the given
        /// range of <see cref="ZonedDateTime"/> (inclusive).
        /// </summary>
        /// <param name="barType">The specification of bars to get.</param>
        /// <param name="fromDateTime">The from date time range.</param>
        /// <param name="toDateTime">The to date time range.</param>
        /// <returns>A read only collection of <see cref="Bar"/>(s).</returns>
        public QueryResult<BarDataFrame> GetBars(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));

            if (this.KeysCount(barType) == 0)
            {
                return QueryResult<BarDataFrame>.Fail($"No market data found for {barType}.");
            }

            var barKeys = KeyProvider.GetBarsKeyStrings(barType, fromDateTime, toDateTime);

            var barsArray = barKeys
                .SelectMany(key => this.redisDatabase.ListRange(key))
                .Select(value => value.ToString())
                .Select(BarFactory.Create)
                .Where(bar => bar.Timestamp.IsGreaterThanOrEqualTo(fromDateTime)
                           && bar.Timestamp.IsLessThanOrEqualTo(toDateTime))
                .ToArray();

            if (barsArray.Length == 0)
            {
                return QueryResult<BarDataFrame>.Fail(
                    $"No market data found for {barType} in time range from " +
                    $"{fromDateTime.ToIsoString()} to " +
                    $"{toDateTime.ToIsoString()}.");
            }

            return QueryResult<BarDataFrame>.Ok(new BarDataFrame(barType, barsArray));
        }

        /// <summary>
        /// Returns a query result if success containing the requested bar, or failure containing
        /// a message.
        /// </summary>
        /// <param name="barType">The requested bars specification.</param>
        /// <param name="timestamp">The requested bars timestamp.</param>
        /// <returns>A query result of <see cref="Bar"/>.</returns>
        [PerformanceOptimized]
        public QueryResult<Bar> GetBar(BarType barType, ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            var key = new BarDataKey(barType, new DateKey(timestamp));
            var persistedBars = this.GetBarsByDay(key.ToString());

            if (persistedBars.IsFailure)
            {
                return QueryResult<Bar>.Fail(persistedBars.Message);
            }

            for (var i = 0; i < persistedBars.Value.Length; i++)
            {
                if (persistedBars.Value[i].Timestamp.Equals(timestamp))
                {
                    return QueryResult<Bar>.Ok(persistedBars.Value[i]);
                }
            }

            return QueryResult<Bar>.Fail($"No market data found for {barType} " +
                                         $"at {timestamp.ToIsoString()}.");
        }

        /// <summary>
        /// Finds and returns bars by the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The query result list of bars.</returns>
        public QueryResult<Bar[]> GetBarsByDay(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.KeyExists(key))
            {
                return QueryResult<Bar[]>.Fail($"No market data found for {key}.");
            }

            var values = this.redisDatabase.ListRange(key);

            return QueryResult<Bar[]>.Ok(Array.ConvertAll(values, b => BarFactory.Create(b)));
        }

        /// <summary>
        /// Organizes the given bars array into a dictionary of bar lists indexed by a date key.
        /// </summary>
        /// <param name="bars">The bars array.</param>
        /// <returns>The organized dictionary.</returns>
        public Dictionary<DateKey, List<Bar>> OrganizeBarsByDay(Bar[] bars)
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

        /// <summary>
        /// Deletes the given key if it exists in the database.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>The result of the operation.</returns>
        public CommandResult Delete(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.KeyExists(key))
            {
                return CommandResult.Fail($"Cannot find {key} to delete in the database.");
            }

            this.redisDatabase.KeyDelete(key);

            return CommandResult.Ok($"Removed {key} from the database.");
        }
    }
}
