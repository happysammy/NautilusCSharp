// -------------------------------------------------------------------------------------------------
// <copyright file="InMemoryExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
using Nautilus.Core.Correctness;
using Nautilus.Core.CQS;
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.Execution.Interfaces;

namespace Nautilus.Execution.Engine
{
    /// <summary>
    /// Provides an in-memory execution database.
    /// </summary>
    public sealed class InMemoryExecutionDatabase : ExecutionDatabase, IExecutionDatabase
    {
        private readonly Dictionary<OrderId, TraderId> indexOrderTrader;
        private readonly Dictionary<OrderId, AccountId> indexOrderAccount;
        private readonly Dictionary<OrderId, PositionId> indexOrderPosition;
        private readonly Dictionary<PositionId, TraderId> indexPositionTrader;
        private readonly Dictionary<PositionId, AccountId> indexPositionAccount;
        private readonly Dictionary<PositionIdBroker, PositionId> indexBrokerPosition;
        private readonly Dictionary<PositionId, PositionIdBroker> indexPositionBroker;
        private readonly Dictionary<PositionId, HashSet<OrderId>> indexPositionOrders;
        private readonly Dictionary<AccountId, HashSet<OrderId>> indexAccountOrders;
        private readonly Dictionary<AccountId, HashSet<PositionId>> indexAccountPositions;

        private readonly Dictionary<TraderId, TraderIdentifierIndex> indexTraders;
        private readonly HashSet<AccountId> indexAccounts;
        private readonly HashSet<OrderId> indexOrders;
        private readonly HashSet<OrderId> indexOrdersWorking;
        private readonly HashSet<OrderId> indexOrdersCompleted;
        private readonly HashSet<PositionId> indexPositions;
        private readonly HashSet<PositionId> indexPositionsOpen;
        private readonly HashSet<PositionId> indexPositionsClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryExecutionDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public InMemoryExecutionDatabase(IComponentryContainer container)
            : base(container)
        {
            this.indexOrderTrader = new Dictionary<OrderId, TraderId>();
            this.indexOrderAccount = new Dictionary<OrderId, AccountId>();
            this.indexOrderPosition = new Dictionary<OrderId, PositionId>();
            this.indexPositionTrader = new Dictionary<PositionId, TraderId>();
            this.indexPositionAccount = new Dictionary<PositionId, AccountId>();
            this.indexPositionBroker = new Dictionary<PositionId, PositionIdBroker>();
            this.indexBrokerPosition = new Dictionary<PositionIdBroker, PositionId>();
            this.indexPositionOrders = new Dictionary<PositionId, HashSet<OrderId>>();
            this.indexAccountOrders = new Dictionary<AccountId, HashSet<OrderId>>();
            this.indexAccountPositions = new Dictionary<AccountId, HashSet<PositionId>>();

            this.indexTraders = new Dictionary<TraderId, TraderIdentifierIndex>();
            this.indexAccounts = new HashSet<AccountId>();
            this.indexOrders = new HashSet<OrderId>();
            this.indexOrdersWorking = new HashSet<OrderId>();
            this.indexOrdersCompleted = new HashSet<OrderId>();
            this.indexPositions = new HashSet<PositionId>();
            this.indexPositionsOpen = new HashSet<PositionId>();
            this.indexPositionsClosed = new HashSet<PositionId>();
        }

