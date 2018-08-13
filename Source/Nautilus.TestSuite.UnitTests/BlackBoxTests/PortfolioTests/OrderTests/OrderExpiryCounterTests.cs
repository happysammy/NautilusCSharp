//--------------------------------------------------------------------------------------------------
// <copyright file="OrderExpiryCounterTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderExpiryCounterTests
    {
        [Fact]
        internal void GivenNewInstantiation_InitializesCorrectly()
        {
            // Arrange
            var tradeProfile = StubTradeProfileFactory.Create(10);
            var entryOrder = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var orderExpiryCounter = new OrderExpiryCounter(entryOrder, tradeProfile.BarsValid);

            // Assert
            Assert.Equal(entryOrder, orderExpiryCounter.Order);
            Assert.Equal(1, orderExpiryCounter.BarsValid);
            Assert.Equal(0, orderExpiryCounter.BarsCount);
        }

        [Fact]
        internal void IncrementCount_ThenIncrementsCount()
        {
            // Arrange
            var tradeProfile = StubTradeProfileFactory.Create(10);
            var entryOrder = new StubOrderBuilder().BuildStopMarketOrder();
            var orderExpiryCounter = new OrderExpiryCounter(entryOrder, tradeProfile.BarsValid);

            // Act
            orderExpiryCounter.IncrementCount();

            // Assert
            Assert.Equal(1, orderExpiryCounter.BarsCount);
        }

        [Fact]
        internal void IsOrderExpired_WhenNoIncrements_ThenReturnsFalse()
        {
            // Arrange
            var tradeProfile = StubTradeProfileFactory.Create(10);
            var entryOrder = new StubOrderBuilder().BuildStopMarketOrder();
            var orderExpiryCounter = new OrderExpiryCounter(entryOrder, tradeProfile.BarsValid);

            // Act
            var result = orderExpiryCounter.IsOrderExpired();

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsOrderExpired_WhenBarsCountEqualsBarsValid_ReturnsTrue()
        {
            // Arrange
            var tradeProfile = StubTradeProfileFactory.Create(10);
            var entryOrder = new StubOrderBuilder().BuildStopMarketOrder();
            var orderExpiryCounter = new OrderExpiryCounter(entryOrder, tradeProfile.BarsValid);

            // Act
            orderExpiryCounter.IncrementCount();

            // Assert
            Assert.Equal(1, orderExpiryCounter.BarsCount);
            Assert.True(orderExpiryCounter.IsOrderExpired());
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var tradeProfile = StubTradeProfileFactory.Create(10);
            var entryOrder = new StubOrderBuilder().BuildStopMarketOrder();
            var orderExpiryCounter = new OrderExpiryCounter(entryOrder, tradeProfile.BarsValid);

            // Act
            var result = orderExpiryCounter.ToString();

            // Assert
            Assert.Equal("ExpiryCounter_StubOrderId", result);
        }
    }
}
