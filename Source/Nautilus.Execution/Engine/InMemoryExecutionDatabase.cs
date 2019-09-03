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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
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
    public class InMemoryExecutionDatabase : Component, IExecutionDatabase
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

        private readonly Dictionary<OrderId, Order> cachedOrders;
        private readonly Dictionary<PositionId, Position> cachedPositions;

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

            this.cachedOrders = new Dictionary<OrderId, Order>();
            this.cachedPositions = new Dictionary<PositionId, Position>();
        }

        /// <inheritdoc />
        public void LoadOrdersCache()
        {
            this.Log.Information("Re-caching orders from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        public void LoadPositionsCache()
        {
            this.Log.Information("Re-caching positions from the database (does nothing for this implementation.)");
        }

        /// <inheritdoc />
        /// <exception cref="ConditionFailedException">If the order identifier is already indexed.</exception>
        public CommandResult AddOrder(AtomicOrder order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
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
        /// <exception cref="ConditionFailedException">If the order identifier is already indexed.</exception>
        public CommandResult AddOrder(Order order, TraderId traderId, AccountId accountId, StrategyId strategyId, PositionId positionId)
        {
            Debug.KeyNotIn(order.Id, this.indexOrderTrader, nameof(order.Id), nameof(this.indexOrderTrader));
            Debug.KeyNotIn(order.Id, this.indexOrderAccount, nameof(order.Id), nameof(this.indexOrderAccount));
            Debug.KeyNotIn(order.Id, this.indexOrderPosition, nameof(order.Id), nameof(this.indexOrderPosition));
            Debug.NotIn(order.Id, this.indexOrders, nameof(order.Id), nameof(this.indexOrders));

            if (this.cachedOrders.ContainsKey(order.Id))
            {
                return CommandResult.Fail($"The {order.Id} already existed in the cache (was not unique).");
            }

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
            }

            this.indexOrders.Add(order.Id);
            this.cachedOrders[order.Id] = order;

            this.Log.Debug($"Added new order_id={order.Id}, " +
                           $"indexed trader_id={traderId}, " +
                           $"indexed account_id={accountId}, " +
                           $"indexed position_id={positionId}, " +
                           $"indexed strategy_id={strategyId}");

            return CommandResult.Ok();
        }

        /// <inheritdoc />
        /// <exception cref="ConditionFailedException">If the position identifier is already indexed.</exception>
        public CommandResult AddPosition(Position position)
        {
            Debug.NotIn(position.Id, this.indexPositions, nameof(position.Id), nameof(this.indexPositions));
            Debug.NotIn(position.Id, this.indexPositionsOpen, nameof(position.Id), nameof(this.indexPositions));

            if (this.cachedPositions.ContainsKey(position.Id))
            {
                return CommandResult.Fail($"The {position.Id} already existed in the cache (was not unique).");
            }

            this.indexPositions.Add(position.Id);
            this.indexPositionsOpen.Add(position.Id);
            this.cachedPositions[position.Id] = position;

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
        public void CheckResiduals()
        {
            foreach (var orderId in this.indexOrdersWorking)
            {
                this.Log.Warning($"Order {orderId} still working.");
            }

            foreach (var positionId in this.indexPositionsOpen)
            {
                this.Log.Warning($"Position {positionId} still open.");
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            this.indexOrderTrader.Clear();
            this.indexOrderAccount.Clear();
            this.indexOrderPosition.Clear();
            this.indexPositionTrader.Clear();
            this.indexPositionAccount.Clear();
            this.indexPositionOrders.Clear();
            this.indexAccountOrders.Clear();
            this.indexAccountPositions.Clear();
            this.indexTraders.Clear();
            this.indexOrders.Clear();
            this.indexOrdersWorking.Clear();
            this.indexOrdersCompleted.Clear();
            this.indexPositions.Clear();
            this.indexPositionsOpen.Clear();
            this.indexPositionsClosed.Clear();
            this.cachedOrders.Clear();
            this.cachedPositions.Clear();
        }

        /// <inheritdoc />
        public void Flush()
        {
            this.Log.Information("Flushing the database (in-memory database does nothing).");
        }

        /// <inheritdoc />
        public ICollection<TraderId> GetTraderIds()
        {
            return new SortedSet<TraderId>(this.indexTraders.Keys);
        }

        /// <inheritdoc />
        public ICollection<AccountId> GetAccountIds()
        {
            return new SortedSet<AccountId>(this.indexAccounts);
        }

        /// <inheritdoc />
        public ICollection<StrategyId> GetStrategyIds(TraderId traderId)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? new SortedSet<StrategyId>(traderIndex.StrategyIds)
                : new SortedSet<StrategyId>();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderIds()
        {
            return new SortedSet<OrderId>(this.indexOrders);
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? strategyIdFilter = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? new SortedSet<OrderId>(traderIndex.OrderIds(strategyIdFilter))
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderWorkingIds()
        {
            return new SortedSet<OrderId>(this.indexOrdersWorking);
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexOrdersWorking, traderIndex.OrderIds(filterStrategyId) })
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderCompletedIds()
        {
            return new SortedSet<OrderId>(this.indexOrdersCompleted);
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexOrdersCompleted, traderIndex.OrderIds(filterStrategyId) })
                : new SortedSet<OrderId>();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionIds()
        {
            return new SortedSet<PositionId>(this.cachedPositions.Keys);
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositions, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionOpenIds()
        {
            return new SortedSet<PositionId>(this.indexPositionsOpen);
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositionsOpen, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionClosedIds()
        {
            return new SortedSet<PositionId>(this.indexPositionsClosed);
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.IntersectionSorted(new[] { this.indexPositionsClosed, traderIndex.PositionIds(filterStrategyId) })
                : new SortedSet<PositionId>();
        }

        /// <inheritdoc />
        public Order? GetOrder(OrderId orderId)
        {
            if (this.cachedOrders.TryGetValue(orderId, out var order))
            {
                return order;
            }

            this.Log.Warning($"Cannot find Order for {orderId} in cache.");
            return null;
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders()
        {
            return new Dictionary<OrderId, Order>(this.cachedOrders);
        }

        /// <inheritdoc />
        [PerformanceOptimized]
        public IDictionary<OrderId, Order> GetOrders(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexOrders, traderIndex.OrderIds(filterStrategyId) })
                : new HashSet<OrderId>();

            return this.GetOrdersFromCache(orderIds);
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking()
        {
            return this.GetOrdersFromCache(this.indexOrdersWorking);
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexOrdersWorking, traderIndex.OrderIds(filterStrategyId) })
                : new HashSet<OrderId>();

            return this.GetOrdersFromCache(orderIds);
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted()
        {
            return this.GetOrdersFromCache(this.indexOrdersCompleted);
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var orderIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexOrdersCompleted, traderIndex.OrderIds(filterStrategyId) })
                : new HashSet<OrderId>();

            return this.GetOrdersFromCache(orderIds);
        }

        /// <inheritdoc />
        public Position? GetPosition(PositionId positionId)
        {
            if (this.cachedPositions.TryGetValue(positionId, out var position))
            {
                return position;
            }

            this.Log.Warning($"Cannot find Position for {positionId} in the memory cache.");
            return null;
        }

        /// <inheritdoc />
        public Position? GetPositionForOrder(OrderId orderId)
        {
            if (this.indexOrderPosition.TryGetValue(orderId, out var positionId))
            {
                return this.GetPosition(positionId);
            }

            this.Log.Warning($"Cannot find Position for {orderId} in the database.");
            return null;
        }

        /// <inheritdoc />
        public PositionId? GetPositionId(OrderId orderId)
        {
            if (this.indexOrderPosition.TryGetValue(orderId, out var positionId))
            {
                return positionId;
            }

            this.Log.Warning($"Cannot find PositionId for {orderId} in the database.");
            return null;
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions()
        {
            return new Dictionary<PositionId, Position>(this.cachedPositions);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexPositions, traderIndex.PositionIds(filterStrategyId) })
                : new HashSet<PositionId>();

            return this.GetPositionsFromCache(positionIds);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen()
        {
            return this.GetPositionsFromCache(this.indexPositionsOpen);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexPositionsOpen, traderIndex.PositionIds(filterStrategyId) })
                : new HashSet<PositionId>();

            return this.GetPositionsFromCache(positionIds);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed()
        {
            return this.GetPositionsFromCache(this.indexPositionsClosed);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            var positionIds = this.indexTraders.TryGetValue(traderId, out var traderIndex)
                ? SetFactory.Intersection(new[] { this.indexPositionsClosed, traderIndex.PositionIds(filterStrategyId) })
                : new HashSet<PositionId>();

            return this.GetPositionsFromCache(positionIds);
        }

        [PerformanceOptimized]
        private Dictionary<OrderId, Order> GetOrdersFromCache(HashSet<OrderId> orderIds)
        {
            var orders = new Dictionary<OrderId, Order>();

            try
            {
                foreach (var orderId in orderIds)
                {
                    orders[orderId] = this.cachedOrders[orderId];
                }
            }
            catch (KeyNotFoundException ex)
            {
                // This should never happen
                this.Log.Error($"Cannot find an OrderId in the memory cache.", ex);
            }

            return orders;
        }

        [PerformanceOptimized]
        private Dictionary<PositionId, Position> GetPositionsFromCache(HashSet<PositionId> positionIds)
        {
            var positions = new Dictionary<PositionId, Position>();

            try
            {
                foreach (var positionId in positionIds)
                {
                    positions[positionId] = this.cachedPositions[positionId];
                }
            }
            catch (KeyNotFoundException ex)
            {
                // This should never happen
                this.Log.Error($"Cannot find a PositionId in the memory cache.", ex);
            }

            return positions;
        }
    }
}
