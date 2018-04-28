//--------------------------------------------------------------
// <copyright file="EntrySignalTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

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
                new BarSpecification(BarTimeFrame.Second, 30),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                0,
                StubDateTime.Now());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("NONE"),
                new Label("NONE"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

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
                new BarSpecification(BarTimeFrame.Second, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubDateTime.Now());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("NONE"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromSeconds(timeMultiple).ToDuration(), result);
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
                new BarSpecification(BarTimeFrame.Minute, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubDateTime.Now());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromMinutes(timeMultiple).ToDuration(), result);
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
                new BarSpecification(BarTimeFrame.Hour, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubDateTime.Now());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromHours(timeMultiple).ToDuration(), result);
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
                new BarSpecification(BarTimeFrame.Day, barsPeriod),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                barsValid,
                StubDateTime.Now());

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
                StubDateTime.Now());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromDays(timeMultiple).ToDuration(), result);
        }

        [Fact]
        internal void ExpireTime_WeekTimeFrame_ReturnsExpectedExpireTime()
        {
            // Arrange
            var tradeProfile = new TradeProfile(
                new TradeType("TestTrades"),
                new BarSpecification(BarTimeFrame.Week, 1),
                10,
                1,
                1000,
                0,
                0,
                0,
                0,
                1,
                StubDateTime.Now());

            var signal = new EntrySignal(
                new Symbol("SYMBOL", Exchange.GLOBEX),
                new EntityId("Test_Strategy"),
                new Label("Test Signal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(2, 1),
                Price.Create(1, 1),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = signal.ExpireTime;

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromDays(7).ToDuration(), result);
        }
    }
}
