// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Keys;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using ServiceStack.Text;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a Redis implementation for the system instrument repository.
    /// </summary>
    public class RedisInstrumentRepository : IInstrumentRepository, IDisposable
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly IDictionary<Symbol, Instrument> cache = new Dictionary<Symbol, Instrument>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="clientsManager">The redis clients manager.</param>
        public RedisInstrumentRepository(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
        }

        /// <summary>
        /// Returns the list of instrument symbols currently held in cache.
        /// </summary>
        public IReadOnlyCollection<Symbol> GetSymbols() => this.cache.Keys.ToList().AsReadOnly();

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
        /// <returns>The result of the operation.</returns>
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
        public void Delete(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            using (var client = this.clientsManager.GetClient())
            {
                client.Remove(KeyProvider.GetInstrumentKey(symbol));
            }
        }

        /// <summary>
        /// Deletes all instruments from the Redis database.
        /// </summary>
        public void DeleteAll()
        {
            using (var client = this.clientsManager.GetClient())
            {
                var keys = this.GetAllKeys();

                foreach (var key in keys)
                {
                    client.Remove(key);
                }
            }
        }

        /// <summary>
        /// Returns all instrument keys from the Redis database.
        /// </summary>
        /// <returns>The keys.</returns>
        public IReadOnlyCollection<string> GetAllKeys()
        {
            using (var client = this.clientsManager.GetClient())
            {
                return client.GetKeysByPattern(KeyProvider.InstrumentsWildcard)
                    .ToList()
                    .AsReadOnly();
            }
        }

        /// <summary>
        /// Adds the given instrument to the repository.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The result of the operation.</returns>
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

            var instrumentBuilder = new InstrumentBuilder(this.cache[symbol])
                .Update(instrument);

            if (instrumentBuilder.Changes.Count == 0)
            {
                return CommandResult.Ok($"Instrument {symbol} unchanged in cache...");
            }

            var changesString = new StringBuilder();

            instrumentBuilder.Changes.ForEach(c => changesString.Append(c));

            var updatedInstrument = instrumentBuilder.Build(timeNow);

            this.cache.Add(symbol, updatedInstrument);
            this.Write(instrument);

            return CommandResult.Ok($"Updated instrument {symbol} " + changesString);
        }

        /// <summary>
        /// Adds the given instruments to the repository.
        /// </summary>
        /// <param name="instruments">The instruments.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The result of the operation.</returns>
        public CommandResult Add(IReadOnlyCollection<Instrument> instruments, ZonedDateTime timeNow)
        {
            Debug.NotNull(instruments, nameof(instruments));
            Debug.NotNull(timeNow, nameof(timeNow));

            var results = new List<CommandResult>();

            foreach (var instrument in instruments)
            {
                var result = this.Add(instrument, timeNow);

                results.Add(result);
            }

            return CommandResult.Combine(results.ToArray());
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

            using (var redis = this.clientsManager.GetClient())
            {
                if (redis.ContainsKey(key))
                {
                    var serialized = redis.Get<string>(key);
                    var deserialized = JsonSerializer.DeserializeFromString<JsonObject>(serialized);
                    var deserializedSymbol = deserialized["Symbol"].ToStringDictionary();

                    var instrument = new Instrument(
                        new Symbol(deserializedSymbol["Code"], deserializedSymbol["Exchange"].ToEnum<Exchange>()),
                        new EntityId(deserializedSymbol["Value"]),
                        new EntityId("AUD/USD"),
                        deserialized["CurrencyCode"].ToEnum<CurrencyCode>(),
                        deserialized["SecurityType"].ToEnum<SecurityType>(),
                        Convert.ToInt32(deserialized["TickDecimals"]),
                        Convert.ToDecimal(deserialized["TickSize"]),
                        Convert.ToInt32(deserialized["TickValue"]),
                        Convert.ToInt32(deserialized["TargetDirectSpread"]),
                        Convert.ToInt32(deserialized["ContractSize"]),
                        Convert.ToInt32(deserialized["MinStopDistanceEntry"]),
                        Convert.ToInt32(deserialized["MinLimitDistanceEntry"]),
                        Convert.ToInt32(deserialized["MinStopDistance"]),
                        Convert.ToInt32(deserialized["MinLimitDistance"]),
                        Convert.ToInt32(deserialized["MinTradeSize"]),
                        Convert.ToInt32(deserialized["MaxTradeSize"]),
                        Convert.ToInt32(deserialized["MarginRequirement"]),
                        Convert.ToDecimal(deserialized["RollOverInterestBuy"]),
                        Convert.ToDecimal(deserialized["RollOverInterestSell"]),
                        deserialized["Timestamp"].ToZonedDateTimeFromIso());

                    return QueryResult<Instrument>.Ok(instrument);
                }

                return QueryResult<Instrument>.Fail($"Cannot find {key}");
            }
        }

        /// <summary>
        /// Returns the tick size for the instrument of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>The result of the query.</returns>
        public QueryResult<decimal> GetTickSize(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            return this.cache.ContainsKey(symbol)
                ? QueryResult<decimal>.Ok(this.cache[symbol].TickSize)
                : QueryResult<decimal>.Fail($"Cannot find instrument {symbol}");
        }

        /// <summary>
        /// Disposes the instrument repository resources.
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            this.clientsManager.Dispose();
        }

        private CommandResult Write(Instrument instrument)
        {
            using (var client = this.clientsManager.GetClient())
            {
                var symbol = instrument.Symbol;

                client.Set(
                    KeyProvider.GetInstrumentKey(symbol),
                    JsonSerializer.SerializeToString(instrument));

                return CommandResult.Ok($"Added instrument ({symbol})...");
            }
        }
    }
}
