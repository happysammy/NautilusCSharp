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
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an execution database for persisting execution related data.
    /// </summary>
    public interface IExecutionDatabase
    {
        /// <summary>
        /// Add the given order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="order">The order to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        /// <param name="positionId">The position identifier to index.</param>
        void AddOrder(
            Order order,
            TraderId traderId,
            StrategyId strategyId,
            PositionId positionId);

        /// <summary>
        /// Add the given order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="position">The position to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        void AddPosition(
            Position position,
            TraderId traderId,
            StrategyId strategyId);

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
        /// Return a collection of trader identifiers persisted in the execution database.
        /// </summary>
        /// <returns>The trader identifiers.</returns>
        ICollection<TraderId> GetTraderIds();

        /// <summary>
        /// Return a collection of strategy identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <returns>The strategy identifiers.</returns>
        ICollection<StrategyId> GetStrategyIds(TraderId? traderIdFilter = null);

        /// <summary>
        /// Return a collection of order identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return a collection of working order identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderWorkingIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return a collection of completed order identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The order identifiers.</returns>
        ICollection<OrderId> GetOrderCompletedIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return a collection of position identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return a collection of open position identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionOpenIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return a collection of closed position identifiers persisted in the execution database.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <returns>The position identifiers.</returns>
        ICollection<PositionId> GetPositionClosedIds(TraderId? traderIdFilter = null, StrategyId? strategyIdFilter = null);

        /// <summary>
        /// Return the strategy identifier for the given order identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The strategy identifier (if found else null).</returns>
        StrategyId? GetStrategyForOrder(OrderId orderId);

        /// <summary>
        /// Return the order matching the given identifier from the memory cache (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="loadIfNotFound">The flag indicating whether the order should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The order (if found else null).</returns>
        Order? GetOrder(OrderId orderId, bool loadIfNotFound = false);

        /// <summary>
        /// Return a dictionary of orders from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether orders should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);

        /// <summary>
        /// Return a dictionary of working orders from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether orders should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetWorkingOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);

        /// <summary>
        /// Return a dictionary of completed orders from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether orders should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of orders.</returns>
        IDictionary<OrderId, Order> GetCompletedOrders(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);

        /// <summary>
        /// Return the position matching the given identifier from the memory cache (if found else null).
        /// </summary>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="loadIfNotFound">The flag indicating whether the position should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The position (if found else null).</returns>
        Position? GetPosition(PositionId positionId, bool loadIfNotFound = false);

        /// <summary>
        /// Return the position matching the given identifier from the memory cache (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier for the position.</param>
        /// <returns>The position (if found else null).</returns>
        Position? GetPositionForOrder(OrderId orderId);

        /// <summary>
        /// Return the position identifier indexed for the given order identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier for the position.</param>
        /// <returns>The position identifier (if found else null).</returns>
        PositionId? GetPositionId(OrderId orderId);

        /// <summary>
        /// Return a dictionary of positions from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether positions should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositions(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);

        /// <summary>
        /// Return a dictionary of open positions from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether positions should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsOpen(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);

        /// <summary>
        /// Return a dictionary of closed positions from the memory cache.
        /// </summary>
        /// <param name="traderIdFilter">The optional trader identifier filter.</param>
        /// <param name="strategyIdFilter">The optional strategy identifier filter.</param>
        /// <param name="loadIfNotFound">The flag indicating whether positions should be loaded
        /// from the database if not found in the memory cache.</param>
        /// <returns>The dictionary of positions.</returns>
        IDictionary<PositionId, Position> GetPositionsClosed(
            TraderId? traderIdFilter = null,
            StrategyId? strategyIdFilter = null,
            bool loadIfNotFound = false);
    }
}
