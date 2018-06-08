// -------------------------------------------------------------------------------------------------
// <copyright file="RedisMarketDataClientTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Compression;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Database.Keys;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisBarClientTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly RedisBarClient client;

        public RedisBarClientTests(ITestOutputHelper output)
        {
            // Fixture Setup
            RedisServiceStack.ConfigureServiceStack();

            this.output = output;

            // Data compression off so that redis-cli is readable.
            this.client = new RedisBarClient(RedisConstants.LocalHost, new LZ4DataCompressor(false));
            this.client.FlushAll("YES");
        }

        public void Dispose()
        {
            //this.client.FlushAll("YES");
        }

        [Fact]
        internal void AllKeysCount_WithNoKeys_ReturnsZero()
        {
            // Arrange
            // Act
            var result = this.client.AllKeysCount();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void KeysCount_WithNoKeys_ReturnsZero()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();

            // Act
            var result = this.client.KeysCount(barSpec);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void KeyExists_WithNoKeys_ReturnsFalse()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var marketDataKey = new BarDataKey(barSpec, new DateKey(StubZonedDateTime.UnixEpoch()));

            // Act
            var result1 = this.client.KeyExists(marketDataKey);
            var result2 = this.client.KeyExists(marketDataKey.ToString());

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }

        [Fact]
        internal void KeyExists_WithKeyInRedis_ReturnsTrue()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var marketDataKey = new BarDataKey(barSpec, new DateKey(StubZonedDateTime.UnixEpoch()));
            var bar = new Bar(0.80000M, 0.80010M, 0.79990M, 0.80001M, 1000000, StubZonedDateTime.UnixEpoch());

            this.client.AddBars(barSpec, new[] { bar });

            // Act
            var result1 = this.client.KeyExists(marketDataKey);
            var result2 = this.client.KeyExists(marketDataKey.ToString());

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        internal void GetAllKeys_WithNoKeysInRedis_ReturnsQueryFailure()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();

            // Act
            var result = this.client.GetAllSortedKeys(barSpec);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetAllKeys_WithKeysInRedis_ReturnsKeys()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(2));

            this.client.AddBars(barSpec, new[] { bar1, bar2 });

            // Act
            var result = this.client.GetAllSortedKeys(barSpec);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);
            result.Value.ForEach(k => this.output.WriteLine(k));
        }

        [Theory]
        [InlineData(0, 1, "1970-01-01")]
        [InlineData(1, 1, "1970-01-01")]
        [InlineData(12, 1, "1970-01-01")]
        [InlineData(24, 1, "1970-01-01")]
        [InlineData(36, 2, "1970-01-02")]
        [InlineData(48, 3, "1970-01-03")]
        [InlineData(168, 8, "1970-01-08")]
        [InlineData(960, 41, "1970-02-10")]
        internal void GetDateKeys_WithVariousTimeRanges_ReturnsExpectedListOfKeys(
            int hoursOffset,
            int dateKeyCount,
            string expectedKey)
        {
            // Arrange
            var fromDateTime = StubZonedDateTime.UnixEpoch();
            var toDateTime = StubZonedDateTime.UnixEpoch() + Duration.FromHours(hoursOffset);

            // Act
            var result = DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime);

            // Assert
            Assert.Equal(dateKeyCount, result.Count);
            Assert.Equal(expectedKey, result.Last().ToString());
            result.ForEach(dk => this.output.WriteLine(dk.ToString()));
        }

        [Fact]
        internal void Add_WithOneBar_AddsBarToRedis()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            var result = this.client.AddBars(barSpec, new[] { bar });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.client.BarsCount(barSpec));
            Assert.Equal(1, this.client.KeysCount(barSpec));
            Assert.Equal(1, this.client.AllBarsCount());
            Assert.Equal(1, this.client.AllKeysCount());
        }

        [Fact]
        internal void Add_OneBarWithOneBarAlreadyPersisted_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);

            this.client.AddBars(barSpec, new[] { bar1 });

            // Act
            var result = this.client.AddBars(barSpec, new[] { bar2 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, this.client.BarsCount(barSpec));
            Assert.Equal(1, this.client.KeysCount(barSpec));
            Assert.Equal(2, this.client.AllBarsCount());
            Assert.Equal(1, this.client.AllKeysCount());
        }

        [Fact]
        internal void Add_OneBarWithTwoBarsAlreadyPersistedWithAGap_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(2);
            var bar3 = StubBarData.Create(4);

            this.client.AddBars(barSpec, new[] { bar1, bar2 });

            // Act
            var result = this.client.AddBars(barSpec, new[] { bar3 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, this.client.BarsCount(barSpec));
            Assert.Equal(3, this.client.AllBarsCount());
        }

        [Fact]
        internal void Add_OneBarWithTwoAlreadyPersistedAtDifferentDay_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(1) + Duration.FromMinutes(1));

            this.client.AddBars(barSpec, new[] { bar1, bar2 });

            // Act
            var result = this.client.AddBars(barSpec, new[] { bar3 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, this.client.BarsCount(barSpec));
            Assert.Equal(3, this.client.AllBarsCount());
        }

        [Fact]
        internal void Add_WithMultipleBars_AddsBarsToRedis()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            // Act
            var result = this.client.AddBars(barSpec, new[] { bar1, bar2, bar3, bar4, bar5 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.client.BarsCount(barSpec));
            Assert.Equal(5, this.client.AllBarsCount());
        }

        [Fact]
        internal void GetBarString_WithOneBar_ReturnsBar()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();

            this.client.AddBars(barSpec, new[] { bar });

            // Act
            var result = this.client.GetBar(barSpec, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Value.ToString());
            Assert.True(result.IsSuccess);
            Assert.Equal("1.00000,1.00000,1.00000,1.00000,1000000,1970-01-01T00:00:00.000Z", result.Value.ToString());
        }

        [Fact]
        internal void GetBar_WithNoBars_ReturnsQueryFailure()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            var result = this.client.GetBar(barSpec, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetBar_WithOneBar_ReturnsBar()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();
            this.client.AddBars(barSpec, new[] { bar });

            // Act
            var result = this.client.GetBar(barSpec, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(bar, result.Value);
        }

        [Fact]
        internal void GetBar_WithMultipleBars_ReturnsExpectedBar()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            this.client.AddBars(barSpec, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBar(barSpec, bar2.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(bar2, result.Value);
        }

        [Fact]
        internal void GetBars_WithNoBars_ReturnsQueryFailure()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();

            // Act
            var result = this.client.GetBars(barSpec, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
            Assert.Equal("QueryResult Failure (No market data found for AUDUSD.Dukascopy 1-Minute[Ask]).", result.FullMessage);
        }

        [Fact]
        internal void GetBars_WithBarOutOfRange_ReturnsQueryFailure()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create(Duration.FromHours(1));

            this.client.AddBars(barSpec, new[] { bar });

            // Act
            var result = this.client.GetBars(barSpec, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(2));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
            Assert.Equal("QueryResult Failure (No market data found for AUDUSD.Dukascopy 1-Minute[Ask] in time range from 1970-01-01T00:00:00.000Z to 1970-01-01T00:02:00.000Z).", result.FullMessage);
        }

        [Fact]
        internal void GetBars_WithRangeOfOneBar_ReturnsExpectedBars()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.client.AddBars(barSpec, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barSpec, bar2.Timestamp, bar2.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value.Bars);
            Assert.Equal(bar2, result.Value.Bars[0]);
        }

        [Fact]
        internal void GetBars_WithRangeOfThreeBars_ReturnsExpectedThreeBars()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.client.AddBars(barSpec, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barSpec, bar1.Timestamp, bar3.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Bars.Length);
        }

        [Fact]
        internal void GetBars_WithThreeBarsSpreadAcrossThreeDays_ReturnsExpectedBars()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            this.client.AddBars(barSpec, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barSpec, bar1.Timestamp, bar3.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Bars.Length);
        }
    }
}
