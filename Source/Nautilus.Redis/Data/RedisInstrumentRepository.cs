﻿// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Data;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.CQS;
using Nautilus.Core.Extensions;
using Nautilus.Data.Interfaces;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.Redis.Data.Internal;
using StackExchange.Redis;

namespace Nautilus.Redis.Data
{
    /// <summary>
    /// Provides a repository for handling <see cref="Instrument"/>s with Redis.
    /// </summary>
    public sealed class RedisInstrumentRepository : DataBusConnected, IInstrumentRepository
    {
        private readonly IDataSerializer<Instrument> serializer;
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;
        private readonly IDictionary<Symbol, Instrument> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="serializer">The instrument serializer.</param>
        /// <param name="connection">The redis connection multiplexer.</param>
        public RedisInstrumentRepository(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            IDataSerializer<Instrument> serializer,
            ConnectionMultiplexer connection)
            : base(container, dataBusAdapter)
        {
            this.serializer = serializer;
            this.redisServer = connection.GetServer(RedisConstants.Localhost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.cache = new Dictionary<Symbol, Instrument>();

            this.RegisterHandler<Instrument>(this.OnData);

            this.Subscribe<Instrument>();
        }

        /// <inheritdoc />
        public void ResetCache()
        {
            this.cache.Clear();
        }

        /// <inheritdoc />
        public void CacheAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                var query = this.Read(key);

                if (!query.IsSuccess || this.cache.ContainsKey(query.Value.Symbol))
                {
                    continue; // Did not contain instruct or already cached
                }

                var instrument = query.Value;
                this.cache.Add(instrument.Symbol, instrument);
            }
        }

        /// <inheritdoc />
        public void Delete(Symbol symbol)
        {
            this.redisDatabase.KeyDelete(KeyProvider.GetInstrumentsKey(symbol));

            this.Logger.LogInformation(LogId.Database, $"Instrument {symbol.Value} deleted.");
        }

