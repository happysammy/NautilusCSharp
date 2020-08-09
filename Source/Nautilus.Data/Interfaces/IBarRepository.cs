//--------------------------------------------------------------------------------------------------
// <copyright file="IBarRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides a repository for accessing <see cref="Bar"/> data.
    /// </summary>
    public interface IBarRepository
    {
        /// <summary>
        /// Returns a value indicating whether any bars for the given <see cref="BarType"/> exist in
        /// the repository.
        /// </summary>
        /// <param name="barType">The bar type to query.</param>
        /// <returns>True if bars exist, else false.</returns>
        bool BarsExist(BarType barType);

        /// <summary>
        /// Returns the count of bars held within the repository for the given
        /// <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The bar type to count.</param>
        /// <returns>A <see cref="int"/>.</returns>
        long BarsCount(BarType barType);

        /// <summary>
        /// Returns the bars held in the database of the given <see cref="BarType"/> within the given
        /// range.
        /// </summary>
        /// <param name="barType">The type of bars to get.</param>
        /// <param name="toDateTime">The from date.</param>
        /// <param name="fromDateTime">The to date.</param>
        /// <param name="limit">The optional count limit for the bars.</param>
        /// <returns>The bar data.</returns>
        BarDataFrame GetBars(
            BarType barType,
            ZonedDateTime? fromDateTime = null,
            ZonedDateTime? toDateTime = null,
            long? limit = null);

        /// <summary>
        /// Returns the bar data held in the database of the given <see cref="BarType"/> within the given
        /// range.
        /// </summary>
        /// <param name="barType">The type of bars data to get.</param>
        /// <param name="fromDateTime">The from datetime.</param>
        /// <param name="toDateTime">The to datetime.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The serialized bar data.</returns>
        byte[][] ReadBarData(
            BarType barType,
            ZonedDateTime? fromDateTime = null,
            ZonedDateTime? toDateTime = null,
            long? limit = null);
    }
}
