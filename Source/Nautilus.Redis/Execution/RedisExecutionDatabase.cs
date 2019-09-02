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
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;
    using StackExchange.Redis;
    using Order = Nautilus.DomainModel.Aggregates.Order;

    /// <summary>
    /// Provides an execution database implemented with Redis.
    /// </summary>
    public class RedisExecutionDatabase : Component, IExecutionDatabase
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;
        private readonly ISerializer<Command> commandSerializer;
        private readonly ISerializer<Event> eventSerializer;
        private readonly bool optionLoadCache;

        private readonly Dictionary<OrderId, Order> cachedOrders;
        private readonly Dictionary<PositionId, Position> cachedPositions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisExecutionDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The redis connection multiplexer.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="optionLoadCache">The option flag to load caches from Redis on instantiation.</param>
        public RedisExecutionDatabase(
            IComponentryContainer container,
            ConnectionMultiplexer connection,
            ISerializer<Command> commandSerializer,
            ISerializer<Event> eventSerializer,
            bool optionLoadCache)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.commandSerializer = commandSerializer;
            this.eventSerializer = eventSerializer;
            this.optionLoadCache = optionLoadCache;

            this.cachedOrders = new Dictionary<OrderId, Order>();
            this.cachedPositions = new Dictionary<PositionId, Position>();
        }

        /// <inheritdoc />
        public void AddOrder(AtomicOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
        }

        /// <inheritdoc />
        public void AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            Debug.KeyNotIn(order.Id, this.cachedOrders, nameof(order.Id), nameof(this.cachedOrders));

            this.redisDatabase.ListRightPush(Key.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent));
            this.redisDatabase.SetAdd(Key.IndexAccountOrders(accountId), order.Id.Value);
            this.redisDatabase.HashSet(Key.IndexOrderTrader, new[] { new HashEntry(order.Id.Value, traderId.Value) });
            this.redisDatabase.HashSet(Key.IndexOrderAccount, new[] { new HashEntry(order.Id.Value, accountId.Value) });
            this.redisDatabase.HashSet(Key.IndexOrderPosition, new[] { new HashEntry(order.Id.Value, positionId.Value) });
            this.redisDatabase.HashSet(Key.IndexOrderStrategy, new[] { new HashEntry(order.Id.Value, strategyId.Value) });
            this.redisDatabase.SetAdd(Key.IndexOrders, order.Id.Value);
            this.redisDatabase.SetAdd(Key.IndexPositionOrders(positionId), order.Id.Value);

            this.cachedOrders[order.Id] = order;
        }

        /// <inheritdoc />
        public void AddPosition(Position position)
        {
            Debug.KeyNotIn(position.Id, this.cachedPositions, nameof(position.Id), nameof(this.cachedPositions));

            this.redisDatabase.ListRightPush(Key.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent));
            this.redisDatabase.SetAdd(Key.IndexAccountPositions(position.AccountId), position.Id.Value);
            this.redisDatabase.SetAdd(Key.IndexPositions, position.Id.Value);
            this.redisDatabase.SetAdd(Key.IndexPositionsOpen, position.Id.Value);

            this.cachedPositions[position.Id] = position;
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            this.redisDatabase.ListRightPush(Key.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent));

            if (order.IsWorking)
            {
                this.redisDatabase.SetAdd(Key.IndexOrdersWorking, order.Id.Value);
                this.redisDatabase.SetRemove(Key.IndexOrdersCompleted, order.Id.Value);
            }
            else if (order.IsCompleted)
            {
                this.redisDatabase.SetAdd(Key.IndexOrdersCompleted, order.Id.Value);
                this.redisDatabase.SetRemove(Key.IndexOrdersWorking, order.Id.Value);
            }
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            this.redisDatabase.ListRightPush(Key.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent));

            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(Key.IndexPositionsOpen, position.Id.Value);
                this.redisDatabase.SetRemove(Key.IndexPositionsClosed, position.Id.Value);
            }
            else if (position.IsClosed)
            {
                this.redisDatabase.SetAdd(Key.IndexPositionsClosed, position.Id.Value);
                this.redisDatabase.SetRemove(Key.IndexPositionsOpen, position.Id.Value);
            }
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            this.redisDatabase.ListRightPush(Key.Account(account.Id), this.eventSerializer.Serialize(account.LastEvent));
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
