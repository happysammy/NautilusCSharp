// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarPublisherTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System;
    using Nautilus.DomainModel.Events;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using ServiceStack.Redis;
    using Xunit;
    using Xunit.Abstractions;

    public class RedisBarPublisherTests
    {
        private readonly ITestOutputHelper output;
        private readonly RedisBarPublisher barPublisher;

        public RedisBarPublisherTests(ITestOutputHelper output)
        {
            this.output = output;
            var localHost = RedisConstants.LocalHost;
            var redisClientManager = new BasicRedisClientManager(new[] { localHost }, new[] { localHost });

            this.barPublisher = new RedisBarPublisher(redisClientManager);
        }

        [Fact]
        internal void Test_can_publish_bar_data_events()
        {
            // Arrange
            var barType = StubBarType.AUDUSD();
            var bar = StubBarBuilder.Build();
            var tick = StubTickFactory.Create(barType.Symbol);

            var barDataEvent = new BarDataEvent(
                barType,
                bar,
                tick, 0.00005m,
                false,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.barPublisher.Publish(barDataEvent);

            // Assert
        }
    }
}
