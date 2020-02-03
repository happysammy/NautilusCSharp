//--------------------------------------------------------------------------------------------------
// <copyright file="IBarRepositoryReadOnly.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using Nautilus.Core.CQS;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

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

        /// <summary>
        /// Returns the result of the last bars timestamp of the given <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType);
    }
}
