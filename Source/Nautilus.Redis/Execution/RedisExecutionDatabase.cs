// -------------------------------------------------------------------------------------------------
// <copyright file="RedisExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Execution
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;
    using StackExchange.Redis;
    using Order = Nautilus.DomainModel.Aggregates.Order;

    /// <summary>
    /// Provides an execution database backed by Redis.
    /// </summary>
    public class RedisExecutionDatabase : Component, IExecutionDatabase
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;

        private readonly Dictionary<OrderId, Order> cachedOrders;
        private readonly Dictionary<PositionId, Position> cachedPositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisExecutionDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The redis connection multiplexer.</param>
        public RedisExecutionDatabase(IComponentryContainer container, ConnectionMultiplexer connection)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();

            this.cachedOrders = new Dictionary<OrderId, Order>();
            this.cachedPositions = new Dictionary<PositionId, Position>();
        }

        /// <inheritdoc />
        public void AddOrder(AtomicOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddPosition(Position position)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void CheckResiduals()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Reset()
        {
            this.cachedOrders.Clear();
            this.cachedPositions.Clear();
        }

        /// <inheritdoc />
        public void Flush()
        {
            this.redisServer.FlushDatabase();
        }

        /// <inheritdoc />
        public ICollection<TraderId> GetTraderIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderWorkingIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderCompletedIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionOpenIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionClosedIds()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Order? GetOrder(OrderId orderId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Position? GetPosition(PositionId positionId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Position? GetPositionForOrder(OrderId orderId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public PositionId? GetPositionId(OrderId orderId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
