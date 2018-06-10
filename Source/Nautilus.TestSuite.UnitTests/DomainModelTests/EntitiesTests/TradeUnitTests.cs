//--------------------------------------------------------------------------------------------------
// <copyright file="TradeUnitTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.EntitiesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Orders;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TradeUnitTests
    {
        [Fact]
        internal void CanInitializeTradeUnit_ReturnsTradeStatusInitializedAndUnitSize()
        {
            var order = new StubOrderBuilder().BuildStopMarket();

            // Act
            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                order,
                order,
                order,
                StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal("U1", tradeUnit.TradeUnitLabel.ToString());
            Assert.Equal(order.Quantity, tradeUnit.UnitSize);
            Assert.Equal(TradeStatus.Initialized, tradeUnit.TradeStatus);
        }

        [Fact]
        internal void OrderStatus_EntryOrderWorking_ReturnsTradeStatusPendingAndMarketPositionFlat()
        {
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().EntryOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = new StubOrderBuilder().EntryOrder("ProfitTargetOrderId").BuildStopMarket();

            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                entryOrder,
                stoplossOrder,
                profitTargetOrder,
                StubZonedDateTime.UnixEpoch());

            var message = new OrderWorking(
                entryOrder.Symbol,
                entryOrder.OrderId,
                new EntityId("NONE"),
                entryOrder.OrderLabel,
                entryOrder.OrderSide,
                entryOrder.OrderType,
                entryOrder.Quantity,
                entryOrder.Price,
                entryOrder.TimeInForce,
                entryOrder.ExpireTime,
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            entryOrder.Apply(message);

            // Assert
            Assert.Equal(TradeStatus.Pending, tradeUnit.TradeStatus);
            Assert.Equal(MarketPosition.Flat, tradeUnit.Position.MarketPosition);
        }

        [Fact]
        internal void OrderStatus_AllOrdersFilled_ReturnsCompleted()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().StoplossOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarket();

            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                entryOrder,
                stoplossOrder,
                profitTargetOrder,
                StubZonedDateTime.UnixEpoch());

            // Act
            tradeUnit.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));
            tradeUnit.Apply(StubEventMessages.OrderFilledEvent(entryOrder));
            tradeUnit.Apply(StubEventMessages.OrderWorkingEvent(stoplossOrder));
            tradeUnit.Apply(StubEventMessages.OrderWorkingEvent(profitTargetOrder));
            tradeUnit.Apply(StubEventMessages.OrderFilledEvent(stoplossOrder));
            tradeUnit.Apply(StubEventMessages.OrderCancelledEvent(profitTargetOrder));

            // Assert
            Assert.Equal(MarketPosition.Flat, tradeUnit.Position.MarketPosition);
            Assert.Equal(TradeStatus.Completed, tradeUnit.TradeStatus);
        }

        [Fact]
        internal void OrderStatus_EntryAndStoplossFilledWithNullProfitTargetOrder_ReturnsCompleted()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().StoplossOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = Option<StopOrder>.None();

            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                entryOrder,
                stoplossOrder,
                profitTargetOrder,
                StubZonedDateTime.UnixEpoch());

            // Act
            tradeUnit.Apply(StubEventMessages.OrderWorkingEvent(entryOrder));
            tradeUnit.Apply(StubEventMessages.OrderFilledEvent(entryOrder));
            tradeUnit.Apply(StubEventMessages.OrderWorkingEvent(stoplossOrder));
            tradeUnit.Apply(StubEventMessages.OrderFilledEvent(stoplossOrder));

            // Assert
            Assert.Equal(MarketPosition.Flat, tradeUnit.Position.MarketPosition);
            Assert.Equal(TradeStatus.Completed, tradeUnit.TradeStatus);
        }

        [Fact]
        internal void GetOrderById_OrderIsInTradeUnit_ReturnsCorrectOrder()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().EntryOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = new StubOrderBuilder().EntryOrder("ProfitTargetOrderId").BuildStopMarket();

            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                entryOrder,
                stoplossOrder,
                profitTargetOrder,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = tradeUnit.GetOrderById(new EntityId("EntryOrderId"));

            // Assert
            Assert.Equal(entryOrder, result);
        }

        [Fact]
        internal void GetOrderById_OrderIsNotInTradeUnit_ReturnsOrderNull()
        {
            // Arrange
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().EntryOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = new StubOrderBuilder().EntryOrder("ProfitTargetOrderId").BuildStopMarket();

            var tradeUnit = new TradeUnit(
                new EntityId("NONE"),
                LabelFactory.TradeUnit(1),
                entryOrder,
                stoplossOrder,
                profitTargetOrder,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = tradeUnit.GetOrderById(new EntityId("bad_order_id"));

            // Assert
            Assert.Equal(Option<Order>.None(), result);
        }
    }
}
