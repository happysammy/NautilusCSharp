//--------------------------------------------------------------------------------------------------
// <copyright file="MockInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockInstrumentRepository : IInstrumentRepository
    {
        private readonly Dictionary<Symbol, Instrument> instruments;
        private readonly IDataSerializer<Instrument> serializer;

        public MockInstrumentRepository(IDataSerializer<Instrument> serializer)
        {
            this.instruments = new Dictionary<Symbol, Instrument>();
            this.serializer = serializer;
        }

        public void ResetCache()
        {
            // Not implemented
        }

        public void CacheAll()
        {
            // Not implemented
        }

        public void DeleteAll()
        {
            this.instruments.Clear();
        }

        public void Delete(Symbol symbol)
        {
            this.instruments.Remove(symbol);
        }

        public void Add(Instrument instrument)
        {
            this.instruments.Add(instrument.Symbol, instrument);
        }

        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            return this.instruments.ContainsKey(symbol)
                ? QueryResult<Instrument>.Ok(this.instruments[symbol])
                : QueryResult<Instrument>.Fail($"no instrument found for symbol {symbol}");
        }

        public QueryResult<Instrument[]> GetInstruments(Venue venue)
        {
            return QueryResult<Instrument[]>.Ok(this.instruments.Values.ToArray());
        }

        public QueryResult<byte[][]> GetInstrumentData(Symbol symbol)
        {
            return QueryResult<byte[][]>.Ok(this.serializer.Serialize(this.instruments.Values.ToArray()));
        }

        public QueryResult<byte[][]> GetInstrumentData(Venue venue)
        {
            return QueryResult<byte[][]>.Ok(this.serializer.Serialize(this.instruments.Values.ToArray()));
        }

        public QueryResult<IEnumerable<Instrument>> FindInCache(Venue venue)
        {
            var query = this.instruments
                .Where(kvp => kvp.Key.Venue.Equals(venue))
                .Select(kvp => kvp.Value)
                .ToList();

            return query.Count > 0
                ? QueryResult<IEnumerable<Instrument>>.Ok(query)
                : QueryResult<IEnumerable<Instrument>>.Fail($"no instruments found for venue {venue}");
        }

        public IReadOnlyCollection<Symbol> GetCachedSymbols()
        {
            return this.instruments.Keys;
        }

        public void SnapshotDatabase()
        {
            // Not implemented yet
        }
    }
}
