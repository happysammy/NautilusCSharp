// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Aggregators;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Newtonsoft.Json;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a Redis implementation for the system instrument repository.
    /// </summary>
    public class RedisInstrumentRepository : IInstrumentRepository
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;
        private readonly IDictionary<Symbol, Instrument> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="connection">The redis clients manager.</param>
        public RedisInstrumentRepository(ConnectionMultiplexer connection)
        {
            Validate.NotNull(connection, nameof(connection));

            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.cache = new Dictionary<Symbol, Instrument>();
        }

        /// <summary>
        /// Clears all instruments from the in-memory cache.
        /// </summary>
        public void ResetCache()
        {
            this.cache.Clear();
        }

        /// <summary>
        /// Adds all persisted instruments to the in-memory cache.
        /// </summary>
        public void CacheAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                var query = this.Read(key);

                if (query.IsSuccess && !this.cache.ContainsKey(query.Value.Symbol))
                {
                    var instrument = query.Value;

                    this.cache.Add(instrument.Symbol, instrument);
                }
            }
        }

        /// <summary>
        /// Deletes the instrument of the given symbol from the Redis database.
        /// </summary>
        /// <param name="symbol">The symbol to delete.</param>
        public void Delete(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.redisDatabase.KeyDelete(KeyProvider.GetInstrumentKey(symbol));
        }

        /// <summary>
        /// Deletes all instruments from the Redis database.
        /// </summary>
        public void DeleteAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                this.redisDatabase.KeyDelete(key);
            }
        }

        /// <summary>
        /// Returns all instrument keys from the Redis database.
        /// </summary>
        /// <returns>The keys.</returns>
        public IReadOnlyCollection<string> GetAllKeys()
        {
            return this.redisServer.Keys(pattern: KeyProvider.InstrumentsWildcard)
                .Select(k => k.ToString())
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Adds the given instrument to the repository.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The result of the operation.</returns>
        [PerformanceOptimized]
        public CommandResult Add(Instrument instrument, ZonedDateTime timeNow)
        {
            Debug.NotNull(instrument, nameof(instrument));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var symbol = instrument.Symbol;

            if (!this.cache.ContainsKey(symbol))
            {
                this.cache.Add(symbol, instrument);
                return this.Write(instrument);
            }

            var instrumentBuilder = new InstrumentBuilder(this.cache[symbol]).Update(instrument);

            if (instrumentBuilder.Changes.Count == 0)
            {
                return CommandResult.Ok($"Instrument {symbol} unchanged in cache...");
            }

            var changesString = new StringBuilder();

            foreach (var change in instrumentBuilder.Changes)
            {
                changesString.Append(change);
            }

            var updatedInstrument = instrumentBuilder.Build(timeNow);

            this.cache[symbol] = updatedInstrument;
            this.Write(instrument);

            return CommandResult.Ok($"Updated instrument {symbol}" + changesString);
        }

        /// <summary>
        /// Returns the instrument from the repository if contained (otherwise returns a failed
        /// query).
        /// </summary>
        /// <param name="symbol">The instrument symbol.</param>
        /// <returns>The result of the query.</returns>
        public QueryResult<Instrument> FindInCache(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            return this.cache.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.cache[symbol])
                : QueryResult<Instrument>.Fail($"Cannot find instrument {symbol}");
        }

        /// <summary>
        /// Returns the instrument from the Redis database with the given key.
        /// </summary>
        /// <param name="key">The instruments key.</param>
        /// <returns>The result of the query.</returns>
        public QueryResult<Instrument> Read(string key)
        {
            Debug.NotNull(key, nameof(key));

            if (!this.redisDatabase.KeyExists(key))
            {
                return QueryResult<Instrument>.Fail($"Cannot find {key}");
            }

            var serialized = this.redisDatabase.StringGet(key).ToString();
            var deserialized = JsonConvert.DeserializeObject(serialized);

// var deserializedSymbol = deserialized["Symbol"];
//            var symbolCode = deserializedSymbol["Code"];
//            var exchange = deserializedSymbol["Symbol"]["Venue"];
//            var brokerSymbol = deserialized["BrokerSymbol"]["Value"];
//
//            var instrument = new Instrument(
//                new Symbol(symbolCode, exchange.ToEnum<Venue>()),
//                new InstrumentId(deserializedSymbol["Value"]),
//                new BrokerSymbol(brokerSymbol),
//                deserialized["QuoteCurrency"].ToEnum<CurrencyCode>(),
//                deserialized["SecurityType"].ToEnum<SecurityType>(),
//                Convert.ToInt32(deserialized["TickDecimals"]),
//                Convert.ToDecimal(deserialized["TickSize"]),
//                Convert.ToDecimal(deserialized["TickValue"]),
//                Convert.ToDecimal(deserialized["TargetDirectSpread"]),
//                Convert.ToInt32(deserialized["RoundLotSize"]),
//                Convert.ToInt32(deserialized["ContractSize"]),
//                Convert.ToInt32(deserialized["MinStopDistanceEntry"]),
//                Convert.ToInt32(deserialized["MinLimitDistanceEntry"]),
//                Convert.ToInt32(deserialized["MinStopDistance"]),
//                Convert.ToInt32(deserialized["MinLimitDistance"]),
//                Convert.ToInt32(deserialized["MinTradeSize"]),
//                Convert.ToInt32(deserialized["MaxTradeSize"]),
//                Convert.ToDecimal(deserialized["MarginRequirement"]),
//                Convert.ToDecimal(deserialized["RolloverInterestBuy"]),
//                Convert.ToDecimal(deserialized["RolloverInterestSell"]),
//                deserialized["Timestamp"].ToZonedDateTimeFromIso());
            return QueryResult<Instrument>.Fail("OOPS");
        }

        /// <summary>
        /// Returns the list of instrument symbols currently held in cache.
        /// </summary>
        /// <returns>The symbols.</returns>
        public IReadOnlyCollection<Symbol> GetSymbols() => this.cache.Keys.ToList().AsReadOnly();

        /// <summary>
        /// Returns the dictionary index of symbols and their corresponding tick size.
        /// </summary>
        /// <returns>The tick size index.</returns>
        public Dictionary<string, int> GetPricePrecisionIndex()
        {
            return this.cache.ToDictionary(
                symbol => symbol.Key.Code,
                symbol => symbol.Value.TickDecimals);
        }

        private CommandResult Write(Instrument instrument)
        {
            var symbol = instrument.Symbol;

            this.redisDatabase.StringSet(KeyProvider.GetInstrumentKey(symbol), JsonConvert.SerializeObject(instrument));

            return CommandResult.Ok($"Added instrument {symbol}");
        }
    }
}
