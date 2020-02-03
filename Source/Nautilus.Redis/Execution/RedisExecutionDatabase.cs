// -------------------------------------------------------------------------------------------------
// <copyright file="RedisExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Interfaces;
    using Nautilus.Redis.Execution.Internal;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisExecutionDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="connection">The redis connection multiplexer.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="loadCaches">The option flag to load caches from Redis on instantiation.</param>
        public RedisExecutionDatabase(
            IComponentryContainer container,
            ConnectionMultiplexer connection,
            ISerializer<Command> commandSerializer,
            ISerializer<Event> eventSerializer,
            bool loadCaches = true)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.commandSerializer = commandSerializer;
            this.eventSerializer = eventSerializer;

            this.OptionLoadCaches = loadCaches;

            if (this.OptionLoadCaches)
            {
                this.Log.Information($"{nameof(this.OptionLoadCaches)} is {this.OptionLoadCaches}");
                this.LoadCaches();
            }
            else
            {
                this.Log.Warning($"{nameof(this.OptionLoadCaches)} is {this.OptionLoadCaches} " +
                                 $"(this should only be done in a testing environment).");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the execution database will load the caches on instantiation.
        /// </summary>
        public bool OptionLoadCaches { get; }

        /// <inheritdoc />
        public override void LoadAccountsCache()
        {
            this.Log.Debug("Re-caching accounts from the database...");

            this.CachedAccounts.Clear();

            var accountKeys = this.redisServer.Keys(pattern: KeyProvider.Accounts).ToArray();
            if (accountKeys.Length == 0)
            {
                this.Log.Information("No accounts found in the database.");
                return;
            }

            foreach (var key in accountKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Log.Error($"Cannot load account {key} from the database (no events persisted).");
                    continue;
                }

                var initial = this.eventSerializer.Deserialize(events.Dequeue());
                if (initial.Type != typeof(AccountStateEvent))
                {
                    this.Log.Error($"Cannot load account {key} from the database (event not AccountStateEvent, was {initial.Type}).");
                    continue;
                }

                var account = new Account((AccountStateEvent)initial);
                this.CachedAccounts[account.Id] = account;
            }

            foreach (var kvp in this.CachedAccounts)
            {
                this.Log.Information($"Cached {kvp.Key}.");
            }
        }

        /// <inheritdoc />
        public override void LoadOrdersCache()
        {
            this.Log.Debug("Re-caching orders from the database...");

            this.CachedOrders.Clear();

            var orderKeys = this.redisServer.Keys(pattern: KeyProvider.Orders).ToArray();
            if (orderKeys.Length == 0)
            {
                this.Log.Information("No orders found in the database.");
                return;
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
                while (events.Count > 0)
                {
                    var nextEvent = (OrderEvent)this.eventSerializer.Deserialize(events.Dequeue());
                    if (nextEvent is null)
                    {
                        this.Log.Error("Could not deserialize OrderEvent.");
                        continue;
                    }

                    order.Apply(nextEvent);
                }

                this.CachedOrders[order.Id] = order;
            }

            this.Log.Information($"Cached {this.CachedOrders.Count} order(s).");
        }

        /// <inheritdoc />
        public override void LoadPositionsCache()
        {
            this.Log.Debug("Re-caching positions from the database...");

            this.CachedPositions.Clear();

            var positionKeys = this.redisServer.Keys(pattern: KeyProvider.Positions).ToArray();
            if (positionKeys.Length == 0)
            {
                this.Log.Information("No positions found in the database.");
                return;
            }

            foreach (var key in positionKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Log.Error($"Cannot load position {key} from the database (no events persisted).");
                    continue;
                }

                var position = new Position(
                    new PositionId(key.ToString().Split(':')[^1]),
                    (OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue()));
                while (events.Count > 0)
                {
                    var nextEvent = (OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue());
                    if (nextEvent is null)
                    {
                        this.Log.Error("Could not deserialize OrderFillEvent.");
                        continue;
                    }

                    position.Apply(nextEvent);
                }

                this.CachedPositions[position.Id] = position;
            }

            this.Log.Information($"Cached {this.CachedPositions.Count} position(s).");
        }

        /// <inheritdoc />
        public override void Flush()
        {
            this.ClearCaches();

            this.Log.Debug("Flushing database...");
            this.redisServer.FlushDatabase();
            this.Log.Information("Database flushed.");
        }

        /// <inheritdoc />
        public CommandResult AddAtomicOrder(AtomicOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            var resultEntry = this.AddOrder(
                order.Entry,
                traderId,
                accountId,
                strategyId,
                positionId);
            if (resultEntry.IsFailure)
            {
                return resultEntry;
            }

            var resultStopLoss = this.AddOrder(
                order.StopLoss,
                traderId,
                accountId,
                strategyId,
                positionId);
            if (resultStopLoss.IsFailure)
            {
                return resultStopLoss;
            }

            if (order.TakeProfit != null)
            {
                var resultTakeProfit = this.AddOrder(
                    order.TakeProfit,
                    traderId,
                    accountId,
                    strategyId,
                    positionId);
                if (resultTakeProfit.IsFailure)
                {
                    return resultTakeProfit;
                }
            }

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddAccount(Account account)
        {
            if (this.CachedAccounts.ContainsKey(account.Id))
            {
                return CommandResult.Fail($"The {account.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.ListRightPush(KeyProvider.Account(account.Id), this.eventSerializer.Serialize(account.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedAccounts[account.Id] = account;

            this.Log.Debug($"Added Account(Id={account.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            if (this.CachedOrders.ContainsKey(order.Id))
            {
                return CommandResult.Fail($"The {order.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(KeyProvider.IndexTraders, traderId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderOrders(traderId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderPositions(traderId), positionId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategies(traderId), strategyId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategyOrders(traderId, strategyId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategyPositions(traderId, strategyId), positionId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexAccountOrders(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexAccountPositions(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderTrader, new[] { new HashEntry(order.Id.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderAccount, new[] { new HashEntry(order.Id.Value, accountId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderPosition, new[] { new HashEntry(order.Id.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderStrategy, new[] { new HashEntry(order.Id.Value, strategyId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionTrader, new[] { new HashEntry(positionId.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionAccount, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionStrategy, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexPositionOrders(positionId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexOrders, order.Id.Value, CommandFlags.FireAndForget);

            this.redisDatabase.ListRightPush(KeyProvider.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedOrders[order.Id] = order;

            this.Log.Debug($"Added Order(Id={order.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddPosition(Position position)
        {
            if (this.CachedPositions.ContainsKey(position.Id))
            {
                return CommandResult.Fail($"The {position.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(KeyProvider.IndexPositions, position.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionBrokerId, new[] { new HashEntry(position.Id.Value, position.IdBroker.Value) }, CommandFlags.FireAndForget);
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
            }
            else
            {
                // The position should always be open when being added
                this.Log.Error($"The added {position} was not open.");
            }

            this.redisDatabase.HashSet(KeyProvider.IndexBrokerIdPosition(position.AccountId), new[] { new HashEntry(position.IdBroker.Value, position.Id.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.ListRightPush(KeyProvider.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedPositions[position.Id] = position;

            this.Log.Debug($"Added Position(Id={position.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            this.redisDatabase.ListRightPush(KeyProvider.Account(account.Id), this.eventSerializer.Serialize(account.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            if (order.IsWorking)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexOrdersWorking, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexOrdersCompleted, order.Id.Value, CommandFlags.FireAndForget);
            }
            else if (order.IsCompleted)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexOrdersCompleted, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexOrdersWorking, order.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(KeyProvider.Order(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexPositionsClosed, position.Id.Value, CommandFlags.FireAndForget);
            }
            else if (position.IsClosed)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsClosed, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexPositionsOpen, position.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(KeyProvider.Position(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(OrderId orderId)
        {
            var traderId = this.redisDatabase.HashGet(KeyProvider.IndexOrderTrader, orderId.Value);
            return traderId == RedisValue.Null
                ? null
                : TraderId.FromString(traderId);
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(PositionId positionId)
        {
            var traderId = this.redisDatabase.HashGet(KeyProvider.IndexPositionTrader, positionId.Value);
            return traderId == RedisValue.Null
                ? null
                : TraderId.FromString(traderId);
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(OrderId orderId)
        {
            var accountId = this.redisDatabase.HashGet(KeyProvider.IndexOrderAccount, orderId.Value);
            return accountId == RedisValue.Null
                ? null
                : AccountId.FromString(accountId);
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(PositionId positionId)
        {
            var accountId = this.redisDatabase.HashGet(KeyProvider.IndexPositionAccount, positionId.Value);
            return accountId == RedisValue.Null
                ? null
                : AccountId.FromString(accountId);
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(OrderId orderId)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexOrderPosition, orderId.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionId(idValue);
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(AccountId accountId, PositionIdBroker positionIdBroker)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexBrokerIdPosition(accountId), positionIdBroker.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionId(idValue);
        }

        /// <inheritdoc />
        public override PositionIdBroker? GetPositionIdBroker(PositionId positionId)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexPositionBrokerId, positionId.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionIdBroker(idValue);
        }

        /// <inheritdoc />
        public override ICollection<TraderId> GetTraderIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexTraders).ToArray(), TraderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<AccountId> GetAccountIds()
        {
            var accountIds = this.redisServer.Keys(pattern: KeyProvider.Accounts)
                .Select(k => k.ToString().Split(':')[^1])
                .ToArray();

            return SetFactory.ConvertToSet(accountIds, AccountId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategies(traderId)), StrategyId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrders), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(KeyProvider.IndexTraderOrders(traderId))
                : this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrdersWorking), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexOrdersWorking, KeyProvider.IndexTraderOrders(traderId))
                : this.GetIntersection(KeyProvider.IndexOrdersWorking, KeyProvider.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrdersCompleted), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexOrdersCompleted, KeyProvider.IndexTraderOrders(traderId))
                : this.GetIntersection(KeyProvider.IndexOrdersCompleted, KeyProvider.IndexTraderStrategyOrders(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositions), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(KeyProvider.IndexTraderPositions(traderId))
                : this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositionsOpen), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexPositionsOpen, KeyProvider.IndexTraderPositions(traderId))
                : this.GetIntersection(KeyProvider.IndexPositionsOpen, KeyProvider.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositionsClosed), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexPositionsClosed, KeyProvider.IndexTraderPositions(traderId))
                : this.GetIntersection(KeyProvider.IndexPositionsClosed, KeyProvider.IndexTraderStrategyPositions(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        private RedisValue[] GetIntersection(string setKey1, string setKey2)
        {
            return this.redisDatabase.SetCombine(SetOperation.Intersect, setKey1, setKey2);
        }
    }
}
