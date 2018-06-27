// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using QuickFix.Fields;
    using ServiceStack.Redis;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides a Redis implementation for the system instrument repository.
    /// </summary>
    public class RedisInstrumentRepository : IInstrumentRepository
    {
        private readonly IRedisClientsManager clientsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisInstrumentRepository"/> class.
        /// </summary>
        /// <param name="clientsManager">The redis clients manager.</param>
        public RedisInstrumentRepository(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
        }

        public IReadOnlyCollection<Symbol> InstrumentSymbolList { get; }

        public CommandResult LoadAllInstrumentsFromDatabase()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(Instrument instrument)
        {
            using (var client = this.clientsManager.GetClient())
            {
                client.As<Instrument>();

                client.Store(instrument);

                return CommandResult.Ok($"Added the {instrument.Symbol} instrument to the repository");
            }
        }

        public CommandResult Add(IReadOnlyCollection<Instrument> instruments)
        {
            using (var client = this.clientsManager.GetClient())
            {
                client.As<Instrument>();

                foreach (var instrument in instruments)
                {
                    client.Store(instrument);
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

        public QueryResult<IList<string>> GetAllKeys()
        {
            using (var redis = this.clientsManager.GetClient())
            {
                var client = redis.As<Instrument>();

                var ids = client.GetAllKeys();

                return ids != null
                    ? QueryResult<IList<string>>.Ok(ids)
                    : QueryResult<IList<string>>.Fail($"Could not find instrument {ids}");
            }
        }

        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            using (var redis = this.clientsManager.GetClient())
            {
                var client = redis.As<Instrument>();


                var instrument = client.GetById($"urn:instrument:{symbol}");

                return instrument != null
                    ? QueryResult<Instrument>.Ok(instrument)
                    : QueryResult<Instrument>.Fail($"Could not find urn:instrument:{symbol}");
            }
        }

        public QueryResult<string> GetInstrumentString(Symbol symbol)
        {
            using (var redis = this.clientsManager.GetClient())
            {
                var client = redis.As<Instrument>();


                var instrument = client.GetById(symbol).ToString();

                return instrument != null
                    ? QueryResult<string>.Ok(instrument)
                    : QueryResult<string>.Fail($"Could not find urn:instrument:{symbol}");
            }
        }

        public QueryResult<decimal> GetTickSize(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
