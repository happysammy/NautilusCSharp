//--------------------------------------------------------------------------------------------------
// <copyright file="TradeTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.AggregatesTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TradeTests
    {
        [Fact]
        internal void CanInitializeTrade_ReturnsTradeStatusInitialized()
        {
            // Arrange
            var atomicOrders = new List<AtomicOrder>
                                {
                                    StubAtomicOrderBuilder.Build(),
                                    StubAtomicOrderBuilder.Build(),
                                    StubAtomicOrderBuilder.Build(),
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            // Act
            var trade = TradeFactory.Create(orderPacket);

            // Assert
            Assert.Equal(TradeStatus.Initialized, trade.TradeStatus);
            Assert.Equal("TestTrade", trade.TradeType.ToString());
            Assert.Equal(new Symbol("AUDUSD", Venue.FXCM), trade.Symbol);
            Assert.Equal("TestTrade", trade.Id.ToString());
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
            Assert.Equal(9, trade.OrderIdList.Count); // 3 ELS orders with 3 order each = 9 orderId's
            Assert.Equal(Quantity.Create(3), trade.TotalQuantity);
            Assert.Equal(3, trade.TradeUnits.Count);
        }

        [Fact]
        internal void GetOrderById_OrderIsWithinATradeUnit_ReturnsCorrectOrder()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder1 = new StubOrderBuilder().StopLossOrder("StoplossOrderId1").BuildStopMarketOrder();
            var stopLossOrder2 = new StubOrderBuilder().StopLossOrder("StoplossOrderId2").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder1,
                profitTargetOrder);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder2,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder1,
                                    atomicOrder1,
                                    atomicOrder2,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            var result = trade.GetOrderById(new OrderId("StoplossOrderId1"));

            // Assert
            Assert.Equal(stopLossOrder1, result);
        }

        [Fact]
        internal void GetOrderById_OrderIsNotWithinTrade_ReturnsNull()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StopLossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            var result = trade.GetOrderById(new OrderId("bad_StopLossOrderId"));

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Fact]
        internal void Apply_OneEntryOrderWorking_ReturnsTradeStatusPending()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));

            // Assert
            Assert.Equal(TradeStatus.Pending, trade.TradeStatus);
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_OneEntryOrderFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Long, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_OneEntryOrderPartiallyFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderPartiallyFilledEvent(entryOrder, 50000, 50000));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Long, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_AllEntryOrderPartiallyFilledSellCase_ReturnsTradeStatusActiveMarketPositionShort()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderPartiallyFilledEvent(entryOrder, 50000, 50000));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Short, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_AllOrdersRejected_ReturnsTradeStatusCompleteMarketPositionFlat()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderRejectedEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderRejectedEvent(stopLossOrder));
            trade.Apply(StubEventMessages.OrderRejectedEvent(profitTargetOrder));

            // Assert
            Assert.Equal(TradeStatus.Completed, trade.TradeStatus);
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_OneOrderRejectedOneOrderFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var entryOrder1 = new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarketOrder();
            var entryOrder2 = new StubOrderBuilder().EntryOrder("EntryOrderId2").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder1,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder2,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder1,
                                    atomicOrder1,
                                    atomicOrder2,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderRejectedEvent(entryOrder2));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder1));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Long, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_TwoPositionsLongOnePositionShort_ReturnsTradeStatusActiveMarketPositionFlat()
        {
            // Arrange
            var entryOrder1 = new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarketOrder();
            var entryOrder2 = new StubOrderBuilder().EntryOrder("EntryOrderId2").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder1,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder2,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder1,
                                    atomicOrder1,
                                    atomicOrder2,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder2));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder1));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Unknown, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_AllOrdersCancelled_ReturnsTradeStatusCompleteMarketPositionFlat()
        {
            // Arrange
            var entryOrder1 = new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarketOrder();
            var entryOrder2 = new StubOrderBuilder().EntryOrder("EntryOrderId2").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder1,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder2,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder1,
                                    atomicOrder1,
                                    atomicOrder2,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderCancelledEvent(entryOrder1));
            trade.Apply(StubEventMessages.OrderCancelledEvent(entryOrder2));
            trade.Apply(StubEventMessages.OrderCancelledEvent(stopLossOrder));
            trade.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder));

            // Assert
            Assert.Equal(TradeStatus.Completed, trade.TradeStatus);
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_AllEntryOrdersExpiredOtherOrdersRemain_ReturnsTradeStatusActiveMarketPositionFlat()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderExpiredEvent(entryOrder));

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_AllEntryOrdersExpiredAllOtherOrdersCancelled_ReturnsTradeStatusCompleteMarketPositionFlat()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder,
                                    atomicOrder,
                                    atomicOrder,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderExpiredEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderCancelledEvent(stopLossOrder));
            trade.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder));

            // Assert
            Assert.Equal(TradeStatus.Completed, trade.TradeStatus);
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
        }

        [Fact]
        internal void Apply_EntryOrdersFilledThenStoplossOrdersModified_ReturnsTradeStatusActiveMarketPositionLongNewStoplossPrice()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stopLossOrder1 = new StubOrderBuilder().StopLossOrder("StoplossOrderId1").BuildStopMarketOrder();
            var stopLossOrder2 = new StubOrderBuilder().StopLossOrder("StoplossOrderId2").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder1,
                profitTargetOrder);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stopLossOrder2,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder>
                                {
                                    atomicOrder1,
                                    atomicOrder1,
                                    atomicOrder2,
                                };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder));
            trade.Apply(StubEventMessages.OrderWorkingEvent(stopLossOrder2));
            trade.Apply(StubEventMessages.OrderModifiedEvent(stopLossOrder2, Price.Create(0.79950m, 0.00001m)));

            var result = trade.GetOrderById(new OrderId("StoplossOrderId2")).Value;

            // Assert
            Assert.Equal(TradeStatus.Active, trade.TradeStatus);
            Assert.Equal(MarketPosition.Long, trade.MarketPosition);
            Assert.Equal(0.79950m, result?.Price.Value.Value);
        }

        [Fact]
        internal void Apply_EntryOrdersFilledThenStoppedOut_ReturnsTradeStatusCompleted()
        {
            // Arrange
            var entryOrder1 = new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarketOrder();
            var stopLossOrder1 = new StubOrderBuilder().StopLossOrder("StoplossOrderId1").BuildStopMarketOrder();
            var profitTargetOrder1 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId1").BuildStopLimitOrder();

            var entryOrder2 = new StubOrderBuilder().EntryOrder("EntryOrderId2").BuildStopMarketOrder();
            var stopLossOrder2 = new StubOrderBuilder().StopLossOrder("StoplossOrderId2").BuildStopMarketOrder();
            var profitTargetOrder2 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").BuildStopLimitOrder();

            var entryOrder3 = new StubOrderBuilder().EntryOrder("EntryOrderId3").BuildStopMarketOrder();
            var stopLossOrder3 = new StubOrderBuilder().StopLossOrder("StoplossOrderId3").BuildStopMarketOrder();
            var profitTargetOrder3 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId3").BuildStopLimitOrder();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder1,
                stopLossOrder1,
                profitTargetOrder1);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder2,
                stopLossOrder2,
                profitTargetOrder2);

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder3,
                stopLossOrder3,
                profitTargetOrder3);

            var atomicOrders = new List<AtomicOrder>
                                   {
                                       atomicOrder1,
                                       atomicOrder2,
                                       atomicOrder3,
                                   };

            var orderPacket = new AtomicOrdersPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            var trade = TradeFactory.Create(orderPacket);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder1));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder1));
            trade.Apply(StubEventMessages.OrderWorkingEvent(stopLossOrder1));
            trade.Apply(StubEventMessages.OrderFilledEvent(stopLossOrder1));
            trade.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder1));

            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder2));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder2));
            trade.Apply(StubEventMessages.OrderWorkingEvent(stopLossOrder2));
            trade.Apply(StubEventMessages.OrderFilledEvent(stopLossOrder2));
            trade.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder2));

            trade.Apply(StubEventMessages.OrderWorkingEvent(entryOrder3));
            trade.Apply(StubEventMessages.OrderFilledEvent(entryOrder3));
            trade.Apply(StubEventMessages.OrderWorkingEvent(stopLossOrder3));
            trade.Apply(StubEventMessages.OrderFilledEvent(stopLossOrder3));
            trade.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder3));

            // Assert
            Assert.Equal(MarketPosition.Flat, trade.MarketPosition);
            Assert.Equal(TradeStatus.Completed, trade.TradeStatus);
        }
    }
}
