// -------------------------------------------------------------------------------------------------
// <copyright file="RedisExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
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
using NodaTime;
using StackExchange.Redis;
using Order = Nautilus.DomainModel.Aggregates.Order;

namespace Nautilus.Redis.Execution
{
    /// <summary>
    /// Provides an execution database implemented with Redis.
    /// </summary>
    public sealed class RedisExecutionDatabase : ExecutionDatabase, IExecutionDatabase
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
        /// <param name="orderStatusCheckInterval">The minutes interval between order status checks.</param>
        public RedisExecutionDatabase(
            IComponentryContainer container,
            ConnectionMultiplexer connection,
            ISerializer<Command> commandSerializer,
            ISerializer<Event> eventSerializer,
            bool loadCaches = true,
            int orderStatusCheckInterval = 1)
            : base(container)
        {
            this.redisServer = connection.GetServer(RedisConstants.Localhost, RedisConstants.DefaultPort);
            this.redisDatabase = connection.GetDatabase();
            this.commandSerializer = commandSerializer;
            this.eventSerializer = eventSerializer;

            this.IsCacheLoadedOnStart = loadCaches;
            this.OrderStatusCheckInterval = Duration.FromMinutes(orderStatusCheckInterval);

            if (this.IsCacheLoadedOnStart)
            {
                this.Logger.LogInformation($"{nameof(this.IsCacheLoadedOnStart)} is {this.IsCacheLoadedOnStart}");
                this.LoadCaches();
            }
            else
            {
                this.Logger.LogWarning(
                    LogId.Database,
                    $"{nameof(this.IsCacheLoadedOnStart)} is {this.IsCacheLoadedOnStart} (this should only be done in a testing environment).");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the execution database will load the caches on instantiation.
        /// </summary>
        public bool IsCacheLoadedOnStart { get; }

        /// <summary>
        /// Gets the duration between order status checks. Zero duration indicates no status checking.
        /// </summary>
        public Duration OrderStatusCheckInterval { get; }

        /// <inheritdoc />
        public override void LoadAccountsCache()
        {
            this.Logger.LogDebug(LogId.Database, "Re-caching accounts from the database...");

            this.CachedAccounts.Clear();

            var accountKeys = this.redisServer.Keys(pattern: KeyProvider.AccountsKey).ToArray();
            if (accountKeys.Length == 0)
            {
                this.Logger.LogInformation(LogId.Database, "No accounts found in the database.");
                return;
            }

            foreach (var key in accountKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    var errorMsg = $"Cannot load account {key} from the database (no events persisted).";
                    this.Logger.LogError(LogId.Database, errorMsg);
                    continue;
                }

                var initial = this.eventSerializer.Deserialize(events.Dequeue());
                if (initial.Type != typeof(AccountStateEvent))
                {
                    var errorMsg = $"Cannot load account {key} from the database " +
                                         $"(event not AccountStateEvent, was {initial.Type}).";
                    this.Logger.LogError(LogId.Database, errorMsg);
                    continue;
                }

                var account = new Account((AccountStateEvent)initial);
                this.CachedAccounts[account.Id] = account;
            }

            foreach (var kvp in this.CachedAccounts)
            {
                this.Logger.LogInformation(LogId.Database, $"Cached {kvp.Key}.");
            }
        }

        /// <inheritdoc />
        public override void LoadOrdersCache()
        {
            this.Logger.LogDebug(LogId.Database, "Re-caching orders from the database...");

            this.CachedOrders.Clear();

            var orderKeys = this.redisServer.Keys(pattern: KeyProvider.OrdersKey).ToArray();
            if (orderKeys.Length == 0)
            {
                this.Logger.LogInformation(LogId.Database, "No orders found in the database.");
                return;
            }

            foreach (var key in orderKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Logger.LogError(LogId.Database, $"Cannot load order {key} from the database (no events persisted).");
                    continue;
                }

                var initial = this.eventSerializer.Deserialize(events.Dequeue());
                if (initial.Type != typeof(OrderInitialized))
                {
                    this.Logger.LogError(LogId.Database, $"Cannot load order {key} from the database (first event not OrderInitialized, was {initial.Type}).");
                    continue;
                }

                var order = new Order((OrderInitialized)initial);
                while (events.Count > 0)
                {
                    var nextEvent = (OrderEvent)this.eventSerializer.Deserialize(events.Dequeue());
                    order.Apply(nextEvent);
                }

                this.CachedOrders[order.Id] = order;
            }

            this.Logger.LogInformation(LogId.Database, $"Cached {this.CachedOrders.Count} order(s).");
        }

