//--------------------------------------------------------------------------------------------------
// <copyright file="IInstrumentRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
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
        /// Returns the instrument symbol collection.
        /// </summary>
        /// <returns>The collection of symbols held by the repository.</returns>
        IReadOnlyCollection<Symbol> GetCachedSymbols();

        /// <summary>
        /// Returns the instrument corresponding to the given symbol.
        /// </summary>
        /// <param name="symbol">The query symbol.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<Instrument> GetInstrument(Symbol symbol);

        /// <summary>
        /// Returns the all instruments corresponding to the given venue.
        /// </summary>
        /// <param name="venue">The query venue.</param>
        /// <returns>A result of the query.</returns>
        QueryResult<Instrument[]> GetInstruments(Venue venue);

        /// <summary>
        /// Returns the instrument data corresponding to the given symbol.
        /// </summary>
        /// <param name="symbol">The query symbol.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetInstrumentData(Symbol symbol);

        /// <summary>
        /// Returns the all instruments corresponding to the given venue.
        /// </summary>
        /// <param name="venue">The query venue.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetInstrumentData(Venue venue);
    }
}
