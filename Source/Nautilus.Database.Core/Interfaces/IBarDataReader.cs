//--------------------------------------------------------------
// <copyright file="IBarDataReader.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core.Interfaces
{
    using System.IO;
    using NautechSystems.CSharp.CQS;
    using NodaTime;
    using Nautilus.Database.Core.Types;

    public interface IBarDataReader
    {
        SymbolBarData SymbolBarData { get; }

        /// <summary>
        /// Returns a <see cref="MarketDataFrame"/> of all bars data.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <returns>A query result of <see cref="MarketDataFrame"/>.</returns>
        QueryResult<MarketDataFrame> GetAllBars(FileInfo csvFile);

        /// <summary>
        /// Returns a <see cref="MarketDataFrame"/> of the bars data from and including the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result of <see cref="MarketDataFrame"/>.</returns>
        QueryResult<MarketDataFrame> GetBars(FileInfo csvFile, ZonedDateTime fromDateTime);

        /// <summary>
        /// Returns a <see cref="MarketDataFrame"/> of data.
        /// </summary>
        /// <param name="csvFile">The csv file.</param>
        /// <returns>A query result of <see cref="MarketDataFrame"/>.</returns>
        QueryResult<MarketDataFrame> GetLastBar(FileInfo csvFile);

        // TODO: Temporary Property
        QueryResult<FileInfo[]> GetAllCsvFilesOrdered();
    }
}
