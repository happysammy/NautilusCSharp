//--------------------------------------------------------------------------------------------------
// <copyright file="CsvBarDataReader.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Readers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Collects market data from CSV files in the specified directory path.
    /// </summary>
    [Immutable]
    public sealed class CsvBarDataReader : IBarDataReader
    {
        private readonly int decimals;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvBarDataReader"/> class.
        /// </summary>
        /// <param name="barType">The symbol bar data.</param>
        /// <param name="dataProvider">The market data provider.</param>
        /// <param name="decimals">The decimal precision for bars.</param>
        /// <exception cref="Nautilus.Core.Validation.ValidationException">Throws if the validation fails.</exception>
        public CsvBarDataReader(
            BarType barType,
            IBarDataProvider dataProvider,
            int decimals)
        {
            Validate.NotNull(barType, nameof(barType));
            Validate.NotNull(dataProvider, nameof(dataProvider));

            this.BarType = barType;
            this.DataProvider = dataProvider;
            this.FilePathWildcard = this.BuildFilePathWildcard(dataProvider);
            this.decimals = decimals;
        }

        /// <summary>
        /// Gets the CSV market data readers symbol bar data.
        /// </summary>
        public BarType BarType { get; }

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
        /// <param name="csvFile">The CSV file to read.</param>
        /// <returns>A query result potentially containing a <see cref="BarDataFrame"/>.</returns>
        public QueryResult<BarDataFrame> GetAllBars(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            return readAllBarsQuery.IsSuccess
                ? QueryResult<BarDataFrame>.Ok(readAllBarsQuery.Value)
                : QueryResult<BarDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the bars based on all data contained within the CSV directory where the bars
        /// timestamp is greater than the given from date time.
        /// </summary>
        /// <param name="csvFile">The CSV file to read.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result containing a <see cref="BarDataFrame"/> if successful.</returns>
        [PerformanceOptimized]
        public QueryResult<BarDataFrame> GetBars(FileInfo csvFile, ZonedDateTime fromDateTime)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            if (readAllBarsQuery.IsSuccess)
            {
                if (readAllBarsQuery.Value.Bars.Last().Timestamp.IsLessThan(fromDateTime))
                {
                    QueryResult<BarDataFrame>.Fail(
                        $"No bars found to collect for {this.BarType} " +
                        $"after {fromDateTime.ToIsoString()}");
                }

                var bars = readAllBarsQuery.Value.Bars;
                var barsAfterFromDate = new List<Bar>();

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
                    return QueryResult<BarDataFrame>.Fail(
                        $"No bars found to collect for {this.BarType} " +
                        $"after {fromDateTime.ToIsoString()}");
                }

                return QueryResult<BarDataFrame>.Ok(new BarDataFrame(
                    this.BarType,
                    barsAfterFromDate.ToArray()));
            }

            return QueryResult<BarDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the latest bar based on all data contained within the CSV directory.
        /// </summary>
        /// <param name="csvFile">The CSV file to read.</param>
        /// <returns>A query result potentially containing a <see cref="BarDataFrame"/>.</returns>
        public QueryResult<BarDataFrame> GetLastBar(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var readAllBarsQuery = this.ReadAllBarsFromCsv(csvFile);

            if (readAllBarsQuery.IsSuccess)
            {
                var lastBar = readAllBarsQuery
                    .Value
                    .Bars
                    .Last();

                return QueryResult<BarDataFrame>.Ok(new BarDataFrame(this.BarType, new[] { lastBar }));
            }

            return QueryResult<BarDataFrame>.Fail(readAllBarsQuery.Message);
        }

        /// <summary>
        /// Returns the timestamp of the last bar from the data.
        /// </summary>
        /// <param name="csvFile">The CSV file to read.</param>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public QueryResult<ZonedDateTime> GetLastBarTimestamp(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            var lastBarQuery = this.GetLastBar(csvFile);

            return lastBarQuery.IsSuccess
                 ? QueryResult<ZonedDateTime>.Ok(lastBarQuery.Value.Bars.First().Timestamp)
                 : QueryResult<ZonedDateTime>.Fail(lastBarQuery.Message);
        }

        private QueryResult<BarDataFrame> ReadAllBarsFromCsv(FileInfo csvFile)
        {
            Validate.NotNull(csvFile, nameof(csvFile));

            while (true)
            {
                try
                {
                    if (!this.DataProvider.DataPath.Exists)
                    {
                        return QueryResult<BarDataFrame>.Fail($"{this.DataProvider.DataPath} does not exist");
                    }

                    if (this.FileCount == 0)
                    {
                        return this.NoDataFoundQueryResultFailure();
                    }

                    var barsList = new List<Bar>();

                    using (var textReader = File.OpenText(csvFile.FullName))
                    {
                        var reader = new CsvReader(textReader);
                        textReader.ReadLine();

                        while (reader.Read())
                        {
                            barsList.Add(new Bar( // TODO: Do not do this. ??
                                Price.Create(reader.GetField<decimal>(1), this.decimals),
                                Price.Create(reader.GetField<decimal>(2), this.decimals),
                                Price.Create(reader.GetField<decimal>(3), this.decimals),
                                Price.Create(reader.GetField<decimal>(4), this.decimals),
                                Quantity.Create(Convert.ToInt32(reader.GetField<double>(5)) * this.DataProvider.VolumeMultiple),
                                reader.GetField<string>(0).ToZonedDateTime(this.DataProvider.TimestampParsePattern)));
                        }
                    }

                    // The only place where bars are sorted into order.
                    barsList.Sort();

                    return barsList.Count > 0
                         ? QueryResult<BarDataFrame>.Ok(new BarDataFrame(
                            this.BarType,
                            barsList.ToArray()))
                         : QueryResult<BarDataFrame>.Fail(
                                   $"No bars to collect for {this.BarType} in {csvFile.Name}");
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
            if (this.BarType.Specification.Resolution == Resolution.Hour)
            {
                return $"{this.BarType.Symbol.Code}_"
                       + $"{barDataProvider.GetResolutionLabel(this.BarType.Specification.Resolution)}_"
                       + $"{this.BarType.Specification.QuoteType}_"
                       + $"*.csv";
            }

            return $"{this.BarType.Symbol.Code}_"
                 + $"{this.BarType.Specification.Period} "
                 + $"{barDataProvider.GetResolutionLabel(this.BarType.Specification.Resolution)}_"
                 + $"{this.BarType.Specification.QuoteType}_"
                 + $"*.csv";
        }

        private QueryResult<BarDataFrame> NoDataFoundQueryResultFailure()
        {
            return QueryResult<BarDataFrame>.Fail(
                $"No data found for {this.BarType.Specification.ToString()} in {this.DataProvider.DataPath}{this.FilePathWildcard}");
        }
    }
}
