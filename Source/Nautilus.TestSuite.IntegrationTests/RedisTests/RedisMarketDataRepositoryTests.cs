// -------------------------------------------------------------------------------------------------
// <copyright file="RedisTickRepositoryTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Common.Data;
using Nautilus.Redis;
using Nautilus.Redis.Data;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsShouldBeDocumented", Justification = "Test Suite")]
    public sealed class RedisMarketDataRepositoryTests : IDisposable
    {
        private readonly ConnectionMultiplexer redisConnection;
        private readonly RedisMarketDataRepository repository;

        public RedisMarketDataRepositoryTests(ITestOutputHelper output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            this.redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            this.repository = new RedisMarketDataRepository(
                container,
                DataBusFactory.Create(container),
                this.redisConnection);

            this.redisConnection.GetServer(RedisConstants.Localhost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        public void Dispose()
        {
            // Tear Down
            this.redisConnection.GetServer(RedisConstants.Localhost, RedisConstants.DefaultPort).FlushAllDatabases();
        }

        [Fact] internal void Ingest_WithValidTick_AddsTickToRepository()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var tick = StubTickProvider.Create(audusd.Symbol);

            this.repository.Update(audusd);

            // Act
            this.repository.Ingest(tick);

            // Assert
            Assert.True(this.repository.TicksExist(audusd.Symbol));
            Assert.Equal(1, this.repository.TicksCount(audusd.Symbol));
        }

        [Fact]
        internal void Add_MultipleTicksDifferentSymbols_AddsTicksToRepository()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var eurusd = StubInstrumentProvider.EURUSD();

            this.repository.Update(audusd);
            this.repository.Update(eurusd);

            var tick1 = StubTickProvider.Create(audusd.Symbol);
            var tick2 = StubTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubTickProvider.Create(eurusd.Symbol, StubZonedDateTime.UnixEpoch());
            var tick5 = StubTickProvider.Create(eurusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Act
            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);
            this.repository.Ingest(tick3);
            this.repository.Ingest(tick4);
            this.repository.Ingest(tick5);

            // Assert
            Assert.Equal(3, this.repository.TicksCount(audusd.Symbol));
            Assert.Equal(2, this.repository.TicksCount(eurusd.Symbol));
        }

        [Fact]
        internal void GetTicks_WithNoTicks_ReturnsEmptyArray()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            // Act
            var result = this.repository.GetTicks(audusd.Symbol);

            // Assert
            Assert.False(this.repository.TicksExist(audusd.Symbol));
            Assert.Equal(0, this.repository.TicksCount(audusd.Symbol));
            Assert.Empty(result);
        }

        [Fact]
        internal void GetTicks_WithOneTick_ReturnsCorrectTick()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            var tick1 = StubTickProvider.Create(audusd.Symbol);

            this.repository.Ingest(tick1);

            // Act
            var result = this.repository.GetTicks(audusd.Symbol);

            // Assert
            Assert.Equal(1, this.repository.TicksCount(audusd.Symbol));
            Assert.Single(result);
            Assert.Equal(tick1, result[0]);
        }

        [Fact]
        internal void GetBars_WithNoTicksIngested_ReturnsEmptyArray()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var barType = StubBarType.AUDUSD_OneMinuteAsk();
            this.repository.Update(audusd);

            // Act
            var result = this.repository.GetBars(StubBarType.AUDUSD_OneMinuteAsk());

            // Assert
            Assert.False(this.repository.BarsExist(barType));
            Assert.Equal(0, this.repository.BarsCount(barType));
            Assert.Empty(result.Bars);
        }
    }
}
