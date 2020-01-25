// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseCommand.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
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
