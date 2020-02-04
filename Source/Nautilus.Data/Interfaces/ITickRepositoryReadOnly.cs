//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using Nautilus.Core.CQS;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a repository for accessing <see cref="Tick"/> data.
    /// </summary>
    public interface ITickRepositoryReadOnly
    {
        /// <summary>
        /// Returns the total count of ticks held within the repository.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        long TicksCount();

        /// <summary>
        /// Returns the count of ticks held within the repository for the given symbol.
        /// </summary>
        /// <param name="symbol">The tick symbol to count.</param>
        /// <returns>An <see cref="int"/>.</returns>
        long TicksCount(Symbol symbol);

        /// <summary>
        /// Returns the ticks held in the repository of the given <see cref="Symbol"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<Tick[]> GetTicks(Symbol symbol, int limit = 0);

        /// <summary>
        /// Returns the ticks held in the repository of the given <see cref="Symbol"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<Tick[]> GetTicks(
            Symbol symbol,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0);

        /// <summary>
        /// Returns the tick data held in the repository of the given <see cref="Symbol"/>.
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetTickData(Symbol symbol, int limit = 0);

        /// <summary>
        /// Returns the tick data held in the repository of the given <see cref="Symbol"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetTickData(
            Symbol symbol,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0);
    }
}
