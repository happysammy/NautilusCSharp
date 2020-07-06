// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseWrite.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.CQS;
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.Execution.Interfaces
{
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
        /// Add the given bracket order to the execution database indexed with the given identifiers.
        /// </summary>
        /// <param name="order">The bracket order to add.</param>
        /// <param name="traderId">The trader identifier to index.</param>
        /// <param name="accountId">The account identifier to index.</param>
        /// <param name="strategyId">The strategy identifier to index.</param>
        /// <param name="positionId">The position identifier to index.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult AddBracketOrder(
            BracketOrder order,
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
