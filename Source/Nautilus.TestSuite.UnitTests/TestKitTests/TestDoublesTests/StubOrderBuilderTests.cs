//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.TestKitTests.TestDoublesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubOrderBuilderTests
    {
        [Fact]
        internal void Build_WithNoParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Venue.FXCM), order.Symbol);
            Assert.Equal("StubOrderId", order.OrderId.ToString());
            Assert.Equal("StubOrderLabel", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.BUY, order.OrderSide);
            Assert.Equal(OrderType.StopMarket, order.OrderType);
            Assert.Equal(Quantity.Create(1), order.Quantity);
            Assert.Equal(Price.Create(1, 1), order.Price);
            Assert.Equal(TimeInForce.DAY, order.TimeInForce);
            Assert.True(order.ExpireTime.HasNoValue);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.OrderTimestamp);
        }

        [Fact]
        internal void Build_WithAllParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder()
               .WithSymbol(new Symbol("AUDUSD", Venue.FXCM))
               .WithOrderId("TestOrderId")
               .WithLabel("TestOrderLabel")
               .WithOrderSide(OrderSide.SELL)
               .WithQuantity(Quantity.Create(100000))
               .WithPrice(Price.Create(1.00000m, 0.00001m))
               .WithTimeInForce(TimeInForce.GTD)
               .WithExpireTime(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration())
               .WithTimestamp(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration())
               .BuildStopMarketOrder();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Venue.FXCM), order.Symbol);
            Assert.Equal("TestOrderId", order.OrderId.ToString());
            Assert.Equal("TestOrderLabel", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.SELL, order.OrderSide);
            Assert.Equal(OrderType.StopMarket, order.OrderType);
            Assert.Equal(Quantity.Create(100000), order.Quantity);
            Assert.Equal(Price.Create(1.00000m, 0.00001m), order.Price);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(), order.OrderTimestamp);
        }

        [Fact]
        internal void BuildMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Assert
            Assert.Equal(OrderType.Market, order.OrderType);
        }

        [Fact]
        internal void BuildStopMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Equal(OrderType.StopMarket, order.OrderType);
        }

        [Fact]
        internal void BuildStopLimit_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }
    }
}
