//--------------------------------------------------------------------------------------------------
// <copyright file="OrderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
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
            var order = OrderFactory.Market(
                new OrderId("O-123456-S1"),
                new Symbol("SYMBOL", "LMAX"),
                new Label("S1-E"),
                OrderSide.BUY,
                Quantity.Create(10),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid());

            // Assert
            Assert.Equal(new Symbol("SYMBOL", "LMAX"), order.Symbol);
            Assert.Equal("O-123456-S1", order.Id.Value);
            Assert.Equal("S1-E", order.Label.Value);
            Assert.Equal(OrderSide.BUY, order.Side);
            Assert.Equal(OrderType.MARKET, order.Type);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Null(order.AveragePrice);
            Assert.Equal(new List<OrderId> { new OrderId("O-123456-S1") }, order.GetOrderIds());
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
            Assert.Equal(OrderState.Initialized, order.State);
        }

        [Fact]
        internal void CreateStopMarketOrder_WithValidParameters_ReturnsExpectedObject()
        {
            // Arrange
            // Act
            var order = OrderFactory.StopMarket(
                new OrderId("O-123456"),
                new Symbol("SYMBOL", "LMAX"),
                new Label("S1_SL"),
                OrderSide.BUY,
                Quantity.Create(10),
                Price.Create(2000, 1),
                TimeInForce.GTD,
                StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid());

            // Assert
            Assert.Equal(new Symbol("SYMBOL", "LMAX"), order.Symbol);
            Assert.Equal("O-123456", order.Id.Value);
            Assert.Equal("S1_SL", order.Label.Value);
            Assert.Equal(OrderSide.BUY, order.Side);
            Assert.Equal(OrderType.STOP_MARKET, order.Type);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Equal(Price.Create(2000, 1), order.Price);
            Assert.Null(order.AveragePrice);
            Assert.Null(order.Slippage);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(new List<OrderId> { new OrderId("O-123456") }.ToImmutableList(), order.GetOrderIds());
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
            Assert.Equal(OrderState.Initialized, order.State);
        }

        [Fact]
        internal void BrokerOrderId_ListIsEmpty_ReturnsNullEntityId()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Null(order.IdBroker);
        }

        [Fact]
        internal void ExecutionId_ListIsEmpty_ReturnsNullEntityId()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Null(order.ExecutionId);
        }

        [Fact]
        internal void Rejected_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderRejectedEvent(order);

            // Act
            order.Apply(event1);
            order.Apply(event2);

            // Assert
            Assert.Equal(3, order.EventCount);
            Assert.Equal(OrderState.Rejected, order.State);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
        }

        [Fact]
        internal void Cancelled_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessages.OrderCancelledEvent(order);

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Assert
            Assert.Equal(5, order.EventCount);
            Assert.Equal(OrderState.Cancelled, order.State);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
        }

        [Fact]
        internal void Expired_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessages.OrderExpiredEvent(order);

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Assert
            Assert.Equal(5, order.EventCount);
            Assert.Equal(OrderState.Expired, order.State);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
        }

        [Fact]
        internal void Working_ParametersValid_ReturnsExpectedResult()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);

            // Assert
            Assert.Equal("BO-123456", order.IdBroker?.Value);
            Assert.Equal(4, order.EventCount);
            Assert.Equal(OrderState.Working, order.State);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.LastEvent.Timestamp);
        }

        [Fact]
        internal void Apply_OrderFilled_ReturnsCorrectOrderStatus()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessages.OrderFilledEvent(order, order.Price);

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Assert
            Assert.Equal(OrderState.Filled, order.State);
        }

        [Fact]
        internal void Apply_OrderPartiallyFilled_ReturnsCorrectOrderStatus()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessages.OrderPartiallyFilledEvent(
                order,
                order.Quantity / 2,
                order.Quantity / 2,
                order.Price);

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            var result = order.State;

            // Assert
            Assert.Equal(OrderState.PartiallyFilled, result);
        }

        [Fact]
        internal void IsComplete_OrderInitialized_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var result = order.IsCompleted;

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderWorking_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = new OrderWorking(
                order.Id,
                new OrderIdBroker("B" + order.Id),
                AccountId.FromString("FXCM-02851908-DEMO"),
                new Symbol("AUDUSD", "FXCM"),
                order.Label,
                order.Side,
                order.Type,
                order.Quantity,
                order.Price,
                order.TimeInForce,
                order.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);

            var result = order.IsCompleted;

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderPartiallyFilled_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder()
               .WithQuantity(Quantity.Create(100000))
               .BuildStopMarketOrder();

            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = new OrderPartiallyFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("some_execution_id"),
                new ExecutionTicket("some_execution_ticket"),
                order.Symbol,
                order.Side,
                Quantity.Create(order.Quantity.Value / 2),
                Quantity.Create(order.Quantity.Value / 2),
                order.Price,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Act
            var result = order.IsCompleted;

            // Assert
            Assert.True(order.State == OrderState.PartiallyFilled);
            Assert.False(result);
        }

        [Fact]
        internal void IsComplete_OrderFilled_ReturnsTrue()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            if (order.Price is null)
            {
                throw new InvalidOperationException("Order must have a price.");
            }

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = new OrderFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("some_execution_id"),
                new ExecutionTicket("some_execution_ticket"),
                order.Symbol,
                order.Side,
                order.Quantity,
                order.Price,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Assert
            Assert.Equal(OrderState.Filled, order.State);
            Assert.True(order.IsCompleted);
        }

        [Fact]
        internal void AddOrderIdModification_ReturnsExpectedModificationId()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();
            var modifiedOrderId = OrderFactory.ModifiedOrderId(order.Id, order.IdCount);

            // Act
            order.AddModifiedOrderId(modifiedOrderId);

            // Assert
            Assert.Equal(2, order.IdCount);
            Assert.Equal(new OrderId("O-123456_R1"), order.IdLast);
        }

        [Fact]
        internal void GetSlippage_UnfilledOrder_ReturnsZero()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            var result = order.Slippage.HasValue;

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(0.80000, 0)]
        [InlineData(0.80001, 0.00001)]
        [InlineData(0.79998, -0.00002)]
        internal void GetSlippage_BuyOrderFilledVariousAveragePrices_ReturnsExpectedResult(decimal averagePrice, decimal expectedSlippage)
        {
            // Arrange
            var order = new StubOrderBuilder()
               .WithOrderSide(OrderSide.BUY)
               .WithPrice(Price.Create(0.80000m, 5))
               .BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = new OrderFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("some_execution_id"),
                new ExecutionTicket("some_execution_ticket"),
                order.Symbol,
                order.Side,
                order.Quantity,
                Price.Create(averagePrice, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Act
            var result = order.Slippage;

            // Assert
            Assert.Equal(OrderState.Filled, order.State);
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
               .WithOrderSide(OrderSide.SELL)
               .WithPrice(Price.Create(1.20000m, 5))
               .BuildStopMarketOrder();

            var event1 = StubEventMessages.OrderSubmittedEvent(order);
            var event2 = StubEventMessages.OrderAcceptedEvent(order);
            var event3 = StubEventMessages.OrderWorkingEvent(order, order.Price);
            var event4 = new OrderFilled(
                order.Id,
                AccountId.FromString("FXCM-02851908-DEMO"),
                new ExecutionId("some_execution_id"),
                new ExecutionTicket("some_execution_ticket"),
                order.Symbol,
                order.Side,
                order.Quantity,
                Price.Create(averagePrice, 5),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            order.Apply(event1);
            order.Apply(event2);
            order.Apply(event3);
            order.Apply(event4);

            // Act
            var result = order.Slippage;

            // Assert
            Assert.Equal(OrderState.Filled, order.State);
            Assert.Equal(expectedSlippage, result);
        }

        [Fact]
        internal void Equals_OrderWithTheSameOrderId_ReturnsFalse()
        {
            // Arrange
            var order1 = new StubOrderBuilder().WithOrderId("O-1234567").BuildStopMarketOrder();
            var order2 = new StubOrderBuilder().WithOrderId("O-123456789").BuildStopMarketOrder();

            // Act
            var result = order1.Equals(order2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void Equals_OrderWithTheSameOrderId_ReturnsTrue()
        {
            // Arrange
            var order1 = new StubOrderBuilder().WithOrderId("O-123456789").BuildStopMarketOrder();
            var order2 = new StubOrderBuilder().WithOrderId("O-123456789").BuildStopMarketOrder();

            // Act
            var result = order1.Equals(order2);

            this.output.WriteLine(order1.ToString());
            this.output.WriteLine(order2.ToString());

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void Equals_ObjectSomeOtherType_ReturnsFalse()
        {
            // Arrange
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global (this is why the test returns false)
            var result = order.Equals(string.Empty);

            // Assert
            Assert.False(result);
        }
    }
}
