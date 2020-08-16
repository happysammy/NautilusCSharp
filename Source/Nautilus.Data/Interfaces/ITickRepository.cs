//--------------------------------------------------------------------------------------------------
// <copyright file="ITickRepository.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides a repository for accessing <see cref="Tick"/> data.
    /// </summary>
    public interface ITickRepository
    {
        /// <summary>
        /// Ingest the given tick into the database.
        /// </summary>
        /// <param name="tick">The tick to ingest.</param>
        void Ingest(QuoteTick tick);

        /// <summary>
        /// Returns a value indicating whether any ticks for the given symbol exist in the
        /// repository.
        /// </summary>
        /// <param name="symbol">The symbol to query.</param>
        /// <returns>True if ticks exist, else false.</returns>
        bool TicksExist(Symbol symbol);

        /// <summary>
        /// Returns the count of ticks held within the repository for the given symbol.
        /// </summary>
        /// <param name="symbol">The tick symbol to count.</param>
        /// <returns>An <see cref="int"/>.</returns>
        long TicksCount(Symbol symbol);

        /// <summary>
        /// Returns the ticks held in the repository of the given <see cref="Symbol"/> within the given
        /// range.
        /// </summary>
        /// <param name="symbol">The tick symbol to find.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The tick data.</returns>
        QuoteTick[] GetTicks(
            Symbol symbol,
            ZonedDateTime? fromDateTime = null,
            ZonedDateTime? toDateTime = null,
            long? limit = null);

        /// <summary>
        /// Read tick data from the given query values.
        /// </summary>
        /// <param name="symbol">The symbol for the ticks.</param>
        /// <param name="fromDateTime">The datetime to query from (optional).</param>
        /// <param name="toDateTime">The datetime to query to (optional).</param>
        /// <param name="limit">The limit of data rows to return (optional).</param>
        /// <returns>The serialized tick data.</returns>
        public byte[][] ReadTickData(
            Symbol symbol,
            ZonedDateTime? fromDateTime,
            ZonedDateTime? toDateTime,
            long? limit);
    }
}
