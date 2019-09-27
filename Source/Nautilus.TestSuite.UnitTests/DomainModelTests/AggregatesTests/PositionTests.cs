//--------------------------------------------------------------------------------------------------
// <copyright file="PositionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PositionTests
    {
        [Fact]
        internal void Position_OrderFilledBuyCase_ReturnsCorrectValues()
        {
            // Arrange
            var orderFill = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.BUY,
                Quantity.Create(1000),
                Price.Create(2000, 2),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var position = new Position(new PositionId("P-123456"), orderFill);

            // Assert
            Assert.Equal(new Symbol("AUDUSD", new Venue("FXCM")), position.Symbol);
            Assert.Equal(new OrderId("O-123456"), position.FromOrderId);
            Assert.Equal(new OrderId("O-123456"), position.LastOrderId);
            Assert.Equal(new ExecutionId("E-123456"), position.LastExecutionId);
            Assert.Equal(new PositionIdBroker("ET-123456"), position.IdBroker);
            Assert.Equal(new OrderId("O-123456"), position.GetOrderIds()[0]);
            Assert.Equal(new ExecutionId("E-123456"), position.GetExecutionIds()[0]);
            Assert.Equal(2000, position.AverageOpenPrice);
            Assert.Null(position.AverageClosePrice);
            Assert.Null(position.ClosedTime);
            Assert.Equal(OrderSide.BUY, position.EntryDirection);
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakOpenQuantity);
            Assert.Equal(Quantity.Create(1000), position.FilledQuantityBuys);
            Assert.Equal(Quantity.Zero(), position.FilledQuantitySells);
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.OpenedTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(0m, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(0m, position.RealizedPnl);
            Assert.True(position.IsOpen);
            Assert.True(position.IsLong);
            Assert.False(position.IsClosed);
            Assert.False(position.IsShort);
        }

        [Fact]
        internal void Position_OrderFilledSellCase_ReturnsCorrectValues()
        {
            // Arrange
            var orderFill = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(1000),
                Price.Create(2000, 2),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var position = new Position(new PositionId("P-123456"), orderFill);

            // Assert
            Assert.Equal(new Symbol("AUDUSD", new Venue("FXCM")), position.Symbol);
            Assert.Equal(new OrderId("O-123456"), position.FromOrderId);
            Assert.Equal(new OrderId("O-123456"), position.LastOrderId);
            Assert.Equal(new ExecutionId("E-123456"), position.LastExecutionId);
            Assert.Equal(new PositionIdBroker("ET-123456"), position.IdBroker);
            Assert.Equal(new OrderId("O-123456"), position.GetOrderIds()[0]);
            Assert.Equal(new ExecutionId("E-123456"), position.GetExecutionIds()[0]);
            Assert.Equal(2000, position.AverageOpenPrice);
            Assert.Null(position.AverageClosePrice);
            Assert.Null(position.ClosedTime);
            Assert.Equal(OrderSide.SELL, position.EntryDirection);
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakOpenQuantity);
            Assert.Equal(Quantity.Create(1000), position.FilledQuantitySells);
            Assert.Equal(Quantity.Zero(), position.FilledQuantityBuys);
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.OpenedTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(0m, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(0m, position.RealizedPnl);
            Assert.True(position.IsOpen);
            Assert.True(position.IsShort);
            Assert.False(position.IsClosed);
            Assert.False(position.IsLong);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromAlreadyShort_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var orderFill11 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var position = new Position(new PositionId("P-123456"), orderFill11);

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(7000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(orderFill2);
            position.Apply(orderFill3);

            // Assert
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(Quantity.Create(7000), position.Quantity);
            Assert.Equal(orderFill3, position.LastEvent);
            Assert.True(position.IsOpen);
            Assert.True(position.IsShort);
            Assert.False(position.IsClosed);
            Assert.False(position.IsLong);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromLongPositionToFlat_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var orderFill1 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.BUY,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var position = new Position(new PositionId("P-123456"), orderFill1);

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(200000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123457"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(50000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill4 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123458"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(250000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(orderFill2);
            position.Apply(orderFill3);
            position.Apply(orderFill4);

            // Assert
            Assert.Equal(MarketPosition.Flat, position.MarketPosition);
            Assert.Equal(Quantity.Zero(), position.Quantity);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.ClosedTime);
            Assert.Equal(1.6666666666666666666666666667m, position.AverageClosePrice);
            Assert.Equal(orderFill4, position.LastEvent);
            Assert.False(position.IsShort);
            Assert.False(position.IsOpen);
            Assert.True(position.IsClosed);
            Assert.False(position.IsLong);
        }

        [Fact]
        internal void ApplyEvents_MultipleFillsInOpenDirection_ReturnsCorrectValues()
        {
            // Arrange
            var orderFill1 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234561"),
                new ExecutionId("E-1234561"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(100000),
                Price.Create(1.00001m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var position = new Position(new PositionId("P-123456"), orderFill1);

            var tick = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")));

            // Act
            position.Apply(orderFill2);

            // Assert
            Assert.Equal(1.000005m, position.AverageOpenPrice);
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(Quantity.Create(200000), position.Quantity);
            Assert.Equal(Quantity.Create(200000), position.FilledQuantitySells);
            Assert.Equal(decimal.Zero, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(decimal.Zero, position.RealizedPnl);
            Assert.Equal(0.199945m, position.UnrealizedPoints(tick));
            Assert.Equal(0.19994400027999865, position.UnrealizedReturn(tick));
            Assert.Equal(39989.00m, position.UnrealizedPnl(tick));
            Assert.Equal(0.199945m, position.TotalPoints(tick));
            Assert.Equal(0.19994400027999865, position.TotalReturn(tick));
            Assert.Equal(39989.00m, position.TotalPnl(tick));
        }

        [Fact]
        internal void ApplyEvents_MultipleFillsInCloseDirection_ReturnsCorrectValues()
        {
            // Arrange
            var orderFill1 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.BUY,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234561"),
                new ExecutionId("E-1234561"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(50000),
                Price.Create(1.00011m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234562"),
                new ExecutionId("E-1234562"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(50000),
                Price.Create(1.00010m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var position = new Position(new PositionId("P-123456"), orderFill1);

            var tick = StubTickProvider.Create(new Symbol("AUDUSD", new Venue("FXCM")));

            // Act
            position.Apply(orderFill2);
            position.Apply(orderFill3);

            // Assert
            Assert.Equal(1.00000m, position.AverageOpenPrice);
            Assert.Equal(1.000105m, position.AverageClosePrice);
            Assert.Equal(MarketPosition.Flat, position.MarketPosition);
            Assert.Equal(Quantity.Create(0), position.Quantity);
            Assert.Equal(Quantity.Create(100000), position.PeakOpenQuantity);
            Assert.Equal(Quantity.Create(100000), position.FilledQuantityBuys);
            Assert.Equal(Quantity.Create(100000), position.FilledQuantitySells);
            Assert.Equal(0.00010m, position.RealizedPoints);
            Assert.Equal(9.999999999998899E-05, position.RealizedReturn);
            Assert.Equal(5.00m, position.RealizedPnl);
            Assert.Equal(decimal.Zero, position.UnrealizedPoints(tick));
            Assert.Equal(0, position.UnrealizedReturn(tick));
            Assert.Equal(decimal.Zero, position.UnrealizedPnl(tick));
            Assert.Equal(0.00010m, position.TotalPoints(tick));
            Assert.Equal(9.999999999998899E-05, position.TotalReturn(tick));
            Assert.Equal(5.00m, position.TotalPnl(tick));
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromShortPositionToLong_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var orderFill1 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123456"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.SELL,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var position = new Position(new PositionId("P-123456"), orderFill1);

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123457"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(500000),
                Price.Create(1.00001m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123458"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(orderFill2);
            position.Apply(orderFill3);

            // Assert
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(Quantity.Create(500000), position.Quantity);
        }
    }
}
