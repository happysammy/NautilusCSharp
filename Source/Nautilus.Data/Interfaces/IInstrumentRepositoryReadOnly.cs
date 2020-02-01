//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides a repository for accessing <see cref="Instrument"/> data.
    /// </summary>
    public interface IInstrumentRepositoryReadOnly
    {
        /// <summary>
        /// Returns the instrument corresponding to the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="QueryResult{Instrument}"/> result.</returns>
        QueryResult<Instrument> FindInCache(Symbol symbol);

        /// <summary>
        /// Returns the all instruments corresponding to the given venue.
        /// </summary>
        /// <param name="venue">The venue.</param>
        /// <returns>A result of the query.</returns>
        QueryResult<IEnumerable<Instrument>> FindInCache(Venue venue);

        /// <summary>
        /// Returns the instrument symbol collection.
        /// </summary>
        /// <returns>The collection of symbols held by the repository.</returns>
        IReadOnlyCollection<Symbol> GetInstrumentSymbols();
    }
}
