//--------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides generic repository commands.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        void SnapshotDatabase();
    }
}
