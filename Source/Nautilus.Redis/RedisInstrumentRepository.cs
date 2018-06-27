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
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using ServiceStack.Text;
    using ServiceStack.Redis;

    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides a Redis implementation for the system instrument repository.
    /// </summary>
    public class RedisInstrumentRepository : IInstrumentRepository
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly IReadOnlyCollection<Symbol> symbolList;
        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="clientsManager">The redis clients manager.</param>
        public RedisInstrumentRepository(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
            this.symbolList = new List<Symbol>();
        }

        public IReadOnlyCollection<Symbol> SymbolList => this.symbolList;

        public CommandResult LoadAllInstrumentsFromDatabase()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(Instrument instrument)
        {
            using (var client = this.clientsManager.GetClient())
            {
                client.Set(
                    $"instruments:{instrument.Symbol}",
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
                        $"instruments:{instrument.Symbol}",
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

        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            using (var redis = this.clientsManager.GetClient())
            {
                if (redis.ContainsKey("instruments:" + symbol))
                {
                    var serialized = redis.Get<string>("instruments:" + symbol);
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

                return QueryResult<Instrument>.Fail($"Could not find instrument:{symbol}");
            }
        }

        public QueryResult<decimal> GetTickSize(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Dispose()
        {
            this.clientsManager.Dispose();

            return CommandResult.Ok();
        }
    }
}
