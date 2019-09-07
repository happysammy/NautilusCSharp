// -------------------------------------------------------------------------------------------------
// <copyright file="InMemoryExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Engine
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;

    /// <summary>
    /// Provides an in-memory execution database.
    /// </summary>
    public class InMemoryExecutionDatabase : ExecutionDatabase, IExecutionDatabase
    {
        private readonly Dictionary<OrderId, TraderId> indexOrderTrader;
        private readonly Dictionary<OrderId, AccountId> indexOrderAccount;
        private readonly Dictionary<OrderId, PositionId> indexOrderPosition;
        private readonly Dictionary<PositionId, TraderId> indexPositionTrader;
        private readonly Dictionary<PositionId, AccountId> indexPositionAccount;
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
            this.Log.Information("Re-caching accounts from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void LoadOrdersCache()
        {
            this.Log.Information("Re-caching orders from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void LoadPositionsCache()
        {
            this.Log.Information("Re-caching positions from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public override void Flush()
        {
            this.Log.Information("Flushing the database...");

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
        /// <exception cref="ConditionFailedException">If the order identifier is already indexed.</exception>
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
                this.indexTraders[traderId] = new TraderIdentifierIndex(traderId);
                this.indexTraders[traderId].AddIdentifiers(order.Id, positionId, strategyId);
            }

            this.indexOrders.Add(order.Id);
            this.CachedOrders[order.Id] = order;

            this.Log.Debug($"Added new {order.Id}, indexed {traderId}, {accountId}, {positionId}, {strategyId}");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        /// <exception cref="ConditionFailedException">If the position identifier is already indexed.</exception>
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
            this.CachedPositions[position.Id] = position;

            this.Log.Debug($"Added open position_id={position.Id}");

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

            this.Log.Warning($"Cannot find TraderId for {orderId}.");
            return null;
        }

        /// <inheritdoc />
        public override PositionId? GetPositionId(OrderId orderId)
        {
            if (this.indexOrderPosition.TryGetValue(orderId, out var positionId))
            {
                return positionId;
            }

            this.Log.Warning($"Cannot find PositionId for {orderId} in the database.");
            return null;
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
