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
            var position = new Position(
                new PositionId("P-123456"),
                new Symbol("SYMBOL", "GLOBEX"),
                StubZonedDateTime.UnixEpoch());

            var message = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(1000),
                Price.Create(2000, 2),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(message);

            // Assert
            Assert.Equal(new Symbol("SYMBOL", "GLOBEX"), position.Symbol);
            Assert.Equal(new OrderId("O-123456"), position.FromOrderId);
            Assert.Equal(new OrderId("O-123456"), position.LastOrderId);
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakQuantity);
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.EntryTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(Price.Create(2000, 2), position.AverageEntryPrice);
            Assert.Equal(new ExecutionId("E-123456"), position.LastExecutionId);
            Assert.Equal(new ExecutionTicket("ET-123456"), position.LastExecutionTicket);
            Assert.Equal(OrderSide.BUY, position.EntryDirection);
            Assert.Equal(message, position.LastEvent);
            Assert.True(position.IsLong);
            Assert.False(position.IsExited);
            Assert.False(position.IsShort);
            Assert.False(position.IsFlat);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromAlreadyShort_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new PositionId("P-123456"),
                new Symbol("SYMBOL", "GLOBEX"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(7000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(message1);
            position.Apply(message2);
            position.Apply(message3);

            // Assert
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(Quantity.Create(7000), position.Quantity);
            Assert.Equal(message3, position.LastEvent);
            Assert.True(position.IsShort);
            Assert.False(position.IsExited);
            Assert.False(position.IsLong);
            Assert.False(position.IsFlat);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromLongPositionToFlat_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new PositionId("P-123456"),
                new Symbol("SYMBOL", "GLOBEX"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(200000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                new OrderId("O-123457"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(50000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message4 = new OrderFilled(
                new OrderId("O-123458"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(250000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(message1);
            position.Apply(message2);
            position.Apply(message3);
            position.Apply(message4);

            // Assert
            Assert.Equal(MarketPosition.Flat, position.MarketPosition);
            Assert.Equal(Quantity.Zero(), position.Quantity);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.ExitTime);
            Assert.Equal(Price.Create(1.00000m, 5), position.AverageExitPrice);
            Assert.Equal(message4, position.LastEvent);
            Assert.False(position.IsShort);
            Assert.True(position.IsExited);
            Assert.False(position.IsLong);
            Assert.True(position.IsFlat);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromShortPositionToLong_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new PositionId("P-123456"),
                new Symbol("SYMBOL", "GLOBEX"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                new OrderId("O-123456"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.SELL,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                new OrderId("O-123457"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(500000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                new OrderId("O-123458"),
                AccountId.FromString("FXCM-02851908"),
                new ExecutionId("E-123456"),
                new ExecutionTicket("ET-123456"),
                position.Symbol,
                OrderSide.BUY,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(message1);
            position.Apply(message2);
            position.Apply(message3);

            // Assert
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(Quantity.Create(500000), position.Quantity);
        }
    }
}
