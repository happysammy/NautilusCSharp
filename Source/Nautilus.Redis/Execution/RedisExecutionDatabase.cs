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
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Interfaces;
    using StackExchange.Redis;
    using Order = Nautilus.DomainModel.Aggregates.Order;

    /// <summary>
    /// Provides an execution database implemented with Redis.
    /// </summary>
    public class RedisExecutionDatabase : ExecutionDatabase, IExecutionDatabase
    {
        private readonly IServer redisServer;
        private readonly IDatabase redisDatabase;
        private readonly ISerializer<Command> commandSerializer;
        private readonly ISerializer<Event> eventSerializer;

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

            this.cachedOrders = new Dictionary<OrderId, Order>();
            this.cachedPositions = new Dictionary<PositionId, Position>();

            this.OptionLoadCache = optionLoadCache;

            if (this.OptionLoadCache)
            {
                this.Log.Information($"The OptionLoadCache is {this.OptionLoadCache}");
                this.LoadOrdersCache();
                this.LoadPositionsCache();
            }
            else
            {
                this.Log.Warning($"The OptionLoadCache is {this.OptionLoadCache} " +
                                 $"this should only be done in a testing environment.");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the execution database will load the cache on instantiation.
        /// </summary>
        public bool OptionLoadCache { get; }

        /// <summary>
        /// Clear the current order cache and load orders from the database.
        /// </summary>
        public void LoadOrdersCache()
        {
            this.Log.Information("Re-caching orders from the database...");

            this.cachedOrders.Clear();

            var orderKeys = this.redisServer.Keys(pattern: Key.Orders).ToArray();

            if (orderKeys.Length == 0)
            {
                this.Log.Information("No orders found in the database.");
            }

            foreach (var key in orderKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Log.Error($"Cannot load order {key} from the database (no events persisted).");
                    continue;
                }

                var initial = this.eventSerializer.Deserialize(events.Dequeue());
                if (initial.Type != typeof(OrderInitialized))
                {
                    this.Log.Error($"Cannot load order {key} from the database (first event not OrderInitialized, was {initial.Type}).");
                    continue;
                }

                var order = new Order((OrderInitialized)initial);
                do
                {
                    order.Apply((OrderEvent)this.eventSerializer.Deserialize(events.Dequeue()));
                }
                while (events.Count > 0);

                this.cachedOrders[order.Id] = order;
            }

            this.Log.Information($"Cached {this.cachedOrders.Count} orders.");
        }

        /// <summary>
        /// Clear the current order cache and load orders from the database.
        /// </summary>
        public void LoadPositionsCache()
        {
            this.Log.Information("Re-caching positions from the database...");

            this.cachedPositions.Clear();

            var positionKeys = this.redisServer.Keys(pattern: Key.Positions).ToArray();

            if (positionKeys.Length == 0)
            {
                this.Log.Information("No positions found in the database.");
            }

            foreach (var key in positionKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Log.Error($"Cannot load position {key} from the database (no events persisted).");
                    continue;
                }

                var position = new Position(new PositionId(key), (OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue()));
                do
                {
                    position.Apply((OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue()));
                }
                while (events.Count > 0);

                this.cachedPositions[position.Id] = position;
            }

            this.Log.Information($"Cached {this.cachedPositions.Count} positions.");
        }

        /// <inheritdoc />
        public CommandResult AddAtomicOrder(AtomicOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            this.AddOrder(
                order.Entry,
                traderId,
                accountId,
                strategyId,
                positionId).OnFailure(errorMsg => { CommandResult.Fail(errorMsg); });

            this.AddOrder(
                order.StopLoss,
                traderId,
                accountId,
                strategyId,
                positionId).OnFailure(errorMsg => { CommandResult.Fail(errorMsg); });

            if (order.TakeProfit != null)
            {
                this.AddOrder(
                    order.TakeProfit,
                    traderId,
                    accountId,
                    strategyId,
                    positionId).OnFailure(errorMsg => { CommandResult.Fail(errorMsg); });
            }

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            if (this.cachedOrders.ContainsKey(order.Id))
            {
                return CommandResult.Fail($"The {order.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(Key.IndexTraderOrders(traderId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexTraderPositions(traderId), positionId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexTraderStrategies(traderId), strategyId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexAccountOrders(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexAccountPositions(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexOrderTrader, new[] { new HashEntry(order.Id.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexOrderAccount, new[] { new HashEntry(order.Id.Value, accountId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexOrderPosition, new[] { new HashEntry(order.Id.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexOrderStrategy, new[] { new HashEntry(order.Id.Value, strategyId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexPositionTrader, new[] { new HashEntry(positionId.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexPositionAccount, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(Key.IndexPositionStrategy, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexPositionOrders(positionId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(Key.IndexOrders, order.Id.Value, CommandFlags.FireAndForget);

            this.redisDatabase.ListRightPush(Key.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.cachedOrders[order.Id] = order;

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddPosition(Position position)
        {
            if (this.cachedPositions.ContainsKey(position.Id))
            {
                return CommandResult.Fail($"The {position.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(Key.IndexPositions, position.Id.Value, CommandFlags.FireAndForget);
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(Key.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
            }
            else
            {
                // The position should always be open when being added
                this.Log.Error($"The added {position} was not open.");
            }

            this.redisDatabase.ListRightPush(Key.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.cachedPositions[position.Id] = position;

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            if (order.IsWorking)
            {
                this.redisDatabase.SetAdd(Key.IndexOrdersWorking, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(Key.IndexOrdersCompleted, order.Id.Value, CommandFlags.FireAndForget);
            }
            else if (order.IsCompleted)
            {
                this.redisDatabase.SetAdd(Key.IndexOrdersCompleted, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(Key.IndexOrdersWorking, order.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(Key.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(Key.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(Key.IndexPositionsClosed, position.Id.Value, CommandFlags.FireAndForget);
            }
            else if (position.IsClosed)
            {
                this.redisDatabase.SetAdd(Key.IndexPositionsClosed, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(Key.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(Key.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            this.redisDatabase.ListRightPush(Key.Account(account.Id), this.eventSerializer.Serialize(account.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void CheckResiduals()
        {
            foreach (var orderId in this.GetOrderWorkingIds())
            {
                this.GetOrder(orderId);  // Check working
                this.Log.Warning($"The {orderId} is still working.");
            }

            foreach (var positionId in this.GetPositionOpenIds())
            {
                this.GetPosition(positionId);  // Check open
                this.Log.Warning($"The {positionId} is still open.");
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            this.CachedAccounts.Clear();
            this.CachedOrders.Clear();
            this.CachedPositions.Clear();
        }

        /// <inheritdoc />
        public void Flush()
        {
            this.Log.Information("Flushing the database...");
            this.redisServer.FlushDatabase();
            this.Log.Information("Database flushed...");
        }

        /// <inheritdoc />
        public override TraderId? GetTraderForOrder(OrderId orderId)
        {
            var traderId = this.redisDatabase.HashGet(Key.IndexOrderTrader, orderId.Value);
            if (traderId == RedisValue.Null)
            {
                this.Log.Warning($"Cannot find TraderId for {orderId}.");
                return null;
            }

            return TraderId.FromString(traderId);
        }

        /// <inheritdoc />
        public override ICollection<TraderId> GetTraderIds()
        {
            return SetFactory.ConvertToSet(this.redisServer.Keys(pattern: Key.Traders).ToArray(), TraderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<AccountId> GetAccountIds()
        {
            return SetFactory.ConvertToSet(this.redisServer.Keys(pattern: Key.Accounts).ToArray(), AccountId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexTraderStrategies(traderId)), StrategyId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexOrders), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(Key.IndexTraderOrders(traderId))
                : this.GetIntersection(Key.IndexOrders, Key.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexOrdersWorking), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(Key.IndexOrdersWorking, Key.IndexTraderOrders(traderId))
                : this.GetIntersection(Key.IndexOrdersWorking, Key.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexOrdersCompleted), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(Key.IndexOrdersCompleted, Key.IndexTraderOrders(traderId))
                : this.GetIntersection(Key.IndexOrdersCompleted, Key.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexPositions), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(Key.IndexTraderPositions(traderId))
                : this.GetIntersection(Key.IndexPositions, Key.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexPositionsOpen), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(Key.IndexPositionsOpen, Key.IndexTraderPositions(traderId))
                : this.GetIntersection(Key.IndexPositionsOpen, Key.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(Key.IndexPositionsClosed), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(Key.IndexPositionsClosed, Key.IndexTraderPositions(traderId))
                : this.GetIntersection(Key.IndexPositionsClosed, Key.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(OrderId orderId)
        {
            var idValue = this.redisDatabase.HashGet(Key.IndexOrderPosition, orderId.Value);

            if (idValue == RedisValue.Null)
            {
                this.Log.Warning($"Cannot find PositionId for {orderId}.");
                return null;
            }

            return new PositionId(idValue);
        }

        private RedisValue[] GetIntersection(string setKey1, string setKey2)
        {
            return this.redisDatabase.SetCombine(SetOperation.Intersect, setKey1, setKey2);
        }
    }
}
