//--------------------------------------------------------------------------------------------------
// <copyright file="OrderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderTests
    {
        private readonly ITestOutputHelper output;

        public OrderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        internal void CreateMarketOrder_WithValidArguments_ReturnsExpectedObject()
        {
            // Arrange
            // Act
            var order = new MarketOrder(
                new Symbol("SYMBOL", Venue.LMAX),
                new EntityId("some_orderId"),
                new Label("some_label"),
                OrderSide.Buy,
                Quantity.Create(10),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(new Symbol("SYMBOL", Venue.LMAX), order.Symbol);
            Assert.Equal("some_orderId", order.OrderId.ToString());
            Assert.Equal("some_label", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.Market, order.OrderType);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Equal(decimal.Zero, order.AveragePrice.Value);
            Assert.Equal(new List<EntityId> { new EntityId("some_orderId") }, order.GetOrderIdList());
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
            Assert.Equal(OrderStatus.Initialized, order.OrderStatus);
        }

        [Fact]
        internal void CreateStopMarketOrder_WithValidParameters_ReturnsExpectedObject()
        {
            // Arrange
            // Act
            var order = new StopMarketOrder(
                new Symbol("SYMBOL", Venue.LMAX),
                new EntityId("some_orderId"),
                new Label("some_label"),
                OrderSide.Buy,
                Quantity.Create(10),
                Price.Create(2000, 0.1m),
                TimeInForce.GTD,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(),
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(new Symbol("SYMBOL", Venue.LMAX), order.Symbol);
            Assert.Equal("some_orderId", order.OrderId.ToString());
            Assert.Equal("some_label", order.OrderLabel.ToString());
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.StopMarket, order.OrderType);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Equal(2000, order.Price.Value);
            Assert.Equal(decimal.Zero, order.AveragePrice.Value);
            Assert.Equal(decimal.Zero, order.Slippage);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(new List<EntityId> { new EntityId("some_orderId") }, order.GetOrderIdList());
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
            Assert.Equal(OrderStatus.Initialized, order.OrderStatus);
        }

        [Fact]
        internal void BrokerOrderId_ListIsEmpty_ReturnsNullEntityId()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarket();

            // Assert
            Assert.Equal("None", order.BrokerOrderId.Value);
        }

        [Fact]
        internal void ExecutionId_ListIsEmpty_ReturnsNullEntityId()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarket();

            // Assert
            Assert.Equal("None", order.ExecutionId.Value);
        }

        [Fact]
        internal void Rejected_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message = StubEventMessages.OrderRejectedEvent(order);

            // Act
            order.Apply(message);

            // Assert
            Assert.Equal(1, order.EventCount);
            Assert.Equal(OrderStatus.Rejected, order.OrderStatus);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
        }

        [Fact]
        internal void Cancelled_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderCancelledEvent(order);

            // Act
            order.Apply(message1);
            order.Apply(message2);

            // Assert
            Assert.Equal(2, order.EventCount);
            Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
        }

        [Fact]
        internal void Expired_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderExpiredEvent(order);

            // Act
            order.Apply(message1);
            order.Apply(message2);

            // Assert
            Assert.Equal(2, order.EventCount);
            Assert.Equal(OrderStatus.Expired, order.OrderStatus);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
        }

        [Fact]
        internal void Working_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message = StubEventMessages.OrderWorkingEvent(order);

            // Act
            order.Apply(message);

            // Assert
            Assert.Equal("some_broker_orderId", order.BrokerOrderId.ToString());
            Assert.Equal(1, order.EventCount);
            Assert.Equal(OrderStatus.Working, order.OrderStatus);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEventTime);
        }

        [Fact]
        internal void Apply_OrderFilled_ReturnsCorrectOrderStatus()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderFilledEvent(order);

            // Act
            order.Apply(message1);
            order.Apply(message2);
            var result = order.OrderStatus;

            // Assert
            Assert.Equal(OrderStatus.Filled, result);
        }

        [Fact]
        internal void Apply_OrderPartiallyFilled_ReturnsCorrectOrderStatus()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = StubEventMessages.OrderPartiallyFilledEvent(order, order.Quantity / 2, order.Quantity / 2);

            // Act
            order.Apply(message1);
            order.Apply(message2);
            var result = order.OrderStatus;

            // Assert
            Assert.Equal(OrderStatus.PartiallyFilled, result);
        }

        [Fact]
        internal void IsComplete_OrderInitialized_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();

            // Act
            var result = order.IsComplete;

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderWorking_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();

            var message = new OrderWorking(
                new Symbol("AUDUSD", Venue.LMAX),
                order.OrderId,
                new EntityId("some_broker_orderId"),
                order.OrderLabel,
                order.OrderSide,
                order.OrderType,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            order.Apply(message);
            var result = order.IsComplete;

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderPartiallyFilled_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder()
               .WithOrderQuantity(Quantity.Create(100000))
               .BuildStopMarket();

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = new OrderPartiallyFilled(
                new Symbol("AUDUSD", Venue.LMAX),
                order.OrderId,
                new EntityId("some_execution_id"),
                new EntityId("some_execution_ticket"),
                order.OrderSide,
                Quantity.Create(order.Quantity.Value / 2),
                Quantity.Create(order.Quantity.Value / 2),
                order.Price,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(message1);
            order.Apply(message2);

            // Act
            var result = order.IsComplete;

            // Assert
            Assert.True(order.OrderStatus == OrderStatus.PartiallyFilled);
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderFilled_ReturnsTrue()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = new OrderFilled(
                order.Symbol,
                order.OrderId,
                new EntityId("some_execution_id"),
                new EntityId("some_execution_ticket"),
                order.OrderSide,
                order.Quantity,
                order.Price,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            order.Apply(message1);
            order.Apply(message2);

            // Assert
            Assert.Equal(OrderStatus.Filled, order.OrderStatus);
            Assert.True(order.IsComplete);
        }

        [Fact]
        internal void AddOrderIdModification_ReturnsExpectedModificationId()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();
            var modifiedOrderId = EntityIdFactory.ModifiedOrderId(order.OrderId, order.OrderIdCount);

            // Act
            order.AddModifiedOrderId(modifiedOrderId);

            // Assert
            Assert.Equal(2, order.OrderIdCount);
            Assert.Equal(new EntityId("StubOrderId_R1"), order.CurrentOrderId);
        }

        [Fact]
        internal void GetSlippage_UnfilledOrder_ReturnsZero()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();

            // Act
            var result = order.Slippage;

            // Assert
            Assert.Equal(decimal.Zero, result);
        }

        [Theory]
        [InlineData(0.80000, 0)]
        [InlineData(0.80001, 0.00001)]
        [InlineData(0.79998, -0.00002)]
        internal void GetSlippage_BuyOrderFilledVariousAveragePrices_ReturnsExpectedResult(decimal averagePrice, decimal expectedSlippage)
        {
            // Arrange
            var order = new StubOrderBuilder()
               .WithOrderSide(OrderSide.Buy)
               .WithOrderPrice(Price.Create(0.80000m, 0.00001m))
               .BuildStopMarket();

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = new OrderFilled(
                order.Symbol,
                order.OrderId,
                new EntityId("some_execution_id"),
                new EntityId("some_execution_ticket"),
                order.OrderSide,
                order.Quantity,
                Price.Create(averagePrice, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(message1);
            order.Apply(message2);

            // Act
            var result = order.Slippage;

            // Assert
            Assert.Equal(OrderStatus.Filled, order.OrderStatus);
            Assert.Equal(expectedSlippage, result);
        }

        [Theory]
        [InlineData(1.20000, 0)]
        [InlineData(1.19998, 0.00002)]
        [InlineData(1.20001, -0.00001)]
        internal void GetSlippage_SellOrderFilledVariousAveragePrices_ReturnsExpectedResult(decimal averagePrice, decimal expectedSlippage)
        {
            // Arrange
            var order = new StubOrderBuilder()
               .WithOrderSide(OrderSide.Sell)
               .WithOrderPrice(Price.Create(1.20000m, 0.00001m))
               .BuildStopMarket();

            var message1 = StubEventMessages.OrderWorkingEvent(order);
            var message2 = new OrderFilled(
                order.Symbol,
                order.OrderId,
                new EntityId("some_execution_id"),
                new EntityId("some_execution_ticket"),
                order.OrderSide,
                order.Quantity,
                Price.Create(averagePrice, 0.00001m),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(message1);
            order.Apply(message2);

            // Act
            var result = order.Slippage;

            // Assert
            Assert.Equal(OrderStatus.Filled, order.OrderStatus);
            Assert.Equal(expectedSlippage, result);
        }

        [Fact]
        internal void Equals_OrderWithTheSameOrderId_ReturnsFalse()
        {
            // Arrange
            var order1 = new StubOrderBuilder().WithOrderId("1234567").BuildStopMarket();
            var order2 = new StubOrderBuilder().WithOrderId("123456789").BuildStopMarket();

            // Act
            var result = order1.Equals(order2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_OrderWithTheSameOrderId_ReturnsTrue()
        {
            // Arrange
            var order1 = new StubOrderBuilder().WithOrderId("123456789").BuildStopMarket();
            var order2 = new StubOrderBuilder().WithOrderId("123456789").BuildStopMarket();

            // Act
            var result = order1.Equals(order2);

            this.output.WriteLine(order1.ToString());
            this.output.WriteLine(order2.ToString());

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void Equals_NullObject_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();

            // Act
            var result = order.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_ObjectSomeOtherType_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarket();

            // Act - ignore warning, this is why the test returns false!
            var result = order.Equals(string.Empty);

            // Assert
            Assert.False(result);
        }
    }
}