        /// <inheritdoc />
        public void DeleteAll()
        {
            var keys = this.GetAllKeys();

            foreach (var key in keys)
            {
                this.redisDatabase.KeyDelete(key);
            }
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public void Add(Instrument instrument)
        {
            var symbol = instrument.Symbol;

            if (!this.cache.ContainsKey(symbol))
            {
                this.cache.Add(symbol, instrument);
                this.Write(instrument);
                this.Logger.LogInformation(LogId.Database, $"Added Instrument {symbol.Value}.");
                return;
            }

            var instrumentBuilder = new InstrumentBuilder(this.cache[symbol]).Update(instrument);
            if (instrumentBuilder.Changes.Count == 0)
            {
                this.Logger.LogInformation(LogId.Database, $"Instrument {symbol.Value} unchanged in cache.");
                return;
            }

            var changesString = new StringBuilder();
            foreach (var change in instrumentBuilder.Changes)
            {
                changesString.Append(change);
            }

            var updatedInstrument = instrumentBuilder.Build(this.TimeNow());
            this.cache[symbol] = updatedInstrument;
            this.Write(instrument);

            this.Logger.LogInformation(LogId.Database, $"Updated instrument {symbol.Value}" + changesString + ".");
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            this.redisServer.Save(SaveType.BackgroundSave, CommandFlags.FireAndForget);
        }

        /// <summary>
        /// Returns all instrument keys from the Redis database.
        /// </summary>
        /// <returns>The keys.</returns>
        public IReadOnlyCollection<string> GetAllKeys()
        {
            return this.redisServer.Keys(pattern: KeyProvider.GetInstrumentsPattern())
                .Select(k => k.ToString())
                .ToArray();
        }

        /// <inheritdoc />
        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            return this.cache.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.cache[symbol])
                : QueryResult<Instrument>.Fail($"Cannot find instrument {symbol.Value}");
        }

        /// <inheritdoc />
        public QueryResult<Instrument[]> GetInstruments(Venue venue)
        {
            var instruments = this.cache
                .Where(kvp => kvp.Key.Venue == venue)
                .Select(kvp => kvp.Value)
                .ToArray();

            return instruments.Length > 0
                ? QueryResult<Instrument[]>.Ok(instruments)
                : QueryResult<Instrument[]>.Fail($"Cannot find any instrument for {venue}");
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetInstrumentData(Symbol symbol)
        {
            var instrumentQuery = this.GetInstrument(symbol);
            if (instrumentQuery.IsFailure)
            {
                return QueryResult<byte[][]>.Fail(instrumentQuery.Message);
            }

            return QueryResult<byte[][]>.Ok(new[] { this.serializer.Serialize(instrumentQuery.Value) });
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetInstrumentData(Venue venue)
        {
            var instrumentQuery = this.GetInstruments(venue);
            if (instrumentQuery.IsFailure)
            {
                return QueryResult<byte[][]>.Fail(instrumentQuery.Message);
            }

            var instrumentsLength = instrumentQuery.Value.Length;
            var instrumentsBytes = new byte[instrumentsLength][];
            for (var i = 0; i < instrumentsLength; i++)
            {
                instrumentsBytes[i] = this.serializer.Serialize(instrumentQuery.Value[i]);
            }

            return QueryResult<byte[][]>.Ok(instrumentsBytes);
        }

        /// <summary>
        /// Returns the instrument from the Redis database matching the given key.
        /// </summary>
        /// <param name="key">The instrument key to read.</param>
        /// <returns>The keys.</returns>
        public QueryResult<Instrument> Read(string key)
        {
            Debug.NotEmptyOrWhiteSpace(key, nameof(key));

            if (!this.redisDatabase.KeyExists(key))
            {
                return QueryResult<Instrument>.Fail($"Cannot find {key}");
            }

            var instrumentDict = this.redisDatabase.HashGetAll(key)
                .ToDictionary(
                    hashEntry => hashEntry.Name.ToString(),
                    hashEntry => hashEntry.Value.ToString());

            var securityType = instrumentDict[nameof(Instrument.SecurityType)].ToEnum<SecurityType>();
            if (securityType == SecurityType.Forex)
            {
                var forexCcy = new ForexInstrument(
                    Symbol.FromString(instrumentDict[nameof(Instrument.Symbol)]),
                    int.Parse(instrumentDict[nameof(Instrument.PricePrecision)]),
                    int.Parse(instrumentDict[nameof(Instrument.SizePrecision)]),
                    int.Parse(instrumentDict[nameof(Instrument.MinStopDistanceEntry)]),
                    int.Parse(instrumentDict[nameof(Instrument.MinLimitDistanceEntry)]),
                    int.Parse(instrumentDict[nameof(Instrument.MinStopDistance)]),
                    int.Parse(instrumentDict[nameof(Instrument.MinLimitDistance)]),
                    Price.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.TickSize)])),
                    Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.RoundLotSize)])),
                    Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.MinTradeSize)])),
                    Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.MaxTradeSize)])),
                    Parser.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestBuy)]),
                    Parser.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestSell)]),
                    instrumentDict[nameof(Instrument.Timestamp)].ToZonedDateTimeFromIso());

                return QueryResult<Instrument>.Ok(forexCcy);
            }

            var instrument = new Instrument(
                Symbol.FromString(instrumentDict[nameof(Instrument.Symbol)]),
                instrumentDict[nameof(Instrument.QuoteCurrency)].ToEnum<Currency>(),
                securityType,
                int.Parse(instrumentDict[nameof(Instrument.PricePrecision)]),
                int.Parse(instrumentDict[nameof(Instrument.SizePrecision)]),
                int.Parse(instrumentDict[nameof(Instrument.MinStopDistanceEntry)]),
                int.Parse(instrumentDict[nameof(Instrument.MinLimitDistanceEntry)]),
                int.Parse(instrumentDict[nameof(Instrument.MinStopDistance)]),
                int.Parse(instrumentDict[nameof(Instrument.MinLimitDistance)]),
                Price.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.TickSize)])),
                Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.RoundLotSize)])),
                Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.MinTradeSize)])),
                Quantity.Create(Parser.ToDecimal(instrumentDict[nameof(Instrument.MaxTradeSize)])),
                Parser.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestBuy)]),
                Parser.ToDecimal(instrumentDict[nameof(Instrument.RolloverInterestSell)]),
                instrumentDict[nameof(Instrument.Timestamp)].ToZonedDateTimeFromIso());

            return QueryResult<Instrument>.Ok(instrument);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Symbol> GetCachedSymbols() => this.cache.Keys.ToArray();

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            // No actions to perform
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.Logger.LogDebug(LogId.Database, "Saving database...");
            this.SnapshotDatabase();
            this.Logger.LogInformation(LogId.Database, "Database saved.");
        }

        private void OnData(Instrument instrument)
        {
            this.Add(instrument);
        }

        private void Write(Instrument instrument)
        {
            if (instrument is ForexInstrument forexCcy)
            {
                var forexHash = new[]
                {
                    new HashEntry(nameof(ForexInstrument.Symbol), forexCcy.Symbol.Value),
                    new HashEntry(nameof(ForexInstrument.BaseCurrency), forexCcy.BaseCurrency.ToString()),
                    new HashEntry(nameof(ForexInstrument.QuoteCurrency), forexCcy.QuoteCurrency.ToString()),
                    new HashEntry(nameof(ForexInstrument.SecurityType), forexCcy.SecurityType.ToString()),
                    new HashEntry(nameof(ForexInstrument.PricePrecision), forexCcy.PricePrecision),
                    new HashEntry(nameof(ForexInstrument.SizePrecision), instrument.SizePrecision),
                    new HashEntry(nameof(ForexInstrument.MinStopDistanceEntry), forexCcy.MinStopDistanceEntry),
                    new HashEntry(nameof(ForexInstrument.MinLimitDistanceEntry), forexCcy.MinLimitDistanceEntry),
                    new HashEntry(nameof(ForexInstrument.MinStopDistance), forexCcy.MinStopDistance),
                    new HashEntry(nameof(ForexInstrument.MinLimitDistance), forexCcy.MinLimitDistance),
                    new HashEntry(nameof(ForexInstrument.TickSize), forexCcy.TickSize.ToString()),
                    new HashEntry(nameof(ForexInstrument.RoundLotSize), forexCcy.RoundLotSize.ToString()),
                    new HashEntry(nameof(ForexInstrument.MinTradeSize), forexCcy.MinTradeSize.ToString()),
                    new HashEntry(nameof(ForexInstrument.MaxTradeSize), forexCcy.MaxTradeSize.ToString()),
                    new HashEntry(nameof(ForexInstrument.RolloverInterestBuy), forexCcy.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture)),
                    new HashEntry(nameof(ForexInstrument.RolloverInterestSell), forexCcy.RolloverInterestSell.ToString(CultureInfo.InvariantCulture)),
                    new HashEntry(nameof(ForexInstrument.Timestamp), forexCcy.Timestamp.ToIso8601String()),
                };

                this.redisDatabase.HashSet(KeyProvider.GetInstrumentsKey(instrument.Symbol), forexHash);
            }

            var instrumentHash = new[]
            {
                new HashEntry(nameof(Instrument.Symbol), instrument.Symbol.Value),
                new HashEntry(nameof(Instrument.QuoteCurrency), instrument.QuoteCurrency.ToString()),
                new HashEntry(nameof(Instrument.SecurityType), instrument.SecurityType.ToString()),
                new HashEntry(nameof(Instrument.PricePrecision), instrument.PricePrecision),
                new HashEntry(nameof(Instrument.SizePrecision), instrument.SizePrecision),
                new HashEntry(nameof(Instrument.MinStopDistanceEntry), instrument.MinStopDistanceEntry),
                new HashEntry(nameof(Instrument.MinLimitDistanceEntry), instrument.MinLimitDistanceEntry),
                new HashEntry(nameof(Instrument.MinStopDistance), instrument.MinStopDistance),
                new HashEntry(nameof(Instrument.MinLimitDistance), instrument.MinLimitDistance),
                new HashEntry(nameof(Instrument.TickSize), instrument.TickSize.ToString()),
                new HashEntry(nameof(Instrument.RoundLotSize), instrument.RoundLotSize.ToString()),
                new HashEntry(nameof(Instrument.MinTradeSize), instrument.MinTradeSize.ToString()),
                new HashEntry(nameof(Instrument.MaxTradeSize), instrument.MaxTradeSize.ToString()),
                new HashEntry(nameof(Instrument.RolloverInterestBuy), instrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(nameof(Instrument.RolloverInterestSell), instrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture)),
                new HashEntry(nameof(Instrument.Timestamp), instrument.Timestamp.ToIso8601String()),
            };

            this.redisDatabase.HashSet(KeyProvider.GetInstrumentsKey(instrument.Symbol), instrumentHash);
        }
    }
}
