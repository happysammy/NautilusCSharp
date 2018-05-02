//--------------------------------------------------------------------------------------------------
// <copyright file="CsvBarDataReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Readers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// Collects market data from CSV files in the specified directory path.
    /// </summary>
    [Immutable]
    public sealed class CsvBarDataReader : IBarDataReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBarDataReader"/> class.
        /// </summary>
        /// <param name="symbolBarData">The symbol bar data.</param>
        /// <param name="dataProvider">The market data provider.</param>
        /// <exception cref="NautechSystems.CSharp.Validation.ValidationException">Throws if the validation fails.</exception>
        public CsvBarDataReader(
            SymbolBarData symbolBarData,
            IBarDataProvider dataProvider)
        {
            Validate.NotNull(symbolBarData, nameof(symbolBarData));
            Validate.NotNull(dataProvider, nameof(dataProvider));

            this.SymbolBarData = symbolBarData;
            this.DataProvider = dataProvider;
            this.FilePathWildcard = this.BuildFilePathWildcard(dataProvider);
        }

        /// <summary>
        /// Gets the CSV market data readers symbol bar data.
        /// </summary>
        public SymbolBarData SymbolBarData { get; }

        /// <summary>
        /// Gets the CSV market data readers data provider.
        /// </summary>
        public IBarDataProvider DataProvider { get; }

        /// <summary>
        /// Gets the file path * of CSV files.
        /// </summary>
        public string FilePathWildcard { get; }

        /// <summary>
        /// Gets the count of CSV files matching the given <see cref="BarSpecification"/>.
        /// </summary>
        public int FileCount => this.DataProvider.DataPath.GetFiles(this.FilePathWildcard).Length;

        /// <summary>
        /// Gets an array of all CSV files ordered by date.
        /// </summary>
        /// <returns>
        /// A <see cref="QueryResult{T}"/> containing an array of <see cref="FileInfo"/>.
        /// </returns>
        public QueryResult<FileInfo[]> GetAllCsvFilesOrdered()
        {
            return this.DataProvider.DataPath.Exists ?
                       QueryResult<FileInfo[]>.Ok(this.DataProvider.DataPath
                           .EnumerateFiles(this.FilePathWildcard)
                           .OrderBy(f => f.Name)
                           .ToArray())
                       : QueryResult<FileInfo[]>.Fail($"The data directory {this.DataProvider.DataPath} does not exist.");
        }

        /// <summary>
        /// Returns the bars based on all data contained within the CSV directory.
        /// </summary>
        /// <returns>A query result potentially containing a <see cref="MarketDataFrame"/>.</returns>
        public QueryResult<MarketDataFrame> GetAllBars(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            return readAllBarsQuery.IsSuccess
                ? QueryResult<MarketDataFrame>.Ok(readAllBarsQuery.Value)
                : QueryResult<MarketDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the bars based on all data contained within the CSV directory where the bars
        /// timestamp is greater than the given from date time.
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result containing a <see cref="MarketDataFrame"/> if successful.</returns>
        [PerformanceOptimized]
        public QueryResult<MarketDataFrame> GetBars(FileInfo csvFile, ZonedDateTime fromDateTime)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            if (readAllBarsQuery.IsSuccess)
            {
                if (readAllBarsQuery.Value.BarsData.Last().Timestamp.IsLessThan(fromDateTime))
                {
                    QueryResult<MarketDataFrame>.Fail(
                        $"No bars found to collect for {this.SymbolBarData} " +
                        $"after {fromDateTime.ToIsoString()}");
                }

                var bars = readAllBarsQuery.Value.BarsData;
                var barsAfterFromDate = new List<BarData>();

                for (var i = 0; i < bars.Length; i++)
                {
                    if (bars[i].Timestamp.IsGreaterThan(fromDateTime))
                    {
                        barsAfterFromDate.Add(bars[i]);
                    }
                }

                // TODO: Redundant check due to bars 0 bug.
                if (barsAfterFromDate.Count == 0)
                {
                    return QueryResult<MarketDataFrame>.Fail(
                        $"No bars found to collect for {this.SymbolBarData} " +
                        $"after {fromDateTime.ToIsoString()}");
                }

                return QueryResult<MarketDataFrame>.Ok(new MarketDataFrame(
                    this.SymbolBarData,
                    barsAfterFromDate.ToArray()));
            }

            return QueryResult<MarketDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the latest bar based on all data contained within the CSV directory.
        /// </summary>
        /// <returns>A query result potentially containing a <see cref="MarketDataFrame"/>.</returns>
        public QueryResult<MarketDataFrame> GetLastBar(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            if (readAllBarsQuery.IsSuccess)
            {
                var lastBar = readAllBarsQuery
                    .Value
                    .BarsData
                    .Last();

                return QueryResult<MarketDataFrame>.Ok(new MarketDataFrame(
                    this.SymbolBarData,
                    new [] { lastBar }));
            }

            return QueryResult<MarketDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the timestamp of the last bar from the data.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public QueryResult<ZonedDateTime> GetLastBarTimestamp(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var lastBarQuery = this.GetLastBar(csvFile);

            return lastBarQuery.IsSuccess
                 ? QueryResult<ZonedDateTime>.Ok(lastBarQuery.Value.BarsData.First().Timestamp)
                 : QueryResult<ZonedDateTime>.Fail(lastBarQuery.Message);
        }

        private QueryResult<MarketDataFrame> ReadAllBarsFromCsv(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            while (true)
            {
                try
                {
                    if (!this.DataProvider.DataPath.Exists)
                    {
                        return QueryResult<MarketDataFrame>.Fail($"{this.DataProvider.DataPath} does not exist");
                    }

                    if (this.FileCount == 0)
                    {
                        return this.NoDataFoundQueryResultFailure();
                    }

                    var barsList = new List<BarData>();

                    using (var textReader = File.OpenText(csvFile.FullName))
                    {
                        var reader = new CsvReader(textReader);
                        textReader.ReadLine();

                        while (reader.Read())
                        {
                            barsList.Add(new BarData( // TODO: Do not do this.
                                reader.GetField<decimal>(1),
                                reader.GetField<decimal>(2),
                                reader.GetField<decimal>(3),
                                reader.GetField<decimal>(4),
                                Convert.ToInt64(reader.GetField<double>(5)) * this.DataProvider.VolumeMultiple,
                                reader.GetField<string>(0).ToZonedDateTime(this.DataProvider.TimestampParsePattern)));
                        }
                    }

                    // The only place where bars are sorted into order.
                    barsList.Sort();

                    return barsList.Count > 0
                         ? QueryResult<MarketDataFrame>.Ok(new MarketDataFrame(
                            this.SymbolBarData,
                            barsList.ToArray()))
                         : QueryResult<MarketDataFrame>.Fail(
                                   $"No bars to collect for {this.SymbolBarData} in {csvFile.Name}");
                }
                catch (IOException)
                {
                    // Empty catch block to restart method due to concurrent file access.
                }
            }
        }

        private string BuildFilePathWildcard(IBarDataProvider barDataProvider)
        {
            Debug.NotNull(barDataProvider, nameof(barDataProvider));

            // TODO: Temporary if to handle Dukas 'Hourly'.
            if (this.SymbolBarData.BarSpecification.Resolution == BarResolution.Hour)
            {
                return $"{this.SymbolBarData}_"
                       + $"{barDataProvider.GetResolutionLabel(this.SymbolBarData.BarSpecification.Resolution)}_"
                       + $"{this.SymbolBarData.BarSpecification.QuoteType}_"
                       + $"*.csv";
            }

            return $"{this.SymbolBarData}_" // TODO: This is now broken.
                 + $"{this.SymbolBarData.BarSpecification.Period} "
                 + $"{barDataProvider.GetResolutionLabel(this.SymbolBarData.BarSpecification.Resolution)}_"
                 + $"{this.SymbolBarData.BarSpecification.QuoteType}_"
                 + $"*.csv";
        }

        private QueryResult<MarketDataFrame> NoDataFoundQueryResultFailure()
        {
            return QueryResult<MarketDataFrame>.Fail(
                $"No data found for {this.SymbolBarData.BarSpecification.ToString()} in {this.DataProvider.DataPath}{this.FilePathWildcard}");
        }
    }
}
