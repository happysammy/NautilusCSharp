//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
