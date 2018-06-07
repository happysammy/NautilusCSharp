// -------------------------------------------------------------------------------------------------
// <copyright file="CsvMarketDataReaderTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests.ReadersTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Database.Core.Readers;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class CsvMarketDataReaderTests
    {
        private readonly ITestOutputHelper output;

        public CsvMarketDataReaderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void FilePath_ReturnsExpectedFilePath()
        {
            // Arrange
            var csvMarketDataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider(),
                5);

            // Act
            var filePath = csvMarketDataReader.FilePathWildcard;

            // Assert
            Assert.Equal("AUDUSD_1 Min_Ask_*.csv", filePath);
        }

        [Fact]
        internal void FileCount_ReturnsOne()
        {
            // Arrange
            var csvMarketDataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider(),
                5);

            // Act
            var fileCount = csvMarketDataReader.FileCount;

            // Assert
            Assert.Equal(1, fileCount);
        }

        [Fact]
        internal void GetAllBars_WithAllTestData_ReturnsExpectedBars()
        {
            // Arrange
            var csvMarketDataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider(),
                5);

            var csvFiles = csvMarketDataReader.GetAllCsvFilesOrdered();

            // Act
            var result = csvMarketDataReader.GetAllBars(csvFiles.Value[0]);


            // Assert
            Assert.Equal(7200, result.Value.Bars.Length);
        }

        [Fact]
        internal void GetLastBar_WithTestData_ReturnsExpectedBar()
        {
            // Arrange
            var csvMarketDataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider(),
                5);

            // Act
            var query = csvMarketDataReader.GetLastBar(csvMarketDataReader.GetAllCsvFilesOrdered().Value[0]);

            // Assert
            Assert.True(query.IsSuccess);
        }

        [Fact]
        internal void GetLastBarTime_WithTestData_ReturnsExpectedTime()
        {
            // Arrange
            var csvMarketDataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider(),
                5);

            // Act
            var query = csvMarketDataReader.GetLastBarTimestamp(csvMarketDataReader.GetAllCsvFilesOrdered().Value[0]);

            // Assert
            Assert.Equal("2018.01.12 23:59:00".ToZonedDateTime("yyyy.MM.dd HH:mm:ss"), query.Value);
        }
    }
}
