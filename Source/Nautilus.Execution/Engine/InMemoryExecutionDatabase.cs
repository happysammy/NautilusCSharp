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
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;

    /// <summary>
    /// Provides an in-memory execution database.
    /// </summary>
    public class InMemoryExecutionDatabase : Component, IExecutionDatabase
    {
        private readonly Dictionary<TraderId, HashSet<OrderId>> indexTraderOrders;
        private readonly Dictionary<TraderId, HashSet<PositionId>> indexTraderPositions;
        private readonly Dictionary<TraderId, HashSet<StrategyId>> indexTraderStrategies;
        private readonly Dictionary<OrderId, TraderId> indexOrderTrader;
        private readonly Dictionary<OrderId, PositionId> indexOrderPosition;
        private readonly Dictionary<OrderId, StrategyId> indexOrderStrategy;
        private readonly Dictionary<PositionId, TraderId> indexPositionTrader;
        private readonly Dictionary<PositionId, HashSet<OrderId>> indexPositionOrders;
        private readonly Dictionary<PositionId, StrategyId> indexPositionStrategy;
        private readonly Dictionary<StrategyId, TraderId> indexStrategyTrader;
        private readonly Dictionary<StrategyId, HashSet<OrderId>> indexStrategyOrders;
        private readonly Dictionary<StrategyId, HashSet<PositionId>> indexStrategyPositions;

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
            this.indexTraderOrders = new Dictionary<TraderId, HashSet<OrderId>>();
            this.indexTraderPositions = new Dictionary<TraderId, HashSet<PositionId>>();
            this.indexTraderStrategies = new Dictionary<TraderId, HashSet<StrategyId>>();
            this.indexOrderTrader = new Dictionary<OrderId, TraderId>();
            this.indexOrderPosition = new Dictionary<OrderId, PositionId>();
            this.indexOrderStrategy = new Dictionary<OrderId, StrategyId>();
            this.indexPositionTrader = new Dictionary<PositionId, TraderId>();
            this.indexPositionOrders = new Dictionary<PositionId, HashSet<OrderId>>();
            this.indexPositionStrategy = new Dictionary<PositionId, StrategyId>();
            this.indexStrategyOrders = new Dictionary<StrategyId, HashSet<OrderId>>();
            this.indexStrategyTrader = new Dictionary<StrategyId, TraderId>();
            this.indexStrategyPositions = new Dictionary<StrategyId, HashSet<PositionId>>();

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
        /// <exception cref="ConditionFailedException">If the order identifier is already contained in the cached orders.</exception>
        public void AddOrder(Order order, TraderId traderId, StrategyId strategyId, PositionId positionId)
        {
            Debug.KeyNotIn(order.Id, this.indexOrderTrader, nameof(order.Id), nameof(this.indexOrderTrader));
            Debug.KeyNotIn(order.Id, this.indexOrderPosition, nameof(order.Id), nameof(this.indexOrderPosition));
            Debug.KeyNotIn(order.Id, this.indexOrderStrategy, nameof(order.Id), nameof(this.indexOrderStrategy));
            Debug.NotIn(order.Id, this.indexOrders, nameof(order.Id), nameof(this.indexOrders));
            Debug.KeyNotIn(order.Id, this.cachedOrders, nameof(order.Id), nameof(this.cachedOrders));

            this.indexOrderTrader[order.Id] = traderId;
            this.indexOrderPosition[order.Id] = positionId;
            this.indexOrderStrategy[order.Id] = strategyId;
            this.indexPositionTrader[positionId] = traderId;
            this.indexPositionStrategy[positionId] = strategyId;
            this.indexStrategyTrader[strategyId] = traderId;

            // Index: TraderId -> Set[OrderId]
            if (this.indexTraderOrders.TryGetValue(traderId, out var traderOrders))
            {
                traderOrders.Add(order.Id);
            }
            else
            {
                this.indexTraderOrders[traderId] = new HashSet<OrderId> { order.Id };
            }

            // Index: TraderId -> Set[PositionId]
            if (this.indexTraderPositions.TryGetValue(traderId, out var traderPositions))
            {
                traderPositions.Add(positionId);
            }
            else
            {
                this.indexTraderPositions[traderId] = new HashSet<PositionId> { positionId };
            }

            // Index: TraderId -> Set[StrategyId]
            if (this.indexTraderStrategies.TryGetValue(traderId, out var traderStrategies))
            {
                traderStrategies.Add(strategyId);
            }
            else
            {
                this.indexTraderStrategies[traderId] = new HashSet<StrategyId> { strategyId };
            }

            // Index: PositionId -> Set[OrderId]
            if (this.indexPositionOrders.TryGetValue(positionId, out var positionOrders))
            {
                positionOrders.Add(order.Id);
            }
            else
            {
                this.indexPositionOrders[positionId] = new HashSet<OrderId> { order.Id };
            }

            // Index: StrategyId -> Set[OrderId]
            if (this.indexStrategyOrders.TryGetValue(strategyId, out var strategyOrders))
            {
                strategyOrders.Add(order.Id);
            }
            else
            {
                this.indexStrategyOrders[strategyId] = new HashSet<OrderId> { order.Id };
            }

            // Index: StrategyId -> Set[PositionId]
            if (this.indexStrategyPositions.TryGetValue(strategyId, out var strategyPositions))
            {
                strategyPositions.Add(positionId);
            }
            else
            {
                this.indexStrategyPositions[strategyId] = new HashSet<PositionId> { positionId };
            }

            this.indexOrders.Add(order.Id);

            this.cachedOrders.Add(order.Id, order);

            this.Log.Debug($"Added new order_id={order.Id}, " +
                           $"indexed trader_id={traderId}, " +
                           $"indexed position_id={positionId}, " +
                           $"indexed strategy_id={strategyId}");
        }

        /// <inheritdoc />
        public void AddPosition(Position position, TraderId traderId, StrategyId strategyId)
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
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void Flush()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<TraderId> GetTraderIds()
        {
            return new HashSet<TraderId>(this.indexTraderOrders.Keys);
        }

        /// <inheritdoc />
        public ICollection<StrategyId> GetStrategyIds(TraderId? traderIdFilter = null)
        {
            if (traderIdFilter is null)
            {
                return new HashSet<StrategyId>(this.indexStrategyOrders.Keys);
            }

            return this.indexTraderStrategies.TryGetValue(traderIdFilter, out var strategyIds)
                ? strategyIds
                : new HashSet<StrategyId>();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderWorkingIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<OrderId> GetOrderCompletedIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionOpenIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ICollection<PositionId> GetPositionClosedIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public StrategyId? GetStrategyForOrder(OrderId orderId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Order? GetOrder(OrderId orderId, bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetWorkingOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetCompletedOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Position? GetPosition(PositionId positionId, bool loadIfNotFound = false)
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
        public IDictionary<PositionId, Position> GetPositions(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
