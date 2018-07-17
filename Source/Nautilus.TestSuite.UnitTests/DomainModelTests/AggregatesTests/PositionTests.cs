//--------------------------------------------------------------------------------------------------
// <copyright file="PositionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PositionTests
    {
        [Fact]
        internal void ApplyEvent_OrderFilledBuyCase_ReturnsCorrectValues()
        {
            // Arrange
            var position = new Position(
                new Symbol("SYMBOL", Venue.GLOBEX),
                new EntityId("NONE"),
                new EntityId("NONE"),
                StubZonedDateTime.UnixEpoch());

            var message = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(1000),
                Price.Create(2000, 0.01m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(message);

            // Assert
            Assert.Equal(new Symbol("SYMBOL", Venue.GLOBEX), position.Symbol);
            Assert.Equal("NONE", position.FromEntryOrderId.ToString());
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.EntryTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(Price.Create(2000, 0.01m), position.AverageEntryPrice);
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromAlreadyShort_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new Symbol("SYMBOL", Venue.GLOBEX),
                new EntityId("NONE"),
                new EntityId("NONE"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Sell,
                Quantity.Create(5000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(5000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Sell,
                Quantity.Create(7000),
                Price.Create(1.00000m, 0.00001m),
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
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromLongPositionToFlat_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new Symbol("SYMBOL", Venue.GLOBEX),
                new EntityId("NONE"),
                new EntityId("NONE"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(100000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(200000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Sell,
                Quantity.Create(50000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message4 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Sell,
                Quantity.Create(250000),
                Price.Create(1.00000m, 0.00001m),
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
        }

        [Fact]
        internal void ApplyEvents_OrderFilledFromShortPositionToLong_ReturnsCorrectMarketPositionAndQuantity()
        {
            // Arrange
            var position = new Position(
                new Symbol("SYMBOL", Venue.GLOBEX),
                new EntityId("NONE"),
                new EntityId("NONE"),
                StubZonedDateTime.UnixEpoch());

            var message1 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Sell,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message2 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(500000),
                Price.Create(1.00000m, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var message3 = new OrderFilled(
                position.Symbol,
                position.FromEntryOrderId,
                new EntityId("NONE"),
                new EntityId("NONE"),
                OrderSide.Buy,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 0.00001m),
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