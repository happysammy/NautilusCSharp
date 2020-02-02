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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a repository for handling <see cref="Tick"/>s with Redis.
    /// </summary>
    public class RedisTickRepository : Component, ITickRepository
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisTickRepository"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The clients manager.</param>
        public RedisTickRepository(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
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
        public void TrimFrom(ZonedDateTime trimFrom)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SnapshotDatabase()
        {
            this.redisServer.Save(SaveType.BackgroundSave, CommandFlags.FireAndForget);
        }

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
        public QueryResult<Tick[]> GetTicks(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public QueryResult<byte[][]> GetTickData(Symbol symbol, DateKey fromDate, DateKey toDate, int limit = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}
