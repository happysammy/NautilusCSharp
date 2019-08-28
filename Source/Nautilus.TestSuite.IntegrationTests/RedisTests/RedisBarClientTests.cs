// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarClientTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using StackExchange.Redis;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisBarClientTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisBarClient client;

        public RedisBarClientTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.client = new RedisBarClient(this.redisConnection);

            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        public void Dispose()
        {
            // Tear Down
            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
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
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.client.KeysCount(barType);

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
            var result = this.client.KeyExists(barKey);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void KeyExists_WithKeyInRedis_ReturnsTrue()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var barKey = KeyProvider.GetBarKey(barType, new DateKey(StubZonedDateTime.UnixEpoch()));
            var bar = new Bar(0.80000M, 0.80010M, 0.79990M, 0.80001M, 1000000, StubZonedDateTime.UnixEpoch());

            this.client.AddBars(barType, new[] { bar });

            // Act
            var result = this.client.KeyExists(barKey);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void GetAllKeys_WithNoKeysInRedis_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.client.GetAllSortedKeys(barType);

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

            this.client.AddBars(barType, new[] { bar1, bar2 });

            // Act
            var result = this.client.GetAllSortedKeys(barType);

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
            var result = KeyProvider.GetDateKeys(fromDateTime, toDateTime);

            // Assert
            Assert.Equal(dateKeyCount, result.Count);
            Assert.Equal(expectedKey, result.Last().ToString());
            result.ForEach(dk => this.output.WriteLine(dk.ToString()));
        }

        [Fact]
        internal void AddBar_WithOneBar_AddsBarToRedis()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            var result = this.client.AddBar(barType, bar);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);

            Assert.Equal(1, this.client.BarsCount(barType));
            Assert.Equal(1, this.client.KeysCount(barType));
            Assert.Equal(1, this.client.AllBarsCount());
            Assert.Equal(1, this.client.AllKeysCount());
        }

        [Fact]
        internal void AddBars_WithOneBar_AddsBarToRedis()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            var result = this.client.AddBars(barType, new[] { bar });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.client.BarsCount(barType));
            Assert.Equal(1, this.client.KeysCount(barType));
            Assert.Equal(1, this.client.AllBarsCount());
            Assert.Equal(1, this.client.AllKeysCount());
        }

        [Fact]
        internal void AddBars_OneBarWithOneBarAlreadyPersisted_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);

            this.client.AddBars(barType, new[] { bar1 });

            // Act
            var result = this.client.AddBars(barType, new[] { bar2 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(2, this.client.BarsCount(barType));
            Assert.Equal(1, this.client.KeysCount(barType));
            Assert.Equal(2, this.client.AllBarsCount());
            Assert.Equal(1, this.client.AllKeysCount());
        }

        [Fact]
        internal void AddBars_OneBarWithTwoBarsAlreadyPersistedWithAGap_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(2);
            var bar3 = StubBarData.Create(4);

            this.client.AddBars(barType, new[] { bar1, bar2 });

            // Act
            var result = this.client.AddBars(barType, new[] { bar3 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, this.client.BarsCount(barType));
            Assert.Equal(3, this.client.AllBarsCount());
        }

        [Fact]
        internal void AddBars_OneBarWithTwoAlreadyPersistedAtDifferentDay_AddsBarToRedisAtCorrectIndex()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(Duration.FromDays(1));
            var bar3 = StubBarData.Create(Duration.FromDays(1) + Duration.FromMinutes(1));

            this.client.AddBars(barType, new[] { bar1, bar2 });

            // Act
            var result = this.client.AddBars(barType, new[] { bar3 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(3, this.client.BarsCount(barType));
            Assert.Equal(3, this.client.AllBarsCount());
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
            var result = this.client.AddBars(barType, new[] { bar1, bar2, bar3, bar4, bar5 });

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.client.BarsCount(barType));
            Assert.Equal(5, this.client.AllBarsCount());
        }

        [Fact]
        internal void GetBarString_WithOneBar_ReturnsBar()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            this.client.AddBars(barType, new[] { bar });

            // Act
            var result = this.client.GetBar(barType, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Value.ToString());
            Assert.True(result.IsSuccess);
            Assert.Equal("1.00000,1.00000,1.00000,1.00000,1000000,1970-01-01T00:00:00.000Z", result.Value.ToString());
        }

        [Fact]
        internal void GetBar_WithNoBars_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();

            // Act
            var result = this.client.GetBar(barType, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetBar_WithOneBar_ReturnsBar()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create();
            this.client.AddBars(barType, new[] { bar });

            // Act
            var result = this.client.GetBar(barType, bar.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(bar, result.Value);
        }

        [Fact]
        internal void GetBar_WithMultipleBars_ReturnsExpectedBar()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            this.client.AddBars(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBar(barType, bar2.Timestamp);

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(bar2, result.Value);
        }

        [Fact]
        internal void GetBars_WithNoBars_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.client.GetBars(barType, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
            Assert.Equal("No market data found for AUDUSD.FXCM-1-MINUTE[ASK].", result.Message);
        }

        [Fact]
        internal void GetBars_WithBarOutOfRange_ReturnsQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarData.Create(Duration.FromHours(1));

            this.client.AddBars(barType, new[] { bar });

            // Act
            var result = this.client.GetBars(barType, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(2));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsFailure);
            Assert.Equal("No market data found for AUDUSD.FXCM-1-MINUTE[ASK] in time range from 1970-01-01T00:00:00.000Z to 1970-01-01T00:02:00.000Z.", result.Message);
        }

        [Fact]
        internal void GetBars_WithRangeOfOneBar_ReturnsExpectedBars()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.client.AddBars(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barType, bar2.Timestamp, bar2.Timestamp);

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
            var barType = StubBarType.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);

            this.client.AddBars(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barType, bar1.Timestamp, bar3.Timestamp);

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
            this.client.AddBars(barType, new[] { bar1, bar2, bar3 });

            // Act
            var result = this.client.GetBars(barType, bar1.Timestamp, bar3.Timestamp);

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

            this.client.AddBars(barType, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });
            this.client.AddBars(barType2, new[] { bar1, bar2, bar3, bar4, bar5, bar6 });

            // Act
            var result = this.client.GetSortedKeysBySymbolResolution(Resolution.MINUTE);

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
            var result = this.client.OrganizeBarsByDay(barList);

            // Assert
            Assert.Equal(expectedCount, result.Keys.Count);
        }
    }
}
