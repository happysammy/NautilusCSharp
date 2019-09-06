// -------------------------------------------------------------------------------------------------
// <copyright file="IExecutionDatabaseCommand.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Interfaces
{
    /// <summary>
    /// Provides an execution database with write operations.
    /// </summary>
    public interface IExecutionDatabaseCommand
    {
        /// <summary>
        /// Gets a value indicating whether the execution database will load the caches on instantiation.
        /// </summary>
        bool OptionLoadCache { get; }

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
    }
}
