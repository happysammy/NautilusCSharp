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
    using Nautilus.Data.Keys;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Redis;
    using Nautilus.Redis.Data;
    using Nautilus.Redis.Data.Internal;
    using Nautilus.Serialization.Bson;
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
            this.repository = new RedisTickRepository(container, new TickDataSerializer(), this.redisConnection);

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
            var result = this.repository.GetKeysSorted(KeyProvider.GetTicksPattern(audusd));

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
            var tick0 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() - Duration.FromDays(1));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(12));
            var tick4 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(22));
            var tick5 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(23));
            var tick6 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(30));
            var tick7 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(31));
            var tick8 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(32));
            var tick9 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(40));

            var tick11 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch());
            var tick12 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick13 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(10));

            this.repository.Add(tick0);
            this.repository.Add(tick1);
            this.repository.Add(tick2);
            this.repository.Add(tick3);
            this.repository.Add(tick4);
            this.repository.Add(tick5);
            this.repository.Add(tick6);
            this.repository.Add(tick7);
            this.repository.Add(tick8);
            this.repository.Add(tick9);

            this.repository.Add(tick12);  // Out of order on purpose
            this.repository.Add(tick11);  // Out of order on purpose
            this.repository.Add(tick13);  // Out of order on purpose

            // Act
            var result = this.repository.GetKeysSortedBySymbol(
                KeyProvider.GetTicksPattern(),
                3,
                4);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(10, result["FXCM:AUDUSD"].Count);
            Assert.Equal(3, result["FXCM:GBPUSD"].Count);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1969-12-31", result["FXCM:AUDUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-01", result["FXCM:AUDUSD"][1]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-03", result["FXCM:AUDUSD"][2]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-13", result["FXCM:AUDUSD"][3]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-23", result["FXCM:AUDUSD"][4]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-24", result["FXCM:AUDUSD"][5]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-31", result["FXCM:AUDUSD"][6]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-02-01", result["FXCM:AUDUSD"][7]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-02-02", result["FXCM:AUDUSD"][8]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-02-10", result["FXCM:AUDUSD"][9]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-01", result["FXCM:GBPUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-02", result["FXCM:GBPUSD"][1]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-11", result["FXCM:GBPUSD"][2]);
        }

        [Fact]
        internal void TrimToDays_WithMultipleDaysOfTicks_CorrectlyTrims()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var gbpusd = new Symbol("GBPUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(3));
            var tick5 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(4));
            var tick6 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(3));
            var tick7 = StubTickProvider.Create(gbpusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(4));

            this.repository.Add(tick1);
            this.repository.Add(tick2);
            this.repository.Add(tick3);
            this.repository.Add(tick4);
            this.repository.Add(tick5);
            this.repository.Add(tick6);
            this.repository.Add(tick7);

            // Act
            this.repository.TrimToDays(2);
            var result = this.repository.GetKeysSortedBySymbol(
                KeyProvider.GetTicksPattern(),
                3,
                4);

            // Assert
            Assert.Equal(4, this.repository.TicksCount());
            Assert.Equal(2, this.repository.TicksCount(audusd));
            Assert.Equal(2, this.repository.TicksCount(gbpusd));
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-04", result["FXCM:AUDUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:AUDUSD:1970-01-05", result["FXCM:AUDUSD"][1]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-04", result["FXCM:GBPUSD"][0]);
            Assert.Equal("NautilusData:Data:Ticks:FXCM:GBPUSD:1970-01-05", result["FXCM:GBPUSD"][1]);
        }

        [Fact]
        internal void GetTicks_WithNoTicks_ReturnsQueryFailure()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));

            // Act
            var result = this.repository.GetTicks(
                audusd,
                new DateKey(StubZonedDateTime.UnixEpoch() - Duration.FromDays(2)),
                new DateKey(StubZonedDateTime.UnixEpoch()));

            // Assert
            Assert.Equal(0, this.repository.TicksCount());
            Assert.Equal(0, this.repository.TicksCount(audusd));
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetTicks_WithOneTicks_ReturnsCorrectTick()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var ticks = new[] { tick1 };

            this.repository.Add(tick1);

            // Act
            var result = this.repository.GetTicks(audusd);

            this.output.WriteLine(result.Message);

            // Assert
            Assert.Equal(1, this.repository.TicksCount());
            Assert.Equal(1, this.repository.TicksCount(audusd));
            Assert.True(result.IsSuccess);
            Assert.Equal(ticks, result.Value);
        }

        [Fact]
        internal void GetTicks_WithTimeRangeWithOneTicks_ReturnsCorrectTick()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var ticks = new[] { tick1 };

            this.repository.Add(tick1);

            // Act
            var result = this.repository.GetTicks(
                audusd,
                new DateKey(StubZonedDateTime.UnixEpoch() - Duration.FromDays(2)),
                new DateKey(StubZonedDateTime.UnixEpoch()));

            this.output.WriteLine(result.Message);

            // Assert
            Assert.Equal(1, this.repository.TicksCount());
            Assert.Equal(1, this.repository.TicksCount(audusd));
            Assert.True(result.IsSuccess);
            Assert.Equal(ticks, result.Value);
        }

        [Fact]
        internal void GetTicks_WithMultipleDaysOfTicks_ReturnsCorrectTicks()
        {
            // Arrange
            var audusd = new Symbol("AUDUSD", new Venue("FXCM"));
            var tick1 = StubTickProvider.Create(audusd);
            var tick2 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(3));
            var tick5 = StubTickProvider.Create(audusd, StubZonedDateTime.UnixEpoch() + Duration.FromDays(4));
            var ticks = new[] { tick1, tick2, tick3, tick4, tick5 };

            this.repository.Add(tick1);
            this.repository.Add(tick2);
            this.repository.Add(tick3);
            this.repository.Add(tick4);
            this.repository.Add(tick5);

            // Act
            var result = this.repository.GetTicks(
                audusd,
                new DateKey(StubZonedDateTime.UnixEpoch()),
                new DateKey(StubZonedDateTime.UnixEpoch() + Duration.FromDays(4)));

            LogDumper.Dump(this.logger, this.output);

            // Assert
            Assert.Equal(5, this.repository.TicksCount());
            Assert.Equal(5, this.repository.TicksCount(audusd));
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Value.Length);
            Assert.Equal(ticks, result.Value);
        }
    }
}
