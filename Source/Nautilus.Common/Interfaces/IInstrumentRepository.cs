//--------------------------------------------------------------------------------------------------
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
        /// Returns the instrument symbol collection.
        /// </summary>
        IReadOnlyCollection<Symbol> GetSymbols();

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
        /// Returns the <see cref="decimal"/> tick size of the <see cref="Instrument"/>
        /// corresponding to the given <see cref="Symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Decimal}"/> result.</returns>
        QueryResult<decimal> GetTickSize(Symbol symbol);
    }
}
