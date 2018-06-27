namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Nautilus.Core.CQS;
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Entities;

    public class MockEconomicEventRepository : IRepository<EconomicEvent>
    {
        public CommandResult Add(EconomicEvent entity)
        {
            throw new NotImplementedException();
        }

        public CommandResult Delete(EconomicEvent entity)
        {
            throw new NotImplementedException();
        }

        public CommandResult Update(EconomicEvent entity)
        {
            throw new NotImplementedException();
        }

        public QueryResult<EconomicEvent> GetSingle(Expression<Func<EconomicEvent, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public QueryResult<IReadOnlyCollection<EconomicEvent>> GetAll(Expression<Func<EconomicEvent, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public QueryResult<IReadOnlyCollection<EconomicEvent>> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<EconomicEvent, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }
    }
}
