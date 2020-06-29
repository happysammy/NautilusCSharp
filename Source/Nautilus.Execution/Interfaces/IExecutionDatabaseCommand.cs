// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseCommand.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Execution.Interfaces
{
    /// <summary>
    /// Provides an adapter to an execution databases command operations.
    /// </summary>
    public interface IExecutionDatabaseCommand
    {
        /// <summary>
        /// Clear all caches and reload from the database.
        /// </summary>
        void LoadCaches();

        /// <summary>
        /// Clear the accounts cache and reload from the database.
        /// </summary>
        void LoadAccountsCache();

        /// <summary>
        /// Clear the orders cache and reload from the database.
        /// </summary>
        void LoadOrdersCache();

        /// <summary>
        /// Clear the positions cache and reload from the database.
        /// </summary>
        void LoadPositionsCache();

        /// <summary>
        /// Reset the execution database by clearing all stateful values.
        /// </summary>
        void ClearCaches();

        /// <summary>
        /// Check for residual working orders and open positions.
        /// </summary>
        void CheckResiduals();

        /// <summary>
        /// WARNING: Flush the execution database of all persisted data.
        /// </summary>
        void Flush();
    }
}
