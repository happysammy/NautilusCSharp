//--------------------------------------------------------------------------------------------------
// <copyright file="MockInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
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
