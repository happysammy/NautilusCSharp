// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepository.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a repository for handling <see cref="Tick"/>s with Redis.
    /// </summary>
    public class RedisTickRepository : ITickRepository
    {
        /// <inheritdoc />
        public int TicksCount(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public int AllTicksCount()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Add(Tick tick)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Add(List<Tick> ticks)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public QueryResult<List<Tick>> Find(Symbol symbol, ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void TrimFrom(ZonedDateTime trimFrom)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            // Not implemented yet
        }
    }
}
