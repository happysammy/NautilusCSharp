﻿//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="IInstrumentRepository"/> interface.
    /// </summary>
    public interface IInstrumentRepository
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
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult Add(Instrument instrument, ZonedDateTime timeNow);

        /// <summary>
        /// Updates the given collection of instruments in the database.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult Add(IReadOnlyCollection<Instrument> instruments, ZonedDateTime timeNow);

        /// <summary>
        /// Returns the instrument corresponding to the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Instrument}"/> result.</returns>
        QueryResult<Instrument> FindInCache(Symbol symbol);

        /// <summary>
        /// Returns the instrument symbol collection.
        /// </summary>
        /// <returns>The collection of symbols held by the repository.</returns>
        IReadOnlyCollection<Symbol> GetSymbols();

        /// <summary>
        /// Returns the dictionary index of broker symbols and their corresponding tick precision.
        /// </summary>
        /// <returns>The tick precision index.</returns>
        Dictionary<string, int> GetTickPrecisionIndex();
    }
}
