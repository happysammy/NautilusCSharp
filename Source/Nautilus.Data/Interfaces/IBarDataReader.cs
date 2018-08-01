//--------------------------------------------------------------------------------------------------
// <copyright file="IBarDataReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.IO;
    using Nautilus.Core.CQS;
    using Nautilus.Data.Types;
    using NodaTime;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The bar data reader interface.
    /// </summary>
    public interface IBarDataReader
    {
        /// <summary>
        /// Gets the readers symbol bar specification.
        /// </summary>
        BarType BarType { get; }

        /// <summary>
        /// Returns a <see cref="BarDataFrame"/> of all bars data.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <returns>A query result of <see cref="BarDataFrame"/>.</returns>
        QueryResult<BarDataFrame> GetAllBars(FileInfo csvFile);

        /// <summary>
        /// Returns a <see cref="BarDataFrame"/> of the bars data from and including the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result of <see cref="BarDataFrame"/>.</returns>
        QueryResult<BarDataFrame> GetBars(FileInfo csvFile, ZonedDateTime fromDateTime);

        /// <summary>
        /// Returns a <see cref="BarDataFrame"/> of data.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <returns>A query result of <see cref="BarDataFrame"/>.</returns>
        QueryResult<BarDataFrame> GetLastBar(FileInfo csvFile);

        // TODO: Temporary Property
        QueryResult<FileInfo[]> GetAllCsvFilesOrdered();
    }
}
