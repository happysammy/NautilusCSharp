// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseWrite.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Interfaces
{
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an adapter to an execution databases write operations.
    /// </summary>
    public interface IExecutionDatabaseWrite
    {
        /// <summary>
        /// Add the given account to the execution database.
        /// </summary>
        /// <param name="account">The account to add.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult AddAccount(Account account);

        /// <summary>
        /// Add the given atomic order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="order">The atomic order to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="accountId">The account identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        /// <param name="positionId">The position identifier to index.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult AddAtomicOrder(
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
        /// <returns>The result of the operation.</returns>
        CommandResult AddOrder(
            Order order,
            TraderId traderId,
            AccountId accountId,
            StrategyId strategyId,
            PositionId positionId);

        /// <summary>
        /// Add the given position to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="position">The position to add.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult AddPosition(Position position);

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
    }
}
