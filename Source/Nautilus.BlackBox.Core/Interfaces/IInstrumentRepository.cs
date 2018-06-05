//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IInstrumentRepository"/> interface.
    /// </summary>
    public interface IInstrumentRepository
    {
        /// <summary>
        /// Gets the instrument symbol list.
        /// </summary>
        IReadOnlyCollection<Symbol> InstrumentSymbolList { get; }

        /// <summary>
        /// Loads all instruments from the database.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult LoadAllInstrumentsFromDatabase();

        /// <summary>
        /// Updates the given instrument in the database.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult UpdateInstrument(Instrument instrument);

        /// <summary>
        /// Updates the given collection of instruments in the database.
        /// </summary>
        /// <param name="instruments">The instruments collection (cannot be null or empty).</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult UpdateInstruments(IReadOnlyCollection<Instrument> instruments);

        /// <summary>
        /// Deletes all instruments from the database.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult DeleteAllInstrumentsFromDatabase();

        /// <summary>
        /// Returns the instrument corresponding to the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Instrument}"/> result.</returns>
        QueryResult<Instrument> GetInstrument(Symbol symbol);

        /// <summary>
        /// Returns the <see cref="decimal"/> tick size of the <see cref="Instrument"/>
        /// corresponding to the given <see cref="Symbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Decimal}"/> result.</returns>
        QueryResult<decimal> GetTickSize(Symbol symbol);

        /// <summary>
        /// Disposes the instrument repository.
        /// </summary>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        CommandResult Dispose();
    }
}
