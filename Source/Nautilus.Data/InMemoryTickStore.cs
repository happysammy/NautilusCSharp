//--------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTickStore.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an in memory tick store.
    /// </summary>
    public class InMemoryTickStore : ITickRepository
    {
        /// <inheritdoc />
        public long TicksCount(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public long AllTicksCount()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public CommandResult Add(Tick tick)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public CommandResult Add(IEnumerable<Tick> ticks)
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
        public CommandResult TrimToDays(Resolution resolution, int trimToDays)
        {
            throw new System.NotImplementedException();
        }
    }
}
