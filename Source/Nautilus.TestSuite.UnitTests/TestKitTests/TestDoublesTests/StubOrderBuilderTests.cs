﻿//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
            Assert.Equal("O-123456", order.Id.ToString());
            Assert.Equal("TEST_ORDER", order.Label.ToString());
            Assert.Equal(OrderSide.BUY, order.Side);
            Assert.Equal(OrderType.STOP_MARKET, order.Type);
            Assert.Equal(Quantity.Create(1), order.Quantity);
            Assert.Equal(Price.Create(1, 1), order.Price);
            Assert.Equal(TimeInForce.DAY, order.TimeInForce);
            Assert.Null(order.ExpireTime);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.Timestamp);
        }

        [Fact]
        internal void Build_WithAllParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder()
               .WithSymbol(new Symbol("AUDUSD", Venue.FXCM))
               .WithOrderId("O-678910")
               .WithLabel("TEST_ORDER2")
               .WithOrderSide(OrderSide.SELL)
               .WithQuantity(Quantity.Create(100000))
               .WithPrice(Price.Create(1.00000m, 5))
               .WithTimeInForce(TimeInForce.GTD)
               .WithExpireTime(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration())
               .WithTimestamp(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration())
               .BuildStopMarketOrder();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Venue.FXCM), order.Symbol);
            Assert.Equal("O-678910", order.Id.ToString());
            Assert.Equal("TEST_ORDER2", order.Label.ToString());
            Assert.Equal(OrderSide.SELL, order.Side);
            Assert.Equal(OrderType.STOP_MARKET, order.Type);
            Assert.Equal(Quantity.Create(100000), order.Quantity);
            Assert.Equal(Price.Create(1.00000m, 5), order.Price);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(), order.Timestamp);
        }

        [Fact]
        internal void BuildMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Assert
            Assert.Equal(OrderType.MARKET, order.Type);
        }

        [Fact]
        internal void BuildStopMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Equal(OrderType.STOP_MARKET, order.Type);
        }

        [Fact]
        internal void BuildStopLimit_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Assert
            Assert.Equal(OrderType.STOP_LIMIT, order.Type);
        }
    }
}
