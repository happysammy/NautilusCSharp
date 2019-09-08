// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseRead.cs" company="Nautech Systems Pty Ltd">
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
    /// Provides an adapter to an execution databases read operations.
    /// </summary>
    public interface IExecutionDatabaseRead
    {
        /// <summary>
        /// Return the trader identifier for the given order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns>The traders identifier (if found else null).</returns>
        TraderId? GetTraderId(OrderId orderId);

        /// <summary>
        /// Return the position identifier for the given order identifier (if found else null).
        /// </summary>
        /// <param name="orderId">The order identifier for the position.</param>
        /// <returns>The position identifier (if found else null).</returns>
        PositionId? GetPositionId(OrderId orderId);

        /// <summary>
        /// Return all trader identifiers.
        /// </summary>
        /// <returns>The trader identifiers.</returns>
        ICollection<TraderId> GetTraderIds();

        /// <summary>
        /// Return all account identifiers.
        /// </summary>
        /// <returns>The collection of account identifiers.</returns>
        ICollection<AccountId> GetAccountIds();

        /// <summary>
        /// Return all strategy identifiers for the given trader identifier.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <returns>The collection of strategy identifiers.</returns>
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
        /// <returns>The collection of order identifiers.</returns>
        ICollection<OrderId> GetOrderIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all working order identifiers.
        /// </summary>
        /// <returns>The collection of order identifiers.</returns>
        ICollection<OrderId> GetOrderWorkingIds();

        /// <summary>
        /// Return all working order identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The collection of order identifiers.</returns>
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
        /// <returns>The collection of order identifiers.</returns>
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
        /// <returns>The collection of position identifiers.</returns>
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
        /// <returns>The collection of position identifiers.</returns>
        ICollection<PositionId> GetPositionOpenIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return all closed position identifiers.
        /// </summary>
        /// <returns>The collection of position identifiers.</returns>
        ICollection<PositionId> GetPositionClosedIds();

        /// <summary>
        /// Return all closed position identifiers for the given trader identifier and optional
        /// strategy identifier filter.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="filterStrategyId">The optional strategy identifier filter.</param>
        /// <returns>The collection of position identifiers.</returns>
        ICollection<PositionId> GetPositionClosedIds(TraderId traderId, StrategyId? filterStrategyId = null);

        /// <summary>
        /// Return the account matching the given identifier (if found else null).
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns>The account (if found else null).</returns>
        Account? GetAccount(AccountId accountId);

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
