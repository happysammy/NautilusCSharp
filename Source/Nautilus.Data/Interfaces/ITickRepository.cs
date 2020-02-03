//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a repository for accessing <see cref="Tick"/> data.
    /// </summary>
    public interface ITickRepository : ITickRepositoryReadOnly
    {
        /// <summary>
        /// Add the given tick to the repository.
        /// </summary>
        /// <param name="tick">The tick to add.</param>
        void Add(Tick tick);

        /// <summary>
        /// Add the given ticks to the repository.
        /// </summary>
        /// <param name="ticks">The ticks to add.</param>
        void Add(List<Tick> ticks);

        /// <summary>
        /// Trims each symbols tick data in the repository to equal the given days.
        /// </summary>
        /// <param name="trimToDays">The number of days (keys) to trim to.</param>
        void TrimToDays(int trimToDays);

        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        void SnapshotDatabase();
    }
}
