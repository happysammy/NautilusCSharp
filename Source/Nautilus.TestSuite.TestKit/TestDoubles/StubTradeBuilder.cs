// -------------------------------------------------------------------------------------------------
// <copyright file="StubTradeBuilder.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTradeBuilder
    {
        public static Trade BuyOneUnit()
        {
            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId").BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarket());

            var atomicOrders = new List<AtomicOrder> { atomicOrder };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new EntityId("TestTrade"),
                StubDateTime.Now());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade SellOneUnit()
        {
            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId").WithOrderSide(OrderSide.Sell).BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId").WithOrderSide(OrderSide.Buy).WithOrderPrice(Price.Create(1.30000m, 0.00001m)).BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").WithOrderSide(OrderSide.Buy).BuildStopMarket());
            var atomicOrders = new List<AtomicOrder> { atomicOrder };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new EntityId("TestTrade"),
                StubDateTime.Now());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade BuyThreeUnits()
        {
            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId1").BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarket());

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId2").BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId2").BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").BuildStopMarket());

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId3").BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId3").BuildStopMarket(),
                Option<StopOrder>.None());

            var atomicOrders = new List<AtomicOrder> { atomicOrder1, atomicOrder2, atomicOrder3 };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new EntityId("TestTrade"),
                StubDateTime.Now());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade SellThreeUnits()
        {
            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId1").WithOrderSide(OrderSide.Sell).BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId1").WithOrderSide(OrderSide.Buy).BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId1").WithOrderSide(OrderSide.Buy).BuildStopMarket());

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId2").WithOrderSide(OrderSide.Sell).BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId2").WithOrderSide(OrderSide.Buy).BuildStopMarket(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").WithOrderSide(OrderSide.Buy).BuildStopMarket());

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId3").WithOrderSide(OrderSide.Sell).BuildStopMarket(),
                new StubOrderBuilder().StoplossOrder("StoplossOrderId3").WithOrderSide(OrderSide.Buy).BuildStopMarket(),
                Option<StopOrder>.None());

            var atomicOrders = new List<AtomicOrder> { atomicOrder1, atomicOrder2, atomicOrder3 };

            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new EntityId("TestTrade"),
                StubDateTime.Now());

            return TradeFactory.Create(orderPacket);
        }
    }
}