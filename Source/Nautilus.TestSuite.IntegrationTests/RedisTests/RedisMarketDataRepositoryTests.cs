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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Common.Data;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.ValueObjects;
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
                this.redisConnection,
                1,
                new Dictionary<BarStructure, int> {{BarStructure.Minute, 1}});

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
            var tick = StubQuoteTickProvider.Create(audusd.Symbol);

            this.repository.Update(audusd);

            // Act
            this.repository.Ingest(tick);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.True(this.repository.TicksExist(audusd.Symbol));
            // Assert.Equal(1, this.repository.TicksCount(audusd.Symbol));
        }

        [Fact]
        internal void Ingest_MultipleTicksDifferentSymbols_CorrectlyAddsTicksToRepository()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var eurusd = StubInstrumentProvider.EURUSD();

            this.repository.Update(audusd);
            this.repository.Update(eurusd);

            var tick1 = StubQuoteTickProvider.Create(audusd.Symbol);
            var tick2 = StubQuoteTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));
            var tick3 = StubQuoteTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(2));
            var tick4 = StubQuoteTickProvider.Create(eurusd.Symbol, StubZonedDateTime.UnixEpoch());
            var tick5 = StubQuoteTickProvider.Create(eurusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromDays(1));

            // Act
            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);
            this.repository.Ingest(tick3);
            this.repository.Ingest(tick4);
            this.repository.Ingest(tick5);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(3, this.repository.TicksCount(audusd.Symbol));
            // Assert.Equal(2, this.repository.TicksCount(eurusd.Symbol));
        }

        [Fact]
        internal void GetTicks_WithNoTicks_ReturnsEmptyArray()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            // Act
            // var result = this.repository.GetTicks(audusd.Symbol, null, null, null);

            // Assert
            // Assert.False(this.repository.TicksExist(audusd.Symbol));
            // Assert.Equal(0, this.repository.TicksCount(audusd.Symbol));
            // Assert.Empty(result);
        }

        [Fact]
        internal void GetTicks_WithOneTick_ReturnsCorrectTick()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            var tick1 = StubQuoteTickProvider.Create(audusd.Symbol);

            this.repository.Ingest(tick1);

            // Act
            // var result = this.repository.GetTicks(audusd.Symbol, null, null, null);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(1, this.repository.TicksCount(audusd.Symbol));
            // Assert.Single(result);
            // Assert.Equal(tick1, result[0]);
        }

        [Fact]
        internal void GetTicks_WithLimitOne_ReturnsLastTick()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            var tick1 = StubQuoteTickProvider.Create(audusd.Symbol);
            var tick2 = StubQuoteTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(100));

            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);

            // Act
            // var result = this.repository.GetTicks(audusd.Symbol, null, null, 1);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(2, this.repository.TicksCount(audusd.Symbol));
            // Assert.Single(result);
            // Assert.Equal(tick2, result[0]);
        }

        [Fact]
        internal void GetTicks_WithFromDateTime_ReturnsTickInRange()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            var fromDateTime = StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1000);

            var tick1 = StubQuoteTickProvider.Create(audusd.Symbol);
            var tick2 = StubQuoteTickProvider.Create(audusd.Symbol, fromDateTime);

            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);

            // Act
            // var result = this.repository.GetTicks(audusd.Symbol, fromDateTime, null, null);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(2, this.repository.TicksCount(audusd.Symbol));
            // Assert.Single(result);
            // Assert.Equal(tick2, result[0]);
        }

        [Fact]
        internal void GetTicks_WithToDateTime_ReturnsTickInRange()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            this.repository.Update(audusd);

            var tick1 = StubQuoteTickProvider.Create(audusd.Symbol);
            var tick2 = StubQuoteTickProvider.Create(audusd.Symbol, StubZonedDateTime.UnixEpoch() + Duration.FromMilliseconds(1000));

            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);

            // Act
            // var result = this.repository.GetTicks(audusd.Symbol, null, StubZonedDateTime.UnixEpoch(), null);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(2, this.repository.TicksCount(audusd.Symbol));
            // Assert.Single(result);
            // Assert.Equal(tick1, result[0]);
        }

        [Fact]
        internal void GetBars_WithNoTicksIngested_ReturnsEmptyArray()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            // var barType = StubBarType.AUDUSD_OneMinuteAsk();
            this.repository.Update(audusd);

            // Act
            // var result = this.repository.GetBars(StubBarType.AUDUSD_OneMinuteAsk(), null, null, null);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.False(this.repository.BarsExist(barType));
            // Assert.Equal(0, this.repository.BarsCount(barType));
            // Assert.Empty(result.Bars);
        }

        [Fact]
        internal void GetBars_WithTicksCreatingBar_ReturnsExpectedBar()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            // var barType = StubBarType.AUDUSD_OneMinuteBid();
            this.repository.Update(audusd);

            var tick0 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00010, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00000, 5),
                Price.Create(1.00010, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(60));

            var tick2 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00030, 5),
                Price.Create(1.00040, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(62));

            var tick3 = new QuoteTick(
                audusd.Symbol,
                Price.Create(0.99980, 5),
                Price.Create(0.99990, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(63));

            var tick4 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00011, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(119));

            var tick5 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00011, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(121));

            this.repository.Ingest(tick0);
            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);
            this.repository.Ingest(tick3);
            this.repository.Ingest(tick4);
            this.repository.Ingest(tick5);

            // var expected = new Bar(
            //     Price.Create(1.00000, 5),
            //     Price.Create(1.00030, 5),
            //     Price.Create(0.99980, 5),
            //     Price.Create(1.00001, 5),
            //     Quantity.Create(4),
            //     StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(120));

            // Act
            // var result = this.repository.GetBars(barType, null, null, 1);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(6, this.repository.TicksCount(audusd.Symbol));
            // Assert.True(this.repository.BarsExist(barType));
            // Assert.Equal(2, this.repository.BarsCount(barType));
            // Assert.Single(result.Bars);
            // Assert.Equal(expected, result.Bars[0]);
        }

        [Fact]
        internal void GetMidBars_WithTicksCreatingBar_ReturnsExpectedBar()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            // var barType = StubBarType.AUDUSD_OneMinuteMid();
            this.repository.Update(audusd);

            var tick0 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00010, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch());

            var tick1 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00000, 5),
                Price.Create(1.00010, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(60));

            var tick2 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00030, 5),
                Price.Create(1.00040, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(62));

            var tick3 = new QuoteTick(
                audusd.Symbol,
                Price.Create(0.99980, 5),
                Price.Create(0.99990, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(63));

            var tick4 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00004, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(119));

            var tick5 = new QuoteTick(
                audusd.Symbol,
                Price.Create(1.00001, 5),
                Price.Create(1.00011, 5),
                Quantity.One(),
                Quantity.One(),
                StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(121));

            this.repository.Ingest(tick0);
            this.repository.Ingest(tick1);
            this.repository.Ingest(tick2);
            this.repository.Ingest(tick3);
            this.repository.Ingest(tick4);
            this.repository.Ingest(tick5);

            // var expected = new Bar(
            //     Price.Create(1.000050, 6),
            //     Price.Create(1.000350, 6),
            //     Price.Create(0.999850, 6),
            //     Price.Create(1.000025, 6),
            //     Quantity.Create(8),
            //     StubZonedDateTime.UnixEpoch() + Duration.FromSeconds(120));

            // Act
            // var result = this.repository.GetBars(barType, null, null, 1);

            // Assert
            // TODO: System.InvalidCastException : Specified cast is not valid. (inside StackExchange.Redis)
            // Assert.Equal(6, this.repository.TicksCount(audusd.Symbol));
            // Assert.True(this.repository.BarsExist(barType));
            // Assert.Equal(2, this.repository.BarsCount(barType));
            // Assert.Single(result.Bars);
            // Assert.Equal(expected, result.Bars[0]);
        }
    }
}
