// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Redis;
    using Nautilus.Redis.Data;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using StackExchange.Redis;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisTickRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisTickRepository repository;

        public RedisTickRepositoryTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            this.logger = containerFactory.LoggingAdapter;
            var container = containerFactory.Create();

            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.repository = new RedisTickRepository(container, this.redisConnection);

            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        public void Dispose()
        {
            // Tear Down
            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        [Fact]
        internal void TicksCount_WithNoTicks_ReturnsZero()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));

            // Act
            // Assert
            Assert.Equal(0, this.repository.TicksCount(symbol));
            Assert.Equal(0, this.repository.TicksCount());
        }

        [Fact]
        internal void Add_WithOneTick_AddsTickToRepository()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick = StubTickProvider.Create(symbol);

            // Act
            this.repository.Add(tick);

            // Assert
            Assert.Equal(1, this.repository.TicksCount(symbol));
            Assert.Equal(1, this.repository.TicksCount());
        }

        [Fact]
        internal void Add_MultipleTicksDifferentSymbols_AddsTickToRepository()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var gbpusd = new Symbol("GBPUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch());
            var tick5 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Act
            this.repository.Add(tick1);
            this.repository.Add(tick2);
            this.repository.Add(tick3);
            this.repository.Add(tick4);
            this.repository.Add(tick5);

            // Assert
            Assert.Equal(3, this.repository.TicksCount(audusd));
            Assert.Equal(2, this.repository.TicksCount(gbpusd));
            Assert.Equal(5, this.repository.TicksCount());
        }

        [Fact]
        internal void GetKeysSorted_WithSymbol_ReturnsCorrectlySortedKeys()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));

            this.repository.Add(tick1);
            this.repository.Add(tick3);  // Out of order on purpose
            this.repository.Add(tick2);  // Out of order on purpose

            // Act
            var result = this.repository.GetKeysSorted(audusd);

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(3, result.Value.Length);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-01", result.Value[0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-02", result.Value[1]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-03", result.Value[2]);
        }

        [Fact]
        internal void GetKeysSorted_WithSeveralTicks_ReturnsCorrectlySortedKeys()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var gbpusd = new Symbol("GBPUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch());
            var tick5 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            this.repository.Add(tick1);
            this.repository.Add(tick2);
            this.repository.Add(tick3);
            this.repository.Add(tick5);  // Out of order on purpose
            this.repository.Add(tick4);  // Out of order on purpose

            // Act
            var result = this.repository.GetKeysSorted();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(3, result["FXCM:AUDUSD"].Count);
            Assert.Equal(2, result["FXCM:GBPUSD"].Count);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-01", result["FXCM:AUDUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-02", result["FXCM:AUDUSD"][1]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-03", result["FXCM:AUDUSD"][2]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-01", result["FXCM:GBPUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-02", result["FXCM:GBPUSD"][1]);
        }
    }
}
