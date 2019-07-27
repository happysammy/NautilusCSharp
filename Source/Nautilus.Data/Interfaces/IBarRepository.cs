//--------------------------------------------------------------------------------------------------
// <copyright file="IBarRepository.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a repository for accessing <see cref="Bar"/> data.
    /// </summary>
    public interface IBarRepository
    {
        /// <summary>
        /// Returns the count of bars held within the repository for the given
        /// <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The bar type to count.</param>
        /// <returns>A <see cref="int"/>.</returns>
        int BarsCount(BarType barType);

        /// <summary>
        /// Returns the total count of bars held within the repository.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        int AllBarsCount();

        /// <summary>
        /// Adds the given bar to the repository.
        /// </summary>
        /// <param name="barType">The barType to add.</param>
        /// <param name="bar">The bar to add.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult Add(BarType barType, Bar bar);

        /// <summary>
        /// Adds the given bars to the repository.
        /// </summary>
        /// <param name="barData">The market data to add.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult Add(BarDataFrame barData);

        /// <summary>
        /// Returns the result of the find bars query.
        /// </summary>
        /// <param name="barType">The bar specification to find.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<BarDataFrame> Find(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime);

        /// <summary>
        /// Returns the result of the last bars timestamp of the given <see cref="BarType"/>.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <returns>The result of the query.</returns>
        QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType);

        /// <summary>
        /// Removes the difference in date keys for each symbol from the repository.
        /// </summary>
        /// <param name="resolution">The bar resolution to trim.</param>
        /// <param name="trimToDays">The number of days (keys) to trim to.</param>
        /// <returns>The result of the operation.</returns>
        CommandResult TrimToDays(Resolution resolution, int trimToDays);

        /// <summary>
        /// Save a snapshot of the database to disk.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        CommandResult SnapshotDatabase();
    }
}
