//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a repository for accessing <see cref="Tick"/> data.
    /// </summary>
    public interface ITickRepositoryReadOnly
    {
        /// <summary>
        /// Returns the count of ticks held within the repository for the given symbol.
        /// </summary>
        /// <param name="symbol">The tick symbol to count.</param>
        /// <returns>An <see cref="int"/>.</returns>
        int TicksCount(Symbol symbol);

        /// <summary>
        /// Returns the total count of ticks held within the repository.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        int AllTicksCount();

        /// <summary>
        /// Returns the result of the find ticks query.
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<List<Tick>> Find(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime);

        /// <summary>
        /// Returns the result of the last tick timestamp of the given symbol.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<ZonedDateTime> LastTickTimestamp(Symbol symbol);
    }
}
