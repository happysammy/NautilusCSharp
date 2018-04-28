//--------------------------------------------------------------
// <copyright file="RiskModelTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.RiskTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Risk;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class RiskModelTests
    {
        [Fact]
        internal void NewZonedDateTimeiation_ReturnsExpectedResults()
        {
            // Arrange

            // Act
            var riskModel = new RiskModel(
                new EntityId("12345"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Assert
            Assert.Equal("12345", riskModel.RiskModelId.ToString());
            Assert.Equal(10, riskModel.GlobalMaxRiskExposure.Value);
            Assert.Equal(1, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.True(riskModel.PositionSizeHardLimits);
            Assert.Equal(1, riskModel.EventCount);
            Assert.Equal(StubDateTime.Now(), riskModel.LastEventTime);
        }

        [Fact]
        internal void GetHardLimitQuantity_WhenNothingSet_ReturnsFalseAndQuantityZero()
        {
            // Arrange
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            var result = riskModel.GetHardLimitQuantity(new Symbol("SYMBOL", Exchange.GLOBEX));

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Fact]
        internal void GetMaxTrades_WhenNothingSet_ReturnsFalseAndZero()
        {
            // Arrange
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdateGlobalMaxRiskPerTrade(Percentage.Create(0.5m), StubDateTime.Now());

            // Assert
            Assert.Equal(0.5m, riskModel.GlobalMaxRiskPerTrade.Value);
            Assert.Equal(2, riskModel.EventCount);
        }

        [Fact]
        internal void UpdateMaxRiskPerTradeType_ValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(0.5m), StubDateTime.Now());

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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(0.5m), StubDateTime.Now());
            riskModel.UpdateMaxRiskPerTradeType(new TradeType("TestType"), Percentage.Create(1.5m), StubDateTime.Now());

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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(3), StubDateTime.Now());

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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(3), StubDateTime.Now());
            riskModel.UpdateMaxTradesPerSymbolType(new TradeType("TestType"), Quantity.Create(1), StubDateTime.Now());

            // Assert
            Assert.Equal(Quantity.Create(2), riskModel.GetMaxTrades(new TradeType("AnotherType")));
            Assert.Equal(Quantity.Create(1), riskModel.GetMaxTrades(new TradeType("TestType")));
            Assert.Equal(3, riskModel.EventCount);
        }

        [Fact]
        internal void UpdatePositionSizeHardLimit_ValidValues_ReturnsExpectedResultAndLogs()
        {
            // Arrange
            var symbol = new Symbol("SYMBOL", Exchange.GLOBEX);
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(1000000), StubDateTime.Now());
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
            var symbol = new Symbol("SYMBOL", Exchange.GLOBEX);
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(1000000), StubDateTime.Now());
            riskModel.UpdatePositionSizeHardLimit(symbol, Quantity.Create(2000000), StubDateTime.Now());
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
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            var eventLog = riskModel.GetEventLog();

            // Assert
            Assert.Equal(1, eventLog.Count);
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var riskModel = new RiskModel(
                new EntityId("NONE"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                StubDateTime.Now());

            // Act
            var result = LogFormatter.ToOutput(riskModel);
            var expected =
                $"RiskModel: GlobalMaxRiskExposure=10%, GlobalMaxRiskPerTrade=1%, PositionSizeHardLimits=True, EventCount=1, LastEventTime=1970-01-01T00:00:01.000Z";

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