        /// <inheritdoc />
        public override void LoadAccountsCache()
        {
            this.Logger.LogInformation(LogId.Database, "Re-caching accounts from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void LoadOrdersCache()
        {
            this.Logger.LogInformation(LogId.Database, "Re-caching orders from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void LoadPositionsCache()
        {
            this.Logger.LogInformation(LogId.Database, "Re-caching positions from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void Flush()
        {
            this.Logger.LogInformation(LogId.Database, "Flushing the database...");

            this.ClearCaches();

            foreach (var traderIndex in this.indexTraders.Values)
            {
                traderIndex.Clear();
            }

            this.indexOrderTrader.Clear();
            this.indexOrderAccount.Clear();
            this.indexOrderPosition.Clear();
            this.indexPositionTrader.Clear();
            this.indexPositionAccount.Clear();
            this.indexBrokerPosition.Clear();
            this.indexPositionOrders.Clear();
            this.indexAccountOrders.Clear();
            this.indexAccountPositions.Clear();
            this.indexTraders.Clear();
            this.indexAccounts.Clear();
            this.indexOrders.Clear();
            this.indexOrdersWorking.Clear();
            this.indexOrdersCompleted.Clear();
            this.indexPositions.Clear();
            this.indexPositionsOpen.Clear();
            this.indexPositionsClosed.Clear();

            this.Logger.LogInformation(LogId.Database, "Database flushed.");
        }

        /// <inheritdoc />
        public CommandResult AddBracketOrder(BracketOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
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

            Debug.NotIn(account.Id, this.indexAccounts, nameof(account.Id), nameof(this.indexAccounts));

            this.indexAccounts.Add(account.Id);

            this.CachedAccounts[account.Id] = account;

            this.Logger.LogDebug(LogId.Database, $"Added Account(Id={account.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public CommandResult AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            if (this.CachedOrders.ContainsKey(order.Id))
            {
                return CommandResult.Fail($"The {order.Id} already existed in the cache (was not unique).");
            }

            Debug.KeyNotIn(order.Id, this.indexOrderTrader, nameof(order.Id), nameof(this.indexOrderTrader));
            Debug.KeyNotIn(order.Id, this.indexOrderAccount, nameof(order.Id), nameof(this.indexOrderAccount));
            Debug.KeyNotIn(order.Id, this.indexOrderPosition, nameof(order.Id), nameof(this.indexOrderPosition));
            Debug.NotIn(order.Id, this.indexOrders, nameof(order.Id), nameof(this.indexOrders));

            this.indexAccounts.Add(accountId);
            this.indexOrderTrader[order.Id] = traderId;
            this.indexOrderAccount[order.Id] = accountId;
            this.indexOrderPosition[order.Id] = positionId;
            this.indexPositionTrader[positionId] = traderId;
            this.indexPositionAccount[positionId] = accountId;

            // PositionId -> Set[OrderId]
            if (this.indexPositionOrders.TryGetValue(positionId, out var positionOrders))
            {
                positionOrders.Add(order.Id);
            }
            else
            {
                this.indexPositionOrders[positionId] = new HashSet<OrderId> { order.Id };
            }

            // AccountId -> Set[OrderId]
            if (this.indexAccountOrders.TryGetValue(accountId, out var accountOrders))
            {
                accountOrders.Add(order.Id);
            }
            else
            {
                this.indexAccountOrders[accountId] = new HashSet<OrderId> { order.Id };
            }

            // AccountId -> Set[PositionId]
            if (this.indexAccountPositions.TryGetValue(accountId, out var accountPositions))
            {
                accountPositions.Add(positionId);
            }
            else
            {
                this.indexAccountPositions[accountId] = new HashSet<PositionId> { positionId };
            }

            // TraderId -> TraderIdentifierIndex
            if (this.indexTraders.TryGetValue(traderId, out var traderIndex))
            {
                traderIndex.AddIdentifiers(order.Id, positionId, strategyId);
            }
            else
            {
                this.indexTraders[traderId] = new TraderIdentifierIndex();
                this.indexTraders[traderId].AddIdentifiers(order.Id, positionId, strategyId);
            }

            this.indexOrders.Add(order.Id);
            this.CachedOrders[order.Id] = order;

            this.Logger.LogDebug(LogId.Database, $"Added Order(Id={order.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">If the position identifier is already indexed.</exception>
        public CommandResult AddPosition(Position position)
        {
            if (this.CachedPositions.ContainsKey(position.Id))
            {
                return CommandResult.Fail($"The {position.Id} already existed in the cache (was not unique).");
            }

            Debug.NotIn(position.Id, this.indexPositions, nameof(position.Id), nameof(this.indexPositions));
            Debug.NotIn(position.Id, this.indexPositionsOpen, nameof(position.Id), nameof(this.indexPositions));

            this.indexPositions.Add(position.Id);
            this.indexPositionsOpen.Add(position.Id);
            this.indexBrokerPosition[position.IdBroker] = position.Id;
            this.indexPositionBroker[position.Id] = position.IdBroker;
            this.CachedPositions[position.Id] = position;

            this.Logger.LogDebug(LogId.Database, $"Added Position(Id={position.Id.Value}).");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        public void UpdateOrder(Order order)
        {
            if (order.IsWorking)
            {
                this.indexOrdersWorking.Add(order.Id);
                this.indexOrdersCompleted.Remove(order.Id);
            }
            else if (order.IsCompleted)
            {
                this.indexOrdersCompleted.Add(order.Id);
                this.indexOrdersWorking.Remove(order.Id);
            }
        }

        /// <inheritdoc />
        public void UpdatePosition(Position position)
        {
            if (position.IsClosed)
            {
                this.indexPositionsClosed.Add(position.Id);
                this.indexPositionsOpen.Remove(position.Id);
            }
        }

        /// <inheritdoc />
        public void UpdateAccount(Account account)
        {
            // Do nothing in memory
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(OrderId orderId)
        {
            if (this.indexOrderTrader.TryGetValue(orderId, out var traderId))
            {
                return traderId;
            }

            this.Logger.LogWarning(LogId.Database, $"Cannot find TraderId for {orderId}.");
            return null;
        }

        /// <inheritdoc />
        public override TraderId? GetTraderId(PositionId positionId)
        {
            if (this.indexPositionTrader.TryGetValue(positionId, out var traderId))
            {
                return traderId;
            }

            this.Logger.LogWarning(LogId.Database, $"Cannot find TraderId for {positionId}.");
            return null;
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(OrderId orderId)
        {
            if (this.indexOrderAccount.TryGetValue(orderId, out var accountId))
            {
                return accountId;
            }

            this.Logger.LogWarning($"Cannot find AccountId for {orderId}.");
            return null;
        }

        /// <inheritdoc />
        public override AccountId? GetAccountId(PositionId positionId)
        {
            if (this.indexPositionAccount.TryGetValue(positionId, out var accountId))
            {
                return accountId;
            }

            this.Logger.LogWarning($"Cannot find AccountId for {positionId} in the database.");
            return null;
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(OrderId orderId)
        {
            if (this.indexOrderPosition.TryGetValue(orderId, out var positionId))
            {
                return positionId;
            }

            this.Logger.LogWarning($"Cannot find PositionId for {orderId} in the database.");
            return null;
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(AccountId accountId, PositionIdBroker positionIdBroker)
        {
            if (this.indexBrokerPosition.TryGetValue(positionIdBroker, out var positionId))
            {
                return positionId;
            }

            this.Logger.LogWarning($"Cannot find PositionId for {positionIdBroker} in the database.");
            return null;
        }

        /// <inheritdoc />
        public override PositionIdBroker? GetPositionIdBroker(PositionId positionId)
        {
            return this.indexPositionBroker.TryGetValue(positionId, out var positionIdBroker)
                ? positionIdBroker
                : null;
        }

        /// <inheritdoc />
        public override ICollection<TraderId> GetTraderIds()
        {
            return new SortedSet<TraderId>(this.indexTraders.Keys);
        }

        /// <inheritdoc />
        public override ICollection<AccountId> GetAccountIds()
        {
            return new SortedSet<AccountId>(this.indexAccounts);
        }

        /// <inheritdoc />
        public override ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? new SortedSet<StrategyId>(traderIndex.StrategyIds)
                : new SortedSet<StrategyId>();
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds()
        {
            return new SortedSet<OrderId>(this.indexOrders);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? strategyIdFilter = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? new SortedSet<OrderId>(traderIndex.OrderIds(strategyIdFilter))
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds()
        {
            return new SortedSet<OrderId>(this.indexOrdersWorking);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexOrdersWorking, traderIndex.OrderIds(filterStrategyId) })
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds()
        {
            return new SortedSet<OrderId>(this.indexOrdersCompleted);
        }

        /// <inheritdoc />
        public override ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexOrdersCompleted, traderIndex.OrderIds(filterStrategyId) })
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds()
        {
            return new SortedSet<PositionId>(this.CachedPositions.Keys);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositions, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds()
        {
            return new SortedSet<PositionId>(this.indexPositionsOpen);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositionsOpen, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds()
        {
            return new SortedSet<PositionId>(this.indexPositionsClosed);
        }

        /// <inheritdoc />
        public override ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositionsClosed, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }
    }
}
