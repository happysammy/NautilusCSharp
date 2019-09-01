// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabase.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an execution database for persisting execution related data.
    /// </summary>
    public interface IExecutionDatabase
    {
        /// <summary>
        /// Add the given atomic order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="order">The atomic order to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="accountId">The account identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        /// <param name="positionId">The position identifier to index.</param>
        void AddOrder(
            AtomicOrder order,
            TraderId traderId,
            AccountId accountId,
            StrategyId strategyId,
            PositionId positionId);

        /// <summary>
        /// Add the given order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="accountId">The account identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        /// <param name="positionId">The position identifier to index.</param>
        void AddOrder(
            Order order,
            TraderId traderId,
            AccountId accountId,
            StrategyId strategyId,
            PositionId positionId);

        /// <summary>
        /// Add the given position to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="position">The position to add.</param>
        void AddPosition(Position position);

        /// <summary>
        /// Update the given order in the execution database (using the orders last event).
        /// </summary>
        /// <param name="order">The order to update.</param>
        void UpdateOrder(Order order);

        /// <summary>
        /// Update the given position in the execution database (using the positions last event).
        /// </summary>
        /// <param name="position">The position to update.</param>
        void UpdatePosition(Position position);

        /// <summary>
        /// Update the given account in the execution database (using the accounts last event).
        /// </summary>
        /// <param name="account">The account to update.</param>
        void UpdateAccount(Account account);

        /// <summary>
        /// Check for residual working orders and open positions.
        /// </summary>
        void CheckResiduals();

        /// <summary>
        /// Reset the execution database by clearing all stateful values.
        /// </summary>
        void Reset();

        /// <summary>
        /// WARNING: Flush the execution database of all persisted data.
        /// </summary>
        void Flush();

        /// <summary>
        /// Return all trader identifiers.
        /// </summary>
        /// <returns>The trader identifiers.</returns>
        ICollection<TraderId> GetTraderIds();

        /// <summary>
        /// Return all strategy identifiers for the given trader identifier.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The strategy identifiers.</returns>
        ICollection<StrategyId> GetStrategyIds(TraderId traderId);

        /// <summary>
        /// Return all order identifiers.
        /// </summary>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderIds();

        /// <summary>
        /// Return all order identifiers for the given trader identifier and optional strategy
        /// identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all working order identifiers.
        /// </summary>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderWorkingIds();

        /// <summary>
        /// Return all working order identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderWorkingIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all completed order identifiers.
        /// </summary>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderCompletedIds();

        /// <summary>
        /// Return all completed order identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderCompletedIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all position identifiers.
        /// </summary>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionIds();

        /// <summary>
        /// Return all position identifiers for the given trader identifier and optional strategy
        /// identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all open position identifiers.
        /// </summary>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionOpenIds();

        /// <summary>
        /// Return all position open identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all closed position identifiers.
        /// </summary>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionClosedIds();

        /// <summary>
        /// Return all closed position identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return the order matching the given identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The order (if found else null).</returns>
        Order? GetOrder(OrderId orderId);

        /// <summary>
        /// Return all orders.
        /// </summary>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrders();

        /// <summary>
        /// Return all orders for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrders(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all working orders.
        /// </summary>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrdersWorking();

        /// <summary>
        /// Return all working orders for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrdersWorking(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all completed orders.
        /// </summary>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrdersCompleted();

        /// <summary>
        /// Return all completed orders for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrdersCompleted(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return the position matching the given identifier (if found else null).
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <returns>The position (if found else null).</returns>
        Position? GetPosition(PositionId positionId);

        /// <summary>
        /// Return the position matching the given identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier for the position.</param>
        /// <returns>The position (if found else null).</returns>
        Position? GetPositionForOrder(OrderId orderId);

        /// <summary>
        /// Return the position identifier for the given order identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier for the position.</param>
        /// <returns>The position identifier (if found else null).</returns>
        PositionId? GetPositionId(OrderId orderId);

        /// <summary>
        /// Return all positions.
        /// </summary>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositions();

        /// <summary>
        /// Return all positions for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositions(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all open positions.
        /// </summary>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsOpen();

        /// <summary>
        /// Return all open positions for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsOpen(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all closed positions.
        /// </summary>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsClosed();

        /// <summary>
        /// Return all closed positions for the given trader identifier and optional strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsClosed(TraderId traderId, StrategyId? filterStrategyId = null);
    }
}
