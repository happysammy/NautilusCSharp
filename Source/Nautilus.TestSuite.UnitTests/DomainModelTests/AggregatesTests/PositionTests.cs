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
        internal void ApplyEvent_OrderFilledBuyCase_ReturnsCorrectValues()
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
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakQuantity);
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.EntryTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(Price.Create(2000, 2), position.AverageEntryPrice);
            Assert.Equal(new ExecutionId("E-123456"), position.LastExecutionId);
            Assert.Equal(new PositionIdBroker("ET-123456"), position.IdBroker);
            Assert.Equal(OrderSide.BUY, position.EntryDirection);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.True(position.IsOpen);
            Assert.True(position.IsLong);
            Assert.False(position.IsClosed);
            Assert.False(position.IsShort);
            Assert.False(position.IsFlat);
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
            Assert.False(position.IsFlat);
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
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.ExitTime);
            Assert.Equal(Price.Create(1.00000m, 5), position.AverageExitPrice);
            Assert.Equal(orderFill4, position.LastEvent);
            Assert.False(position.IsShort);
            Assert.False(position.IsOpen);
            Assert.True(position.IsClosed);
            Assert.False(position.IsLong);
            Assert.True(position.IsFlat);
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
                Price.Create(1.00000m, 5),
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
