//--------------------------------------------------------------------------------------------------
// <copyright file="MockInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockInstrumentRepository : IInstrumentRepository
    {
        private readonly Dictionary<Symbol, Instrument> instruments;

        public MockInstrumentRepository()
        {
            this.instruments = new Dictionary<Symbol, Instrument>();
        }

        public void ResetCache()
        {
            throw new System.NotImplementedException();
        }

        public void CacheAll()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new System.NotImplementedException();
        }

        public void Delete(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult Add(Instrument instrument, ZonedDateTime timeNow)
        {
            this.instruments.Add(instrument.Symbol, instrument);

            return CommandResult.Ok();
        }

        public QueryResult<Instrument> FindInCache(Symbol symbol)
        {
            return QueryResult<Instrument>.Ok(this.instruments[symbol]);
        }

        public QueryResult<IEnumerable<Instrument>> FindInCache(Venue venue)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<Symbol> GetInstrumentSymbols()
        {
            return this.instruments.Keys;
        }
    }
}
