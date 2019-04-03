// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using ServiceStack.Redis;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisInstrumentRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly IRedisClientsManager clientsManager;
        private readonly RedisInstrumentRepository repository;

        public RedisInstrumentRepositoryTests(ITestOutputHelper output)
        {
            RedisServiceStack.ConfigureServiceStack();

            this.output = output;
            this.clientsManager = new BasicRedisClientManager(
                new[] { RedisConstants.LocalHost },
                new[] { RedisConstants.LocalHost });

            this.repository = new RedisInstrumentRepository(this.clientsManager);

            this.clientsManager.GetClient().FlushAll();
        }

        public void Dispose()
        {
            this.clientsManager.GetClient().FlushAll();
        }

        [Fact]
        internal void Test_can_add_one_instrument()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();

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
            var instrument = StubInstrumentFactory.AUDUSD();
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
            var instrument1 = StubInstrumentFactory.AUDUSD();
            var instrument2 = StubInstrumentFactory.EURUSD();
            var instrument3 = StubInstrumentFactory.USDJPY();

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
            var instrument1 = StubInstrumentFactory.AUDUSD();
            var instrument2 = StubInstrumentFactory.EURUSD();
            var instrument3 = StubInstrumentFactory.USDJPY();

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
            var instrument = StubInstrumentFactory.AUDUSD();
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
            var instrument1 = StubInstrumentFactory.AUDUSD();
            var instrument2 = StubInstrumentFactory.EURUSD();
            var instrument3 = StubInstrumentFactory.USDJPY();

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
        }
    }
}
