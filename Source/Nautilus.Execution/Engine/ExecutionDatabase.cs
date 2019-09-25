// -------------------------------------------------------------------------------------------------
// <copyright file="ExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;

    /// <summary>
    /// Provides the abstract base class for all execution databases.
    /// </summary>
    public abstract class ExecutionDatabase : Component, IExecutionDatabaseRead, IExecutionDatabaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionDatabase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        protected ExecutionDatabase(IComponentryContainer container)
        : base(container)
        {
            this.CachedAccounts = new Dictionary<AccountId, Account>();
            this.CachedOrders = new Dictionary<OrderId, Order>();
            this.CachedPositions = new Dictionary<PositionId, Position>();
        }

        /// <summary>
        /// Gets the cached accounts.
        /// </summary>
        protected Dictionary<AccountId, Account> CachedAccounts { get; }

        /// <summary>
        /// Gets the cached orders.
        /// </summary>
        protected Dictionary<OrderId, Order> CachedOrders { get; }

        /// <summary>
        /// Gets the cached positions.
        /// </summary>
        protected Dictionary<PositionId, Position> CachedPositions { get; }

        /// <inheritdoc />
        public void LoadCaches()
        {
            this.Execute(() =>
            {
                this.LoadAccountsCache();
                this.LoadOrdersCache();
                this.LoadPositionsCache();
            });
        }

        /// <inheritdoc />
        public abstract void LoadAccountsCache();

        /// <inheritdoc />
        public abstract void LoadOrdersCache();

        /// <inheritdoc />
        public abstract void LoadPositionsCache();

        /// <inheritdoc/>
        public void ClearCaches()
        {
            this.Log.Debug($"Clearing caches...");
            this.CachedAccounts.Clear();
            this.CachedOrders.Clear();
            this.CachedPositions.Clear();
            this.Log.Information($"Caches cleared.");
        }

        /// <inheritdoc />
        public void CheckResiduals()
        {
            foreach (var orderId in this.GetOrderWorkingIds())
            {
                this.GetOrder(orderId);  // Check working
                this.Log.Warning($"Residual working order {orderId}.");
            }

            foreach (var positionId in this.GetPositionOpenIds())
            {
                this.GetPosition(positionId);  // Check open
                this.Log.Warning($"Residual open position {positionId}.");
            }
        }

        /// <inheritdoc/>
        public abstract void Flush();

        /// <inheritdoc />
        public abstract TraderId? GetTraderId(OrderId orderId);

        /// <inheritdoc />
        public abstract TraderId? GetTraderId(PositionId positionId);

        /// <inheritdoc />
        public abstract AccountId? GetAccountId(OrderId orderId);

        /// <inheritdoc />
        public abstract AccountId? GetAccountId(PositionId positionId);

        /// <inheritdoc />
        public abstract PositionId? GetPositionId(OrderId orderId);

        /// <inheritdoc />
        public abstract PositionId? GetPositionId(AccountId accountId, PositionIdBroker positionIdBroker);

        /// <inheritdoc />
        public abstract ICollection<TraderId> GetTraderIds();

        /// <inheritdoc />
        public abstract ICollection<AccountId> GetAccountIds();

        /// <inheritdoc />
        public abstract ICollection<StrategyId> GetStrategyIds(TraderId traderId);

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderIds();

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderWorkingIds();

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderCompletedIds();

        /// <inheritdoc />
        public abstract ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionIds();

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionOpenIds();

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionClosedIds();

        /// <inheritdoc />
        public abstract ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <inheritdoc />
        public Account? GetAccount(AccountId accountId)
        {
            return this.CachedAccounts.TryGetValue(accountId, out var account)
                ? account
                : null;
        }

        /// <inheritdoc />
        public Order? GetOrder(OrderId orderId)
        {
            return this.CachedOrders.TryGetValue(orderId, out var order)
                ? order
                : null;
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders()
        {
            return new Dictionary<OrderId, Order>(this.CachedOrders);
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrders(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreateOrdersDictionary(this.GetOrderIds(traderId, filterStrategyId));
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking()
        {
            return this.CreateOrdersWorkingDictionary(this.GetOrderWorkingIds());
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersWorking(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreateOrdersWorkingDictionary(this.GetOrderWorkingIds(traderId, filterStrategyId));
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted()
        {
            return this.CreateOrdersCompletedDictionary(this.GetOrderCompletedIds());
        }

        /// <inheritdoc />
        public IDictionary<OrderId, Order> GetOrdersCompleted(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreateOrdersCompletedDictionary(this.GetOrderCompletedIds(traderId, filterStrategyId));
        }

        /// <inheritdoc />
        public Position? GetPosition(PositionId positionId)
        {
            return this.CachedPositions.TryGetValue(positionId, out var position)
                ? position
                : null;
        }

        /// <inheritdoc />
        public Position? GetPositionForOrder(OrderId orderId)
        {
            var positionId = this.GetPositionId(orderId);

            return positionId is null
                ? null
                : this.GetPosition(positionId);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions()
        {
            return new Dictionary<PositionId, Position>(this.CachedPositions);
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositions(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreatePositionsDictionary(this.GetPositionIds(traderId, filterStrategyId));
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen()
        {
            return this.CreatePositionsOpenDictionary(this.GetPositionOpenIds());
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsOpen(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreatePositionsOpenDictionary(this.GetPositionOpenIds(traderId, filterStrategyId));
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed()
        {
            return this.CreatePositionsClosedDictionary(this.GetPositionClosedIds());
        }

        /// <inheritdoc />
        public IDictionary<PositionId, Position> GetPositionsClosed(TraderId traderId, StrategyId? filterStrategyId = null)
        {
            return this.CreatePositionsClosedDictionary(this.GetPositionClosedIds(traderId, filterStrategyId));
        }

        private Dictionary<OrderId, Order> CreateOrdersDictionary(ICollection<OrderId> orderIds)
        {
            var orders = new Dictionary<OrderId, Order>(orderIds.Count);
            foreach (var orderId in orderIds)
            {
                var order = this.GetOrder(orderId);
                if (order is null)
                {
                    this.Log.Error($"Cannot find {orderId} in the cache.");
                    continue;  // Do not add null order to dictionary
                }

                orders[orderId] = order;
            }

            return orders;
        }

        private Dictionary<OrderId, Order> CreateOrdersWorkingDictionary(ICollection<OrderId> orderIds)
        {
            var orders = new Dictionary<OrderId, Order>(orderIds.Count);
            foreach (var orderId in orderIds)
            {
                var order = this.GetOrder(orderId);
                if (order is null)
                {
                    this.Log.Error($"Cannot find {orderId} in the cache.");
                    continue; // Do not add null order to dictionary
                }

                if (!order.IsWorking)
                {
                    this.Log.Error($"The {orderId} was found not working.");
                    continue;  // Do not add non-working order to dictionary
                }

                orders[orderId] = order;
            }

            return orders;
        }

        private Dictionary<OrderId, Order> CreateOrdersCompletedDictionary(ICollection<OrderId> orderIds)
        {
            var orders = new Dictionary<OrderId, Order>(orderIds.Count);
            foreach (var orderId in orderIds)
            {
                var order = this.GetOrder(orderId);
                if (order is null)
                {
                    this.Log.Error($"Cannot find {orderId} in the cache.");
                    continue; // Do not add null order to dictionary
                }

                if (!order.IsCompleted)
                {
                    this.Log.Error($"The {orderId} was found not completed.");
                    continue;  // Do not add non-completed order to dictionary
                }

                orders[orderId] = order;
            }

            return orders;
        }

        private Dictionary<PositionId, Position> CreatePositionsDictionary(ICollection<PositionId> positionIds)
        {
            var positions = new Dictionary<PositionId, Position>(positionIds.Count);
            foreach (var positionId in positionIds)
            {
                var position = this.GetPosition(positionId);
                if (position is null)
                {
                    this.Log.Error($"Cannot find {positionId} in the cache.");
                    continue;  // Do not add null position to dictionary
                }

                positions[positionId] = position;
            }

            return positions;
        }

        private Dictionary<PositionId, Position> CreatePositionsOpenDictionary(ICollection<PositionId> positionIds)
        {
            var positions = new Dictionary<PositionId, Position>(positionIds.Count);
            foreach (var positionId in positionIds)
            {
                var position = this.GetPosition(positionId);
                if (position is null)
                {
                    this.Log.Error($"Cannot find {positionId} in the cache.");
                    continue;  // Do not add null position to dictionary
                }

                if (!position.IsOpen)
                {
                    this.Log.Error($"The {positionId} was found not open.");
                    continue;  // Do not add non-open position to dictionary
                }

                positions[positionId] = position;
            }

            return positions;
        }

        private Dictionary<PositionId, Position> CreatePositionsClosedDictionary(ICollection<PositionId> positionIds)
        {
            var positions = new Dictionary<PositionId, Position>(positionIds.Count);
            foreach (var positionId in positionIds)
            {
                var position = this.GetPosition(positionId);
                if (position is null)
                {
                    this.Log.Error($"Cannot find {positionId} in the cache.");
                    continue;  // Do not add null position to dictionary
                }

                if (!position.IsClosed)
                {
                    this.Log.Error($"The {positionId} was found not closed.");
                    continue;  // Do not add non-closed position to dictionary
                }

                positions[positionId] = position;
            }

            return positions;
        }
    }
}
