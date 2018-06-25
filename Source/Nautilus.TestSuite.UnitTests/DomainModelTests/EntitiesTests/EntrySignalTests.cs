//--------------------------------------------------------------------------------------------------
// <copyright file="EntrySignalTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.EntitiesTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class EntrySignalTests
    {
        [Fact]
        internal void ExpireTime_BarsValidZero_ReturnsNullExpireTime()
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Second, 30),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                0,
                StubZonedDateTime.UnixEpoch());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("NONE"),
                new Label("NONE"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Theory]
        [InlineData(10, 1, 10)]
        [InlineData(30, 1, 30)]
        [InlineData(100, 2, 200)]
        internal void ExpireTime_SecondTimeFrameVariousParams_ReturnsExpectedExpireTime(
            int barsPeriod,
            int barsValid,
            int timeMultiple)
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Second, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubZonedDateTime.UnixEpoch());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("NONE"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromSeconds(timeMultiple).ToDuration(), result);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(5, 2, 10)]
        [InlineData(15, 3, 45)]
        internal void ExpireTime_MinuteTimeFrameVariousParams_ReturnsExpectedExpireTime(
            int barsPeriod,
            int barsValid,
            int timeMultiple)
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Minute, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubZonedDateTime.UnixEpoch());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(timeMultiple).ToDuration(), result);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 3, 3)]
        [InlineData(4, 2, 8)]
        internal void ExpireTime_HourTimeFrameVariousParams_ReturnsExpectedExpireTime(
            int barsPeriod,
            int barsValid,
            int timeMultiple)
        {
            // Arrange
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Hour, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubZonedDateTime.UnixEpoch());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromHours(timeMultiple).ToDuration(), result);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 3, 3)]
        internal void ExpireTime_DayTimeFrameVariousParams_ReturnsExpectedExpireTime(
            int barsPeriod,
            int barsValid,
            int timeMultiple)
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Day, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubZonedDateTime.UnixEpoch());

            // Arrange
            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromDays(timeMultiple).ToDuration(), result);
        }

        [Fact]
        internal void ExpireTime_WeekTimeFrame_ReturnsExpectedExpireTime()
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(QuoteType.Bid, Resolution.Day, 5),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                1,
                StubZonedDateTime.UnixEpoch());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromDays(5).ToDuration(), result);
        }
    }
}
