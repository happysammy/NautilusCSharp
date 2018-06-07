// -------------------------------------------------------------------------------------------------
// <copyright file="RedisEconomicNewsEventRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Nautilus.Core.CQS;
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Entities;

    public class RedisEconomicNewsEventRepository : IEconomicEventRepository<EconomicEvent>
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
