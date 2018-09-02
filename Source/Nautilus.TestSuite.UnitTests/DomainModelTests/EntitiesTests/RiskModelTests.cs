//--------------------------------------------------------------------------------------------------
// <copyright file="RiskModelTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.EntitiesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RiskModelTests
    {
        [Fact]
        internal void NewZonedDateTime_ReturnsExpectedResults()
        {
            // Arrange

            // Act
            var riskModel = new RiskModel(
                new RiskModelId("12345"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("12345", riskModel.Id.ToString());
            Assert.Equal(10, riskModel.GlobalMaxRiskExposure.Value);
            Assert.Equal(1, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.True(riskModel.PositionSizeHardLimits);
            Assert.Equal(1, riskModel.EventCount);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), riskModel.LastEventTime);
        }

        [Fact]
        internal void GetHardLimitQuantity_WhenNothingSet_ReturnsFalseAndQuantityZero()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = riskModel.GetHardLimitQuantity(new Symbol("SYMBOL", Venue.GLOBEX));

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Fact]
        internal void GetMaxTrades_WhenNothingSet_ReturnsFalseAndZero()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = riskModel.GetMaxTrades(new TradeType("some_trade_type"));

            // Assert
            Assert.Equal(Quantity.Create(2), result);
        }

        [Fact]
        internal void GetRiskPerTrade_WhenNothingSet_ReturnsMaxGlobalRiskPerTrade()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = riskModel.GetRiskPerTrade(new TradeType("some_trade_type"));

            // Assert
            Assert.Equal(1, result.Value);
        }

        [Fact]
        internal void UpdateGlobalMaxRiskPerTrade_ValidValue_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdateGlobalMaxRiskPerTrade(Percentage.Create(0.5m), StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(0.5m, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.Equal(2, riskModel.EventCount);
        }

        [Fact]
        internal void UpdateMaxRiskPerTradeType_ValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(0.5m), StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(0.5m, riskModel.GetRiskPerTrade(new TradeType("TestType")).Value);
            Assert.Equal(1, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.Equal(2, riskModel.EventCount);
        }

        [Fact]
        internal void UpdateMaxRiskPerTradeType_WhenTypeAlreadyExistsWithValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(0.5m), StubZonedDateTime.UnixEpoch());
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(1.5m), StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(1.5m, riskModel.GetRiskPerTrade(new TradeType("TestType")).Value);
            Assert.Equal(1, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.Equal(3, riskModel.EventCount);
        }

        [Fact]
        internal void UpdateMaxTradesPerSymbolType_ValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(3), StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(Quantity.Create(2), riskModel.GetMaxTrades(new TradeType("AnotherType")));
            Assert.Equal(Quantity.Create(3), riskModel.GetMaxTrades(new TradeType("TestType")));
            Assert.Equal(2, riskModel.EventCount);
        }

        [Fact]
        internal void UpdateMaxTradesPerSymbolType_WhenSymbolAlreadyExistsWithValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(3), StubZonedDateTime.UnixEpoch());
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(1), StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(Quantity.Create(2), riskModel.GetMaxTrades(new TradeType("AnotherType")));
            Assert.Equal(Quantity.Create(1), riskModel.GetMaxTrades(new TradeType("TestType")));
            Assert.Equal(3, riskModel.EventCount);
        }

        [Fact]
        internal void UpdatePositionSizeHardLimit_ValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var symbol = new Symbol("SYMBOL", Venue.GLOBEX);
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(1000000), StubZonedDateTime.UnixEpoch());
            var result = riskModel.GetHardLimitQuantity(symbol);

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(Quantity.Create(1000000), result.Value);
            Assert.Equal(2, riskModel.EventCount);
        }

        [Fact]
        internal void UpdatePositionSizeHardLimit_WhenHardLimitInPlaceWithValidValues_UpdatesAndLogs()
        {
            // Arrange
            var symbol = new Symbol("SYMBOL", Venue.GLOBEX);
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(1000000), StubZonedDateTime.UnixEpoch());
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(2000000), StubZonedDateTime.UnixEpoch());
            var result = riskModel.GetHardLimitQuantity(symbol);

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(Quantity.Create(2000000), result.Value);
            Assert.Equal(3, riskModel.EventCount);
        }

        [Fact]
        internal void GetEventLog_ReturnsTheEventLog()
        {
            // Arrange
            var riskModel = new RiskModel(
                new RiskModelId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubZonedDateTime.UnixEpoch());

            // Act
            var eventLog = riskModel.GetEventLog();

            // Assert
            Assert.Equal(1, eventLog.Count);
        }
    }
}
