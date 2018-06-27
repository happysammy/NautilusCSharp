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
    using Nautilus.Common.Interfaces;
    using NodaTime;
    using ServiceStack.Redis;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Compression;
    using Nautilus.Database.Integrity.Checkers;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RedisInstrumentRepositoryTests : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly IInstrumentRepository repository;

        public RedisInstrumentRepositoryTests(ITestOutputHelper output)
        {
            RedisServiceStack.ConfigureServiceStack();

            this.output = output;
            var localHost = RedisConstants.LocalHost;
            var clientManager = new BasicRedisClientManager(new[] { localHost }, new[] { localHost });

            // Data compression off so that redis-cli is readable.
            this.repository = new RedisInstrumentRepository(clientManager);

            this.repository.DeleteAll();
        }

        public void Dispose()
        {
            this.repository.DeleteAll();
        }

        [Fact]
        internal void Test_can_add_one_instrument()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();

            // Act
            var result = this.repository.Add(instrument);

            // Assert
            this.output.WriteLine(result.Message);
        }
    }
}
