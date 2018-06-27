// -------------------------------------------------------------------------------------------------
// <copyright file="RedisInstrumentRepositoryTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ServiceStack.Redis;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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

            this.repository = new RedisInstrumentRepository(clientsManager);

            this.clientsManager.GetClient().FlushAll();
        }

        public void Dispose()
        {
            //this.clientsManager.GetClient().FlushAll();
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
            var result = repository.GetAllKeys();

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
    }
}
