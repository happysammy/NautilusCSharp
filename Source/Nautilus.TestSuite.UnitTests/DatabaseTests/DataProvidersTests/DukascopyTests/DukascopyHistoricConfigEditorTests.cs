// -------------------------------------------------------------------------------------------------
// <copyright file="DukascopyHistoricConfigEditorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.DataProvidersTests.DukascopyTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NodaTime;
    using ServiceStack;
    using Xunit;
    using Nautilus.Database.Dukascopy;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DukascopyHistoricConfigEditorTests
    {
        [Fact]
        internal void UpdateConfigCsv_WithValidData_CreatesExpectedFile()
        {
            // Arrange
            var dukasEditor = new DukascopyHistoricConfigEditor(
                TestKitConstants.TestDataDirectory + "historicConfigTest.csv",
                "yyyy.MM.dd HH:mm:ss",
                false,
                string.Empty);

            var currencyPairs = new List<string> { "AUDUSD", "GBPUSD", "EURUSD", "USDJPY" };

            // Act
            dukasEditor.UpdateConfigCsv(
                currencyPairs,
                StubZonedDateTime.UnixEpoch(),
                StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Assert
            Assert.True(File.Exists(dukasEditor.ConfigCsvPath.FullName));
        }

        [Fact]
        internal void UpdateConfigCsv_WithExampleData_CreatesFileMatchingExample()
        {
            // Arrange
            var dukasEditor = new DukascopyHistoricConfigEditor(
                TestKitConstants.TestDataDirectory + "historicConfigTest.csv",
                "yyyy.MM.dd HH:mm:ss",
                false,
                string.Empty);

            var fromDateTime = new ZonedDateTime(new LocalDateTime(2018, 1, 8, 0, 0), DateTimeZone.Utc, Offset.Zero);
            var toDateTime = new ZonedDateTime(new LocalDateTime(2018, 1, 12, 23, 59), DateTimeZone.Utc, Offset.Zero);

            var currencyPairs = new List<string> { "EURUSD", "AUDUSD", "GBPUSD", "USDJPY" };

            // Act
            dukasEditor.UpdateConfigCsv(
                currencyPairs,
                fromDateTime,
                toDateTime);

            var historicConfigTxt = dukasEditor.ConfigCsvPath
                .Open(FileMode.Open)
                .ReadLines()
                .Join();

            var historicConfigExampleTxt = new FileInfo(TestKitConstants.TestDataDirectory + "historicConfigExample.csv")
                .Open(FileMode.Open)
                .ReadLines()
                .Join();

            // Assert
            Assert.Equal(historicConfigExampleTxt, historicConfigTxt);
        }
    }
}
