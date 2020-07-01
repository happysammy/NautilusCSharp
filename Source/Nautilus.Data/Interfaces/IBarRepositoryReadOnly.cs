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

using Nautilus.Core.CQS;
using Nautilus.Data.Keys;
using Nautilus.DomainModel.Frames;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.Data.Interfaces
{
    /// <summary>
    /// Provides a repository for accessing <see cref="Bar"/> data.
    /// </summary>
    public interface IBarRepositoryReadOnly
    {
        /// <summary>
        /// Returns the total count of bars held within the repository.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        long BarsCount();

        /// <summary>
        /// Returns the count of bars held within the repository for the given
        /// <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The bar type to count.</param>
        /// <returns>A <see cref="int"/>.</returns>
        long BarsCount(BarType barType);

        /// <summary>
        /// Returns all bars from the repository of the given <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The type of bars to get.</param>
        /// <param name="limit">The optional limit for a count of bars.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<BarDataFrame> GetBars(BarType barType, int limit = 0);

        /// <summary>
        /// Returns the bars held in the database of the given <see cref="BarType"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="barType">The type of bars to get.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="limit">The optional count limit for the bars.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<BarDataFrame> GetBars(
            BarType barType,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0);

        /// <summary>
        /// Returns the bar data held in the database of the given <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The type of bars data to get.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetBarData(BarType barType, int limit = 0);

        /// <summary>
        /// Returns the bar data held in the database of the given <see cref="BarType"/> within the given
        /// range of <see cref="DateKey"/>s (inclusive).
        /// </summary>
        /// <param name="barType">The type of bars data to get.</param>
        /// <param name="fromDate">The from date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="limit">The optional count limit for the data.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<byte[][]> GetBarData(
            BarType barType,
            DateKey fromDate,
            DateKey toDate,
            int limit = 0);
    }
}
