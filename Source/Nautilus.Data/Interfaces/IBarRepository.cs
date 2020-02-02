//--------------------------------------------------------------------------------------------------
// <copyright file="IBarRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a repository for accessing <see cref="Bar"/> data.
    /// </summary>
    public interface IBarRepository : IBarRepositoryReadOnly
    {
        /// <summary>
        /// Adds the given bars to the repository.
        /// </summary>
        /// <param name="barData">The market data to add.</param>
        void Add(BarDataFrame barData);

        /// <summary>
        /// Adds the given bar to the repository.
        /// </summary>
        /// <param name="barType">The barType to add.</param>
        /// <param name="bar">The bar to add.</param>
        void Add(BarType barType, Bar bar);

        /// <summary>
        /// Adds the given bars to the repository.
        /// </summary>
        /// <param name="barType">The barType to add.</param>
        /// <param name="bars">The bars to add.</param>
        void Add(BarType barType, Bar[] bars);

        /// <summary>
        /// Removes the difference in date keys for each symbol from the repository.
        /// </summary>
        /// <param name="barStructure">The bar resolution to trim.</param>
        /// <param name="trimToDays">The number of days (keys) to trim to.</param>
        void TrimToDays(BarStructure barStructure, int trimToDays);

        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        void SnapshotDatabase();
    }
}
