// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis;
    using Nautilus.Redis.Data;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using StackExchange.Redis;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisBarRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisBarRepository repository;

        public RedisBarRepositoryTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.logger = containerFactory.LoggingAdapter;
            var container = containerFactory.Create();

            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.repository = new RedisBarRepository(container, this.redisConnection);

            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        public void Dispose()
        {
            // Tear Down
            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        [Fact]
        internal void Add_WithOneBar_AddsBarToRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barType, new[] { bar });

            // Act
            this.repository.Add(marketData);

            // Assert
            this.PrintRepositoryStatus(barType);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barType));
        }

        [Fact]
        internal void Add_WithMultipleBars_AddsBarsToRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);
            var marketData = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5 });

            // Act
            this.repository.Add(marketData);

            // Assert
            this.PrintRepositoryStatus(barType);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(5, this.repository.BarsCount(barType));
        }

        [Fact]
        internal void Add_WithMultipleBarsAndBarsAlreadyPersisted_AddsExpectedBarsToRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            var bar6 = StubBarData.Create(5);
            var bar7 = StubBarData.Create(6);
            var bar8 = StubBarData.Create(7);
            var marketData1 = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5 });
            var marketData2 = new BarDataFrame(barType, new[] { bar6, bar7, bar8 });

            this.repository.Add(marketData1);

            // Act
            this.repository.Add(marketData2);

            // Assert
            this.PrintRepositoryStatus(barType);
            Assert.Equal(8, this.repository.BarsCount(barType));
            Assert.Equal(8, this.repository.AllBarsCount());
        }

        [Fact]
        internal void Add_WithMultipleBarsAlreadyPersisted_AddsNewBarsToRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            var bar6 = StubBarData.Create(5);
            var bar7 = StubBarData.Create(6);
            var bar8 = StubBarData.Create(7);
            var marketData1 = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5 });
            var marketData2 = new BarDataFrame(barType, new[] { bar6, bar7, bar8 });

            this.repository.Add(marketData1);

            // Act
            this.repository.Add(marketData2);

            // Assert
            this.PrintRepositoryStatus(barType);
            Assert.Equal(8, this.repository.BarsCount(barType));
            Assert.Equal(8, this.repository.AllBarsCount());
        }

        [Fact]
        internal void Find_WithNoMarketData_ReturnsExpectedQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsFailure);
            Assert.Equal(0, this.repository.AllBarsCount());
            Assert.Equal(0, this.repository.BarsCount(barType));
            Assert.Equal("Market data not found for AUDUSD.FXCM-1-MINUTE[ASK]", result.Message);
        }

        [Fact]
        internal void Find_WithOtherMarketData_ReturnsExpectedQueryFailure()
        {
            // Arrange
            var barType1 = StubBarType.AUDUSD();
            var barType2 = StubBarType.GBPUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barType1, new[] { bar });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.GetBars(barType2);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barType1));
            Assert.Equal(0, this.repository.BarsCount(barType2));
            Assert.Equal("Market data not found for GBPUSD.FXCM-1-MINUTE[BID]", result.Message);
        }

        [Fact]
        internal void Find_WhenOnlyOneBarPersisted_ReturnsExpectedMarketDataFromRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barType, new[] { bar });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Single(result.Value.Bars);
            Assert.Equal(1, this.repository.BarsCount(barType));
        }

        [Fact]
        internal void Find_WhenMultipleBarsPersisted_ReturnsExpectedMarketDataFromRepository()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);
            var marketData = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5 });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(5, this.repository.BarsCount(barType));
            Assert.Equal(5, result.Value.Bars.Length);
        }

        [Fact]
        internal void Test_can_trim_bars_leaving_correct_number_of_keys()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            var bar4 = StubBarData.Create(Duration.FromDays(3));
            var bar5 = StubBarData.Create(Duration.FromDays(4));
            var bar6 = StubBarData.Create(Duration.FromDays(5));
            var marketData = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });

            this.repository.Add(marketData);

            // Act
            this.repository.TrimToDays(BarStructure.Second, 1);
            this.repository.TrimToDays(BarStructure.Minute, 10);

            // Assert
            Assert.Equal(6, this.repository.BarsCount(barType));
        }

        [Fact]
        internal void Test_can_trim_bars_down_to_correct_number_of_keys()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            var bar4 = StubBarData.Create(Duration.FromDays(3));
            var bar5 = StubBarData.Create(Duration.FromDays(4));
            var bar6 = StubBarData.Create(Duration.FromDays(5));
            var marketData = new BarDataFrame(barType, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });

            this.repository.Add(marketData);

            // Act
            this.repository.TrimToDays(BarStructure.Second, 1);
            this.repository.TrimToDays(BarStructure.Minute, 5);

            // Assert
            Assert.Equal(5, this.repository.BarsCount(barType));
        }

        [Fact]
        internal void Test_can_trim_bars_down_to_correct_number_of_keys_when_multiple_resolutions()
        {
            // Arrange
            var barType1 = StubBarType.AUDUSD();
            var barType2 = StubBarType.GBPUSD_Second();

            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            var bar4 = StubBarData.Create(Duration.FromDays(3));
            var bar5 = StubBarData.Create(Duration.FromDays(4));
            var bar6 = StubBarData.Create(Duration.FromDays(5));
            var barData1 = new BarDataFrame(barType1, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });
            var barData2 = new BarDataFrame(barType2, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });

            this.repository.Add(barData1);
            this.repository.Add(barData2);

            // Act
            this.repository.TrimToDays(BarStructure.Minute, 5);

            // Assert
            this.PrintRepositoryStatus(barType1);
            this.PrintRepositoryStatus(barType2);
            Assert.Equal(5, this.repository.BarsCount(barType1));
            Assert.Equal(6, this.repository.BarsCount(barType2));
        }

        [Fact]
        internal void AllKeysCount_WithNoKeys_ReturnsZero()
        {
            // Arrange
            // Act
            var result = this.repository.AllKeysCount();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void KeysCount_WithNoKeys_ReturnsZero()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.repository.KeysCount(barType);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void KeyExists_WithNoKeys_ReturnsFalse()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var barKey = KeyProvider.GetBarKey(barType, new DateKey(StubZonedDateTime.UnixEpoch()));

            // Act
            var result = this.repository.KeyExists(barKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void KeyExists_WithKeyInRedis_ReturnsTrue()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var barKey = KeyProvider.GetBarKey(barType, new DateKey(StubZonedDateTime.UnixEpoch()));
            var bar = new Bar(
                Price.Create(0.80000m),
                Price.Create(0.80010m),
                Price.Create(0.79990m),
                Price.Create(0.80001m),
                Volume.Create(1000000m),
                StubZonedDateTime.UnixEpoch());

            this.repository.Add(barType, new[] { bar });

            // Act
            var result = this.repository.KeyExists(barKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void GetAllKeys_WithNoKeysInRedis_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.repository.GetKeysSorted(barType);

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetAllKeys_WithKeysInRedis_ReturnsKeys()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(2));

            this.repository.Add(barType, new[] { bar1, bar2 });

            // Act
            var result = this.repository.GetKeysSorted(barType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Length);
            result.Value.ToList().ForEach(k => this.output.WriteLine(k));
        }

        [Theory]
        [InlineData(0, 1, "1970-01-01")]
        [InlineData(1, 1, "1970-01-01")]
        [InlineData(12, 1, "1970-01-01")]
        [InlineData(24, 2, "1970-01-02")]
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
            var result = KeyProvider.GetDateKeys(fromDateTime, toDateTime);

            result.ForEach(dk => this.output.WriteLine(dk.ToString()));

            // Assert
            Assert.Equal(dateKeyCount, result.Count);
            Assert.Equal(expectedKey, result.Last().ToString());
        }

        [Fact]
        internal void AddBar_WithOneBar_AddsBarToRedis()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            this.repository.Add(barType, bar);

            // Assert
            Assert.Equal(1, this.repository.BarsCount(barType));
            Assert.Equal(1, this.repository.KeysCount(barType));
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.AllKeysCount());
        }

        [Fact]
        internal void AddBars_WithOneBar_AddsBarToRedis()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            this.repository.Add(barType, new[] { bar });

            // Assert
            Assert.Equal(1, this.repository.BarsCount(barType));
            Assert.Equal(1, this.repository.KeysCount(barType));
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.AllKeysCount());
        }

        [Fact]
        internal void AddBars_OneBarWithOneBarAlreadyPersisted_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);

            this.repository.Add(barType, new[] { bar1 });

            // Act
            this.repository.Add(barType, new[] { bar2 });

            // Assert
            Assert.Equal(2, this.repository.BarsCount(barType));
            Assert.Equal(1, this.repository.KeysCount(barType));
            Assert.Equal(2, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.AllKeysCount());
        }

        [Fact]
        internal void AddBars_OneBarWithTwoBarsAlreadyPersistedWithAGap_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(2);
            var bar3 = StubBarData.Create(4);

            this.repository.Add(barType, new[] { bar1, bar2 });

            // Act
            this.repository.Add(barType, new[] { bar3 });

            // Assert
            Assert.Equal(3, this.repository.BarsCount(barType));
            Assert.Equal(3, this.repository.AllBarsCount());
        }

        [Fact]
        internal void AddBars_OneBarWithTwoAlreadyPersistedAtDifferentDay_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(1) + Duration.FromMinutes(1));

            this.repository.Add(barType, new[] { bar1, bar2 });

            // Act
            this.repository.Add(barType, new[] { bar3 });

            // Assert
            Assert.Equal(3, this.repository.BarsCount(barType));
            Assert.Equal(3, this.repository.AllBarsCount());
        }

        [Fact]
        internal void AddBars_WithMultipleBars_AddsBarsToRedis()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            // Act
            this.repository.Add(barType, new[] { bar1, bar2, bar3, bar4, bar5 });

            // Assert
            Assert.Equal(5, this.repository.BarsCount(barType));
            Assert.Equal(5, this.repository.AllBarsCount());
        }

        [Fact]
        internal void GetBars_WithNoBars_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
            Assert.Equal("Market data not found for AUDUSD.FXCM-1-MINUTE[ASK]", result.Message);
        }

        [Fact]
        internal void GetBars_WithBarOutOfRange_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create(Duration.FromHours(1));

            this.repository.Add(barType, new[] { bar });

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        internal void GetBars_WithRangeOfOneBar_ReturnsExpectedBars()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.repository.Add(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(bar1, result.Value.Bars[0]);
            Assert.Equal(bar2, result.Value.Bars[1]);
            Assert.Equal(bar3, result.Value.Bars[2]);
        }

        [Fact]
        internal void GetBars_WithRangeOfThreeBars_ReturnsExpectedThreeBars()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.repository.Add(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Bars.Length);
        }

        [Fact]
        internal void GetBars_WithThreeBarsSpreadAcrossThreeDays_ReturnsExpectedBars()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            this.repository.Add(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.repository.GetBars(barType);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value.Bars.Length);
        }

        [Fact]
        internal void Test_can_get_sorted_bar_keys_by_symbol_and_resolution()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var barType2 = StubBarType.GBPUSD_Second();

            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(2));
            var bar4 = StubBarData.Create(Duration.FromDays(3));
            var bar5 = StubBarData.Create(Duration.FromDays(4));
            var bar6 = StubBarData.Create(Duration.FromDays(5));

            this.repository.Add(barType, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });
            this.repository.Add(barType2, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });

            // Act
            var result = this.repository.GetSortedKeysBySymbolStructure(BarStructure.Minute);

            foreach (var symbol in result)
            {
                this.output.WriteLine(symbol.Key);

                foreach (var key in symbol.Value)
                {
                    this.output.WriteLine(key);
                }
            }

            // Assert
            Assert.Single(result.Keys);
            Assert.Equal(6, result["FXCM:AUDUSD"].Count);
        }

        [Theory]
        [InlineData(0, 1, 2)]
        [InlineData(1, 1, 2)]
        [InlineData(1, 2, 3)]
        internal void OrganizeBarsByDay(
            int offset1,
            int offset2,
            int expectedCount)
        {
            // Arrange
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(Duration.FromDays(offset1));
            var bar4 = StubBarData.Create(Duration.FromDays(offset2));

            var barList = new[] { bar1, bar2, bar3, bar4 };

            // Act
            var result = RedisBarRepository.OrganizeBarsByDay(barList);

            // Assert
            Assert.Equal(expectedCount, result.Keys.Count);
        }

        private void PrintRepositoryStatus(BarType barType)
        {
            var barsQuery = this.repository.GetBars(barType);

            if (barsQuery.IsFailure)
            {
                this.output.WriteLine(barsQuery.Message);

                return;
            }

            var bars = this.repository.GetBars(barType).Value.Bars;

            foreach (var bar in bars)
            {
                this.output.WriteLine(bar.ToString());
            }
        }
    }
}
