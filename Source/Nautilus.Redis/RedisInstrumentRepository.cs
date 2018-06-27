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
    using System.Runtime.InteropServices.WindowsRuntime;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Keys;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using ServiceStack.Text;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a Redis implementation for the system instrument repository.
    /// </summary>
    public class RedisInstrumentRepository : IInstrumentRepository, IDisposable
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly IDictionary<Symbol, Instrument> instrumentCache = new Dictionary<Symbol, Instrument>();

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
        public IReadOnlyCollection<Symbol> GetSymbols() => this.instrumentCache.Keys.ToList().AsReadOnly();

        /// <summary>
        /// Caches all instruments into memory inside the repository.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public CommandResult CacheAllInstruments()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                this.Read(key)
                    .OnSuccess(i => this.instrumentCache.Add(i.Symbol, i));
            }

            return CommandResult.Ok();
        }

        public IEnumerable<string> GetAllKeys()
        {
            using (var client = this.clientsManager.GetClient())
            {
                return client.GetKeysByPattern(KeyProvider.InstrumentsWildcard);
            }
        }

        public CommandResult Add(Instrument instrument)
        {
            using (var client = this.clientsManager.GetClient())
            {
                client.Set(
                    KeyProvider.GetInstrumentKey(instrument.Symbol),
                    JsonSerializer.SerializeToString(instrument));

                return CommandResult.Ok($"Added the {instrument.Symbol} instrument to the repository");
            }
        }

        public CommandResult Add(IReadOnlyCollection<Instrument> instruments)
        {
            using (var client = this.clientsManager.GetClient())
            {
                foreach (var instrument in instruments)
                {
                    client.Set(
                        KeyProvider.GetInstrumentKey(instrument.Symbol),
                        JsonSerializer.SerializeToString(instrument));
                }

                return CommandResult.Ok("Added all instruments to the repository");
            }
        }

        public CommandResult DeleteAll()
        {
            using (var client = this.clientsManager.GetClient())
            {
                client.FlushAll();
            }

            return CommandResult.Ok();
        }

        /// <summary>
        /// Returns the instrument from the repository if contained (otherwise returns a failed
        /// query).
        /// </summary>
        /// <param name="symbol">The instrument symbol.</param>
        /// <returns>A <see cref="QueryResult{Instrument}"/>.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public QueryResult<Instrument> Find(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            return this.instrumentCache.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.instrumentCache[symbol])
                : QueryResult<Instrument>.Fail($"Cannot find instrument {symbol}");
        }


        public QueryResult<Instrument> Read(string key)
        {
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

        private CommandResult StoreInstrument(Instrument instrument)
        {
            Debug.NotNull(instrument, nameof(instrument));

//            var symbol = instrument.Symbol;
//
//            if (!this.instrumentIndex.ContainsKey(symbol))
//            {
//                this.instrumentIndex.Add(symbol, instrument);
//                this.database.Store(instrument);
//
//                return CommandResult.Ok($"Added instrument ({instrument})...");
//            }
//
//            var instrumentBuilder = new InstrumentBuilder(this.instrumentIndex[symbol]).Update(instrument);
//
//            if (instrumentBuilder.Changes.Count == 0)
//            {
//                return CommandResult.Ok();
//            }
//
//            var changesString = new StringBuilder();
//
//            instrumentBuilder.Changes.ForEach(c => changesString.Append(c));
//
//            var updatedInstrument = instrumentBuilder.Build(this.clock.TimeNow());
//
//            this.database.Delete(this.instrumentIndex[symbol]);
//            this.instrumentIndex.Remove(symbol);
//
//            this.instrumentIndex.Add(symbol, updatedInstrument);
//            this.database.Store(updatedInstrument);

            return CommandResult.Ok(); //($"Instrument {symbol} updated" + changesString);
        }

        /// <summary>
        /// Returns the tick size for the instrument of the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>The tick size decimal.</returns>
        public QueryResult<decimal> GetTickSize(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            return this.instrumentCache.ContainsKey(symbol)
                ? QueryResult<decimal>.Ok(this.instrumentCache[symbol].TickSize)
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
    }
}
