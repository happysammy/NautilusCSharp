//--------------------------------------------------------------------------------------------------
// <copyright file="OrderTests.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Core.Types;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Factories;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using Nautilus.TestSuite.TestKit.Fixtures;
using Nautilus.TestSuite.TestKit.Stubs;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class OrderTests : TestBase
    {
        public OrderTests(ITestOutputHelper output)
            : base(output)
        {
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
                OrderSide.Buy,
                OrderPurpose.Entry,
                Quantity.Create(10),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid());

            // Assert
            Assert.Equal(new Symbol("SYMBOL", "LMAX"), order.Symbol);
            Assert.Equal("O-123456-S1", order.Id.Value);
            Assert.Equal("S1-E", order.Label.Value);
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.Market, order.OrderType);
            Assert.Equal(OrderPurpose.Entry, order.OrderPurpose);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Null(order.AveragePrice);
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
                OrderSide.Buy,
                OrderPurpose.Entry,
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
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.Stop, order.OrderType);
            Assert.Equal(OrderPurpose.Entry, order.OrderPurpose);
            Assert.Equal(10, order.Quantity.Value);
            Assert.Equal(Price.Create(2000, 1), order.Price);
            Assert.Null(order.AveragePrice);
            Assert.Null(order.Slippage);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderRejectedEvent(order);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderCancelledEvent(order);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderExpiredEvent(order);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderFilledEvent(order, order.Price);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderPartiallyFilledEvent(
                order,
                order.Quantity,
                Quantity.Create(order.Quantity / 2),
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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderPartiallyFilledEvent(
                order,
                Quantity.Create(50000),
                Quantity.Create(50000),
                order.Price);

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

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderFilledEvent(order, order.Price);

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
               .WithOrderSide(OrderSide.Buy)
               .WithPrice(Price.Create(0.80000m, 5))
               .BuildStopMarketOrder();

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderFilledEvent(order, Price.Create(averagePrice, 5));

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
               .WithOrderSide(OrderSide.Sell)
               .WithPrice(Price.Create(1.20000m, 5))
               .BuildStopMarketOrder();

            var event1 = StubEventMessageProvider.OrderSubmittedEvent(order);
            var event2 = StubEventMessageProvider.OrderAcceptedEvent(order);
            var event3 = StubEventMessageProvider.OrderWorkingEvent(order, order.Price);
            var event4 = StubEventMessageProvider.OrderFilledEvent(order, Price.Create(averagePrice, 5));

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

            this.Output.WriteLine(order1.ToString());
            this.Output.WriteLine(order2.ToString());

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
