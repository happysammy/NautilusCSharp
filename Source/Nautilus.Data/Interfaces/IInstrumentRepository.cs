//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides a repository for accessing <see cref="Instrument"/> data.
    /// </summary>
    public interface IInstrumentRepository : IInstrumentRepositoryReadOnly
    {
        /// <summary>
        /// Clears all instruments from the in-memory cache.
        /// </summary>
        void ResetCache();

        /// <summary>
        /// Adds all persisted instruments to the in-memory cache.
        /// </summary>
        void CacheAll();

        /// <summary>
        /// Deletes all instruments from the database.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Deletes the instrument of the given symbol from the database.
        /// </summary>
        /// <param name="symbol">The instrument symbol to delete.</param>
        void Delete(Symbol symbol);

        /// <summary>
        /// Updates the given instrument in the database.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        void Add(Instrument instrument);

        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        void SnapshotDatabase();
    }
}
