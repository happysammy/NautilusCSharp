// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepositoryTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NodaTime;
    using ServiceStack.Redis;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Compression;
    using Nautilus.Data.Types;
    using Nautilus.Database.Integrity.Checkers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisBarRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly RedisBarRepository repository;

        public RedisBarRepositoryTests(ITestOutputHelper output)
        {
            RedisServiceStack.ConfigureServiceStack();

            this.output = output;
            var localHost = RedisConstants.LocalHost;
            var redisClientManager = new BasicRedisClientManager(new[] { localHost }, new[] { localHost });

            // Data compression off so that redis-cli is readable.
            this.repository = new RedisBarRepository(
                redisClientManager,
                localHost,
                Duration.FromSeconds(10),
                new LZ4DataCompressor(false));

            this.repository.FlushAll("YES");
        }

        public void Dispose()
        {
            this.repository.FlushAll("YES");
        }

        [Fact]
        internal void Add_WithOneBar_AddsBarToRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barSpec, new[] { bar });

            // Act
            var result = this.repository.Add(marketData);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barSpec);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barSpec));
            Assert.Equal("Added 1 bars to AUDUSD.Dukascopy-1-Minute[Ask] (TotalCount=1)", result.Message);
        }

        [Fact]
        internal void Add_WithMultipleBars_AddsBarsToRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);
            var marketData = new BarDataFrame(barSpec, new[] { bar1, bar2, bar3, bar4, bar5 });

            // Act
            var result = this.repository.Add(marketData);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barSpec);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(5, this.repository.BarsCount(barSpec));
            Assert.Equal("Added 5 bars to AUDUSD.Dukascopy-1-Minute[Ask] (TotalCount=5)", result.Message);
        }

        [Fact]
        internal void Add_WithMultipleBarsAndBarsAlreadyPersisted_AddsExpectedBarsToRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            var bar6 = StubBarData.Create(5);
            var bar7 = StubBarData.Create(6);
            var bar8 = StubBarData.Create(7);
            var marketData1 = new BarDataFrame(barSpec, new[] { bar1, bar2, bar3, bar4, bar5 });
            var marketData2 = new BarDataFrame(barSpec, new[] { bar6, bar7, bar8 });

            this.repository.Add(marketData1);

            // Act
            var result = this.repository.Add(marketData2);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barSpec);
            Assert.True(result.IsSuccess);
            Assert.Equal(8, this.repository.BarsCount(barSpec));
            Assert.Equal(8, this.repository.AllBarsCount());
            Assert.Equal("Added 3 bars to AUDUSD.Dukascopy-1-Minute[Ask] (TotalCount=8)", result.Message);
        }

        [Fact]
        internal void Add_WithMultipleBarsAlreadyPersisted_AddsNewBarsToRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);

            var bar6 = StubBarData.Create(5);
            var bar7 = StubBarData.Create(6);
            var bar8 = StubBarData.Create(7);
            var marketData1 = new BarDataFrame(barSpec, new[] { bar1, bar2, bar3, bar4, bar5 });
            var marketData2 = new BarDataFrame(barSpec, new[] { bar6, bar7, bar8 });

            this.repository.Add(marketData1);

            // Act
            var result = this.repository.Add(marketData2);

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barSpec);
            Assert.True(result.IsSuccess);
            Assert.Equal(8, this.repository.BarsCount(barSpec));
            Assert.Equal(8, this.repository.AllBarsCount());
            Assert.Equal("Added 3 bars to AUDUSD.Dukascopy-1-Minute[Ask] (TotalCount=8)", result.Message);
        }

        [Fact]
        internal void Find_WithNoMarketData_ReturnsExpectedQueryFailure()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();

            // Act
            var result = this.repository.Find(barSpec, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Assert
            this.output.WriteLine(result.Message);
            this.PrintRepositoryStatus(barSpec);
            Assert.True(result.IsFailure);
            Assert.Equal(0, this.repository.AllBarsCount());
            Assert.Equal(0, this.repository.BarsCount(barSpec));
            Assert.Equal("QueryResult Failure (No market data found for AUDUSD.Dukascopy-1-Minute[Ask]).", result.FullMessage);
        }

        [Fact]
        internal void Find_WithOtherMarketData_ReturnsExpectedQueryFailure()
        {
            // Arrange
            var barSpec1 = StubSymbolBarSpec.AUDUSD();
            var barSpec2 = StubSymbolBarSpec.GBPUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barSpec1, new[] { bar });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.Find(barSpec2, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Equal(1, this.repository.BarsCount(barSpec1));
            Assert.Equal(0, this.repository.BarsCount(barSpec2));
            Assert.Equal("QueryResult Failure (No market data found for GBPUSD.Dukascopy-1-Minute[Bid]).", result.FullMessage);
        }

        [Fact]
        internal void Find_WhenOnlyOneBarPersisted_ReturnsExpectedMarketDataFromRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar = StubBarData.Create();
            var marketData = new BarDataFrame(barSpec, new[] { bar });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.Find(barSpec, StubZonedDateTime.UnixEpoch(), StubZonedDateTime.UnixEpoch());

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(1, this.repository.AllBarsCount());
            Assert.Single(result.Value.Bars);
            Assert.Equal(1, this.repository.BarsCount(barSpec));
        }

        [Fact]
        internal void Find_WhenMultipleBarsPersisted_ReturnsExpectedMarketDataFromRepository()
        {
            // Arrange
            var barSpec = StubSymbolBarSpec.AUDUSD();
            var bar1 = StubBarData.Create();
            var bar2 = StubBarData.Create(1);
            var bar3 = StubBarData.Create(2);
            var bar4 = StubBarData.Create(3);
            var bar5 = StubBarData.Create(4);
            var marketData = new BarDataFrame(barSpec, new[] { bar1, bar2, bar3, bar4, bar5 });

            this.repository.Add(marketData);

            // Act
            var result = this.repository.Find(barSpec, StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1), StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(3));

            // Assert
            this.output.WriteLine(result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(5, this.repository.AllBarsCount());
            Assert.Equal(3, result.Value.Bars.Length);
            Assert.Equal(5, this.repository.BarsCount(barSpec));
        }

        private void PrintRepositoryStatus(SymbolBarSpec barSpec)
        {
            var barsQuery = this.repository.FindAll(barSpec);

            if (barsQuery.IsFailure)
            {
                this.output.WriteLine(barsQuery.Message);

                return;
            }

            var bars = this.repository.FindAll(barSpec).Value.Bars;

            foreach (var bar in bars)
            {
                this.output.WriteLine(bar.ToString());
            }

            var barsCheck = BarDataChecker.CheckBars(barSpec, this.repository.FindAll(barSpec).Value.Bars);
            barsCheck.Value.ForEach(a => this.output.WriteLine(a));
        }
    }
}
