//--------------------------------------------------------------------------------------------------
// <copyright file="IEconomicNewsEventRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nautilus.Core.CQS;

namespace Nautilus.Database.Core.Interfaces
{
    public interface IEconomicEventRepository<T>
    {
        CommandResult Add(T entity);

        CommandResult Delete(T entity);

        CommandResult Update(T entity);

        QueryResult<T> GetSingle(Expression<Func<T, bool>> predicate);

        QueryResult<IReadOnlyCollection<T>> GetAll(Expression<Func<T, bool>> predicate);

        QueryResult<IReadOnlyCollection<T>> GetAll();

        int Count(Expression<Func<T, bool>> predicate);

        int Count();
    }
}