        /// <inheritdoc />
        public override void LoadPositionsCache()
        {
            this.Logger.LogDebug(LogId.Database, "Re-caching positions from the database...");

            this.CachedPositions.Clear();

            var positionKeys = this.redisServer.Keys(pattern: KeyProvider.PositionsKey).ToArray();
            if (positionKeys.Length == 0)
            {
                this.Logger.LogInformation(LogId.Database, "No positions found in the database.");
                return;
            }

            foreach (var key in positionKeys)
            {
                var events = new Queue<RedisValue>(this.redisDatabase.ListRange(key));
                if (events.Count == 0)
                {
                    this.Logger.LogError(LogId.Database, $"Cannot load position {key} from the database (no events persisted).");
                    continue;
                }

                var position = new Position(
                    new PositionId(key.ToString().Split(':')[^1]),
                    (OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue()));
                while (events.Count > 0)
                {
                    var nextEvent = (OrderFillEvent)this.eventSerializer.Deserialize(events.Dequeue());
                    position.Apply(nextEvent);
                }

                this.CachedPositions[position.Id] = position;
            }

            this.Logger.LogInformation(LogId.Database, $"Cached {this.CachedPositions.Count} position(s).");
        }

        /// <inheritdoc />
        public override void Flush()
        {
            this.ClearCaches();

            this.Logger.LogDebug(LogId.Database, "Flushing database...");
            this.redisServer.FlushDatabase();
            this.Logger.LogInformation(LogId.Database, "Database flushed.");
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

            this.redisDatabase.ListRightPush(KeyProvider.AccountKey(account.Id), this.eventSerializer.Serialize(account.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedAccounts[account.Id] = account;

            this.Logger.LogDebug($"Added Account(Id={account.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            if (this.CachedOrders.ContainsKey(order.Id))
            {
                return CommandResult.Fail($"The {order.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(KeyProvider.IndexTradersKey, traderId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderOrdersKey(traderId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderPositionsKey(traderId), positionId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategiesKey(traderId), strategyId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategyOrdersKey(traderId, strategyId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexTraderStrategyPositionsKey(traderId, strategyId), positionId.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexAccountOrdersKey(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexAccountPositionsKey(accountId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderTraderKey, new[] { new HashEntry(order.Id.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderAccountKey, new[] { new HashEntry(order.Id.Value, accountId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderPositionKey, new[] { new HashEntry(order.Id.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexOrderStrategyKey, new[] { new HashEntry(order.Id.Value, strategyId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionTraderKey, new[] { new HashEntry(positionId.Value, traderId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionAccountKey, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionStrategyKey, new[] { new HashEntry(positionId.Value, positionId.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexPositionOrdersKey(positionId), order.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.SetAdd(KeyProvider.IndexOrdersKey, order.Id.Value, CommandFlags.FireAndForget);

            this.redisDatabase.ListRightPush(KeyProvider.OrderKey(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedOrders[order.Id] = order;

            this.Logger.LogDebug(LogId.Database, $"Added Order(Id={order.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddPosition(Position position)
        {
            if (this.CachedPositions.ContainsKey(position.Id))
            {
                return CommandResult.Fail($"The {position.Id} already existed in the cache (was not unique).");
            }

            this.redisDatabase.SetAdd(KeyProvider.IndexPositionsKey, position.Id.Value, CommandFlags.FireAndForget);
            this.redisDatabase.HashSet(KeyProvider.IndexPositionBrokerIdKey, new[] { new HashEntry(position.Id.Value, position.IdBroker.Value) }, CommandFlags.FireAndForget);
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsOpenKey, position.Id.Value, CommandFlags.FireAndForget);
            }
            else
            {
                // The position should always be open when being added
                this.Logger.LogError(LogId.Database, $"The added {position} was not open.");
            }

            this.redisDatabase.HashSet(KeyProvider.IndexBrokerIdPositionKey(position.AccountId), new[] { new HashEntry(position.IdBroker.Value, position.Id.Value) }, CommandFlags.FireAndForget);
            this.redisDatabase.ListRightPush(KeyProvider.PositionKey(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);

            this.CachedPositions[position.Id] = position;

            this.Logger.LogDebug(LogId.Database, $"Added Position(Id={position.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            this.redisDatabase.ListRightPush(KeyProvider.AccountKey(account.Id), this.eventSerializer.Serialize(account.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            if (order.IsWorking)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexOrdersWorkingKey, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexOrdersCompletedKey, order.Id.Value, CommandFlags.FireAndForget);
            }
            else if (order.IsCompleted)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexOrdersCompletedKey, order.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexOrdersWorkingKey, order.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(KeyProvider.OrderKey(order.Id), this.eventSerializer.Serialize(order.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            if (position.IsOpen)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsOpenKey, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexPositionsClosedKey, position.Id.Value, CommandFlags.FireAndForget);
            }
            else if (position.IsClosed)
            {
                this.redisDatabase.SetAdd(KeyProvider.IndexPositionsClosedKey, position.Id.Value, CommandFlags.FireAndForget);
                this.redisDatabase.SetRemove(KeyProvider.IndexPositionsOpenKey, position.Id.Value, CommandFlags.FireAndForget);
            }

            this.redisDatabase.ListRightPush(KeyProvider.PositionKey(position.Id), this.eventSerializer.Serialize(position.LastEvent), When.Always, CommandFlags.FireAndForget);
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(OrderId orderId)
        {
            var traderId = this.redisDatabase.HashGet(KeyProvider.IndexOrderTraderKey, orderId.Value);
            return traderId == RedisValue.Null
                ? null
                : TraderId.FromString(traderId);
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(PositionId positionId)
        {
            var traderId = this.redisDatabase.HashGet(KeyProvider.IndexPositionTraderKey, positionId.Value);
            return traderId == RedisValue.Null
                ? null
                : TraderId.FromString(traderId);
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(OrderId orderId)
        {
            var accountId = this.redisDatabase.HashGet(KeyProvider.IndexOrderAccountKey, orderId.Value);
            return accountId == RedisValue.Null
                ? null
                : AccountId.FromString(accountId);
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(PositionId positionId)
        {
            var accountId = this.redisDatabase.HashGet(KeyProvider.IndexPositionAccountKey, positionId.Value);
            return accountId == RedisValue.Null
                ? null
                : AccountId.FromString(accountId);
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(OrderId orderId)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexOrderPositionKey, orderId.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionId(idValue);
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(AccountId accountId, PositionIdBroker positionIdBroker)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexBrokerIdPositionKey(accountId), positionIdBroker.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionId(idValue);
        }

        /// <inheritdoc />
        public override PositionIdBroker? GetPositionIdBroker(PositionId positionId)
        {
            var idValue = this.redisDatabase.HashGet(KeyProvider.IndexPositionBrokerIdKey, positionId.Value);
            return idValue == RedisValue.Null
                ? null
                : new PositionIdBroker(idValue);
        }

        /// <inheritdoc />
        public override ICollection<TraderId> GetTraderIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexTradersKey).ToArray(), TraderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<AccountId> GetAccountIds()
        {
            var accountIds = this.redisServer.Keys(pattern: KeyProvider.AccountsKey)
                .Select(k => k.ToString().Split(':')[^1])
                .ToArray();

            return SetFactory.ConvertToSet(accountIds, AccountId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategiesKey(traderId)), StrategyId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrdersKey), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(KeyProvider.IndexTraderOrdersKey(traderId))
                : this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategyOrdersKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrdersWorkingKey), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexOrdersWorkingKey, KeyProvider.IndexTraderOrdersKey(traderId))
                : this.GetIntersection(KeyProvider.IndexOrdersWorkingKey, KeyProvider.IndexTraderStrategyOrdersKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexOrdersCompletedKey), OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexOrdersCompletedKey, KeyProvider.IndexTraderOrdersKey(traderId))
                : this.GetIntersection(KeyProvider.IndexOrdersCompletedKey, KeyProvider.IndexTraderStrategyOrdersKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(orderIdValues, OrderId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositionsKey), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.redisDatabase.SetMembers(KeyProvider.IndexTraderPositionsKey(traderId))
                : this.redisDatabase.SetMembers(KeyProvider.IndexTraderStrategyPositionsKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositionsOpenKey), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexPositionsOpenKey, KeyProvider.IndexTraderPositionsKey(traderId))
                : this.GetIntersection(KeyProvider.IndexPositionsOpenKey, KeyProvider.IndexTraderStrategyPositionsKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds()
        {
            return SetFactory.ConvertToSet(this.redisDatabase.SetMembers(KeyProvider.IndexPositionsClosedKey), PositionId.FromString);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIdValues = filterStrategyId is null
                ? this.GetIntersection(KeyProvider.IndexPositionsClosedKey, KeyProvider.IndexTraderPositionsKey(traderId))
                : this.GetIntersection(KeyProvider.IndexPositionsClosedKey, KeyProvider.IndexTraderStrategyPositionsKey(traderId, filterStrategyId));

            return SetFactory.ConvertToSet(positionIdValues, PositionId.FromString);
        }

        private RedisValue[] GetIntersection(string setKey1, string setKey2)
        {
            return this.redisDatabase.SetCombine(SetOperation.Intersect, setKey1, setKey2);
        }
    }
}
