//--------------------------------------------------------------------------------------------------
// <copyright file="StubTradeBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTradeBuilder
    {
        public static Trade BuyOneUnit()
        {
            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StopLossOrderId").BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder());

            var atomicOrders = new List<AtomicOrder> { atomicOrder };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade SellOneUnit()
        {
            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StopLossOrderId").WithOrderSide(OrderSide.BUY).WithPrice(Price.Create(1.30000m, 5)).BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder());
            var atomicOrders = new List<AtomicOrder> { atomicOrder };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade BuyThreeUnits()
        {
            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId1").BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId1").BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder());

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId2").BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId2").BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").BuildStopMarketOrder());

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId3").BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId3").BuildStopMarketOrder(),
                Option<Order>.None());

            var atomicOrders = new List<AtomicOrder> { atomicOrder1, atomicOrder2, atomicOrder3 };
            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            return TradeFactory.Create(orderPacket);
        }

        public static Trade SellThreeUnits()
        {
            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId1").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId1").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId1").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder());

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId2").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId2").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder(),
                new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder());

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().EntryOrder("EntryOrderId3").WithOrderSide(OrderSide.SELL).BuildStopMarketOrder(),
                new StubOrderBuilder().StopLossOrder("StoplossOrderId3").WithOrderSide(OrderSide.BUY).BuildStopMarketOrder(),
                Option<Order>.None());

            var atomicOrders = new List<AtomicOrder> { atomicOrder1, atomicOrder2, atomicOrder3 };

            var orderPacket = new AtomicOrderPacket(
                atomicOrders[0].Symbol,
                atomicOrders[0].TradeType,
                atomicOrders,
                new OrderPacketId("TestTrade"),
                StubZonedDateTime.UnixEpoch());

            return TradeFactory.Create(orderPacket);
        }
    }
}
