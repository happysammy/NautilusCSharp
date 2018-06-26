namespace Nautilus.Redis
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    public class RedisInstrumentRepository : IInstrumentRepository
    {
        public IReadOnlyCollection<Symbol> InstrumentSymbolList { get; }

        public CommandResult LoadAllInstrumentsFromDatabase()
        {
            throw new System.NotImplementedException();
        }

        public CommandResult UpdateInstrument(Instrument instrument)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult UpdateInstruments(IReadOnlyCollection<Instrument> instruments)
        {
            throw new System.NotImplementedException();
        }

        public CommandResult DeleteAllInstrumentsFromDatabase()
        {
            throw new System.NotImplementedException();
        }

        public QueryResult<Instrument> GetInstrument(Symbol symbol)
        {
            throw new System.NotImplementedException();
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
