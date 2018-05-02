// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataFrameTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests.TypesTests
{
    using Nautilus.Database.Core.Types;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MarketDataFrameTests
    {
        private readonly ITestOutputHelper output;
        private readonly SymbolBarData stubBarSpec;

        public MarketDataFrameTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.stubBarSpec = StubSymbolBarData.AUDUSD();
        }

        [Fact]
        internal void StartDateTime_WithStubBars_ReturnsExpectedResult()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bars = new[] { bar1, bar2 };

            var marketDataFrame = new MarketDataFrame(
                this.stubBarSpec,
                bars);

            // Act
            var result = marketDataFrame.StartDateTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch(), result);
        }

        [Fact]
        internal void EndDateTime_WithStubBars_ReturnsExpectedResult()
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bars = new[] { bar1, bar2 };

            var marketDataFrame = new MarketDataFrame(
                this.stubBarSpec,
                bars);

            // Act
            var result = marketDataFrame.EndDateTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1), result);
        }

        [Fact]
        internal void Serialize_WithStubBar_ReturnsExpectedString()
        {
            // Arrange
            var bar = StubBarData.Create();
            var bars = new[] { bar };

            var marketDataFrame = new MarketDataFrame(
                this.stubBarSpec,
                bars);

            // Act
            var result = JsonConvert.SerializeObject(marketDataFrame);

            // Assert
            this.output.WriteLine(result);
        }

        [Fact]
        internal void Serialize_WithMultipleStubBars_ReturnsExpectedString()
        {
            // Arrange
            var bar1 = StubBarData.Create();;
            var bar2 = StubBarData.Create(1);;
            var bar3 = StubBarData.Create(2);;
            var bar4 = StubBarData.Create(3);;

            var bars = new[] { bar1, bar2, bar3, bar4 };

            var marketDataFrame = new MarketDataFrame(
                this.stubBarSpec,
                bars);

            // Act
            var result = JsonConvert.SerializeObject(marketDataFrame);

            // Assert
            this.output.WriteLine(result);
        }
    }
}
