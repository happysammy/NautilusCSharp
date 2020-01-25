// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Redis;
    using Nautilus.Redis.Data;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using StackExchange.Redis;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisInstrumentRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisInstrumentRepository repository;

        public RedisInstrumentRepositoryTests(ITestOutputHelper output)
        {
            this.output = output;
            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();

            this.repository = new RedisInstrumentRepository(this.redisConnection);
        }

        public void Dispose()
        {
            // Tear Down
            this.redisConnection.GetServer(RedisConstants.LocalHost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        [Fact]
        internal void Test_can_add_one_instrument()
        {
            // Arrange
            var instrument = StubInstrumentProvider.AUDUSD();

            // Act
            var result = this.repository.Add(instrument, StubZonedDateTime.UnixEpoch());
            var count = this.repository.GetAllKeys().Count;

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, count);
            this.output.WriteLine(result.Message);
        }

        [Fact]
        internal void Test_can_delete_one_instrument()
        {
            // Arrange
            var instrument = StubInstrumentProvider.AUDUSD();
            this.repository.Add(instrument, StubZonedDateTime.UnixEpoch());

            // Act
            this.repository.Delete(instrument.Symbol);
            var result = this.repository.GetAllKeys();

            // Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        internal void Test_can_add_collection_of_instruments()
        {
            // Arrange
            var instrument1 = StubInstrumentProvider.AUDUSD();
            var instrument2 = StubInstrumentProvider.EURUSD();
            var instrument3 = StubInstrumentProvider.USDJPY();

            // Act
            var result1 = this.repository.Add(instrument1, StubZonedDateTime.UnixEpoch());
            var result2 = this.repository.Add(instrument2, StubZonedDateTime.UnixEpoch());
            var result3 = this.repository.Add(instrument3, StubZonedDateTime.UnixEpoch());
            var count = this.repository.GetAllKeys().Count;

            // Assert
            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsSuccess);
            Assert.True(result3.IsSuccess);
            Assert.Equal(3, count);
            this.output.WriteLine(result1.Message);
            this.output.WriteLine(result2.Message);
            this.output.WriteLine(result3.Message);
        }

        [Fact]
        internal void Test_can_delete_all_instruments()
        {
            // Arrange
            var instrument1 = StubInstrumentProvider.AUDUSD();
            var instrument2 = StubInstrumentProvider.EURUSD();
            var instrument3 = StubInstrumentProvider.USDJPY();

            this.repository.Add(instrument1, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument2, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument3, StubZonedDateTime.UnixEpoch());

            // Act
            this.repository.DeleteAll();
            var result = this.repository.GetAllKeys();

            // Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        internal void Test_can_find_one_instrument()
        {
            // Arrange
            var instrument = StubInstrumentProvider.AUDUSD();
            this.repository.Add(instrument, StubZonedDateTime.UnixEpoch());
            this.repository.CacheAll();

            // Act
            var result = this.repository.FindInCache(instrument.Symbol);

            // Assert
            Assert.Equal(instrument, result.Value);
        }

        [Fact]
        internal void Test_can_cache_multiple_instruments()
        {
            // Arrange
            this.output.WriteLine(nameof(Instrument.TickPrecision));
            var instrument1 = StubInstrumentProvider.AUDUSD();
            var instrument2 = StubInstrumentProvider.EURUSD();
            var instrument3 = StubInstrumentProvider.USDJPY();

            this.repository.Add(instrument1, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument2, StubZonedDateTime.UnixEpoch());
            this.repository.Add(instrument3, StubZonedDateTime.UnixEpoch());

            // Act
            this.repository.CacheAll();
            var result1 = this.repository.FindInCache(instrument1.Symbol);
            var result2 = this.repository.FindInCache(instrument2.Symbol);
            var result3 = this.repository.FindInCache(instrument3.Symbol);

            // Assert
            Assert.Equal(instrument1, result1.Value);
            Assert.Equal(instrument2, result2.Value);
            Assert.Equal(instrument3, result3.Value);

            this.output.WriteLine(nameof(Instrument.TickPrecision));
        }
    }
}
