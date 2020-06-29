//--------------------------------------------------------------------------------------------------
// <copyright file="PositionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class PositionTests
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
                OrderSide.Buy,
                Quantity.Create(1000),
                Price.Create(2000, 2),
                Currency.USD,
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
            Assert.Null(position.OpenDuration);
            Assert.Equal(OrderSide.Buy, position.EntryDirection);
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakQuantity);
            Assert.Equal(MarketPosition.Long, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.OpenedTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(0m, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(Money.Zero(position.BaseCurrency), position.RealizedPnl);
            Assert.True(position.IsOpen);
            Assert.True(position.IsLong);
            Assert.False(position.IsClosed);
            Assert.False(position.IsShort);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.LastUpdated);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(orderFill, position.InitialEvent);
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
                OrderSide.Sell,
                Quantity.Create(1000),
                Price.Create(2000, 2),
                Currency.USD,
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
            Assert.Null(position.OpenDuration);
            Assert.Equal(OrderSide.Sell, position.EntryDirection);
            Assert.Equal(Quantity.Create(1000), position.Quantity);
            Assert.Equal(Quantity.Create(1000), position.PeakQuantity);
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.OpenedTime);
            Assert.Equal(1, position.EventCount);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(0m, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(Money.Zero(position.BaseCurrency), position.RealizedPnl);
            Assert.True(position.IsOpen);
            Assert.True(position.IsShort);
            Assert.False(position.IsClosed);
            Assert.False(position.IsLong);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), position.LastUpdated);
            Assert.Equal(orderFill, position.LastEvent);
            Assert.Equal(orderFill, position.InitialEvent);
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
                OrderSide.Sell,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                Currency.USD,
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
                OrderSide.Buy,
                Quantity.Create(5000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(1),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(orderFill2);

            // Assert
            Assert.Equal(MarketPosition.Flat, position.MarketPosition);
            Assert.Equal(Duration.FromMinutes(1), position.OpenDuration);
            Assert.Equal(Quantity.Create(0), position.Quantity);
            Assert.Equal(orderFill2, position.LastEvent);
            Assert.True(position.IsClosed);
            Assert.False(position.IsOpen);
            Assert.False(position.IsShort);
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
                OrderSide.Buy,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                Currency.USD,
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
                OrderSide.Buy,
                Quantity.Create(200000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123457"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.Sell,
                Quantity.Create(50000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill4 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123458"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.Sell,
                Quantity.Create(250000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            position.Apply(orderFill2);
            position.Apply(orderFill3);
            position.Apply(orderFill4);

            // Assert
            Assert.Equal(MarketPosition.Short, position.MarketPosition);
            Assert.Equal(Quantity.Create(100000), position.Quantity);
            Assert.Null(position.ClosedTime);
            Assert.Equal(1.00000m, position.AverageClosePrice);
            Assert.Equal(orderFill4, position.LastEvent);
            Assert.True(position.IsShort);
            Assert.True(position.IsOpen);
            Assert.False(position.IsClosed);
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
                OrderSide.Sell,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234561"),
                new ExecutionId("E-1234561"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.Sell,
                Quantity.Create(100000),
                Price.Create(1.00001m, 5),
                Currency.USD,
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
            Assert.Equal(decimal.Zero, position.RealizedPoints);
            Assert.Equal(0, position.RealizedReturn);
            Assert.Equal(Money.Zero(position.BaseCurrency), position.RealizedPnl);
            Assert.Equal(0.199945m, position.UnrealizedPoints(tick));
            Assert.Equal(0.19994400027999865, position.UnrealizedReturn(tick));
            Assert.Equal(Money.Create(39989m, position.BaseCurrency), position.UnrealizedPnl(tick));
            Assert.Equal(0.199945m, position.TotalPoints(tick));
            Assert.Equal(0.19994400027999865, position.TotalReturn(tick));
            Assert.Equal(Money.Create(39989m, position.BaseCurrency), position.TotalPnl(tick));
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
                OrderSide.Buy,
                Quantity.Create(100000),
                Price.Create(1.00000m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill2 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234561"),
                new ExecutionId("E-1234561"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.Sell,
                Quantity.Create(50000),
                Price.Create(1.00011m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-1234562"),
                new ExecutionId("E-1234562"),
                new PositionIdBroker("ET-123456"),
                new Symbol("AUDUSD", new Venue("FXCM")),
                OrderSide.Sell,
                Quantity.Create(50000),
                Price.Create(1.00010m, 5),
                Currency.USD,
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
            Assert.Equal(Quantity.Create(100000), position.PeakQuantity);
            Assert.Equal(0.000105m, position.RealizedPoints);
            Assert.Equal(0.00010500000000002174, position.RealizedReturn);
            Assert.Equal(Money.Create(10.50m, position.BaseCurrency), position.RealizedPnl);
            Assert.Equal(decimal.Zero, position.UnrealizedPoints(tick));
            Assert.Equal(0, position.UnrealizedReturn(tick));
            Assert.Equal(Money.Zero(position.BaseCurrency), position.UnrealizedPnl(tick));
            Assert.Equal(0.000105m, position.TotalPoints(tick));
            Assert.Equal(0.00010500000000002174, position.TotalReturn(tick));
            Assert.Equal(Money.Create(10.50m, position.BaseCurrency), position.TotalPnl(tick));
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
                OrderSide.Sell,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                Currency.USD,
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
                OrderSide.Buy,
                Quantity.Create(500000),
                Price.Create(1.00001m, 5),
                Currency.USD,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var orderFill3 = new OrderFilled(
                AccountId.FromString("FXCM-02851908-DEMO"),
                new OrderId("O-123458"),
                new ExecutionId("E-123456"),
                new PositionIdBroker("ET-123456"),
                position.Symbol,
                OrderSide.Buy,
                Quantity.Create(1000000),
                Price.Create(1.00000m, 5),
                Currency.USD,
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
