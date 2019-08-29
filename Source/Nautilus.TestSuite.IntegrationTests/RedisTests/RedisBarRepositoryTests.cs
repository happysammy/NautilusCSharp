// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisBarRepository repository;

        public RedisBarRepositoryTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.repository = new RedisBarRepository(this.redisConnection);

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
            var result = this.repository.Add(marketData);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barType));
            Assert.Equal("Added 1 bars to AUDUSD.FXCM-1-MINUTE[ASK] (TotalCount=1).", result.Message);
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
            var result = this.repository.Add(marketData);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(5, this.repository.BarsCount(barType));
            Assert.Equal("Added 5 bars to AUDUSD.FXCM-1-MINUTE[ASK] (TotalCount=5).", result.Message);
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
            var result = this.repository.Add(marketData2);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsSuccess);
            Assert.Equal(8, this.repository.BarsCount(barType));
            Assert.Equal(8, this.repository.AllBarsCount());
            Assert.Equal("Added 3 bars to AUDUSD.FXCM-1-MINUTE[ASK] (TotalCount=8).", result.Message);
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
            var result = this.repository.Add(marketData2);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsSuccess);
            Assert.Equal(8, this.repository.BarsCount(barType));
            Assert.Equal(8, this.repository.AllBarsCount());
            Assert.Equal("Added 3 bars to AUDUSD.FXCM-1-MINUTE[ASK] (TotalCount=8).", result.Message);
        }

        [Fact]
        internal void Find_WithNoMarketData_ReturnsExpectedQueryFailure()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();

            // Act
            var result = this.repository.Find(barType, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barType);
            Assert.True(result.IsFailure);
            Assert.Equal(0, this.repository.AllBarsCount());
            Assert.Equal(0, this.repository.BarsCount(barType));
            Assert.Equal("No market data found for AUDUSD.FXCM-1-MINUTE[ASK].", result.Message);
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
            var result = this.repository.Find(barType2, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barType1));
            Assert.Equal(0, this.repository.BarsCount(barType2));
            Assert.Equal("No market data found for GBPUSD.FXCM-1-MINUTE[BID].", result.Message);
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
            var result = this.repository.Find(barType, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch());

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
            var result = this.repository.Find(barType, StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(3));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(3, result.Value.Bars.Length);
            Assert.Equal(5, this.repository.BarsCount(barType));
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
            this.repository.TrimToDays(Resolution.SECOND, 1);
            this.repository.TrimToDays(Resolution.MINUTE, 10);

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
            this.repository.TrimToDays(Resolution.SECOND, 1);
            this.repository.TrimToDays(Resolution.MINUTE, 5);

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
            this.repository.TrimToDays(Resolution.MINUTE, 5);

            // Assert
            this.PrintRepositoryStatus(barType1);
            this.PrintRepositoryStatus(barType2);
            Assert.Equal(5, this.repository.BarsCount(barType1));
            Assert.Equal(6, this.repository.BarsCount(barType2));
        }

        private void PrintRepositoryStatus(BarType barType)
        {
            var barsQuery = this.repository.FindAll(barType);

            if (barsQuery.IsFailure)
            {
                this.output.WriteLine(barsQuery.Message);

                return;
            }

            var bars = this.repository.FindAll(barType).Value.Bars;

            foreach (var bar in bars)
            {
                this.output.WriteLine(bar.ToString());
            }
        }
    }
}
