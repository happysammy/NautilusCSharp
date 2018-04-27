// -------------------------------------------------------------------------------------------------
// <copyright file="StubOrderPacketBuilder.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The stub trade unit builder.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubOrderPacketBuilder
    {
        public static AtomicOrderPacket Build()
        {
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarket();
            var stoplossOrder = new StubOrderBuilder().StoplossOrder("StoplossOrderId").BuildStopMarket();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarket();

            var atomicOrder = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder,
                stoplossOrder,
                profitTargetOrder);

            var atomicOrders = new List<AtomicOrder> { atomicOrder };

            return new AtomicOrderPacket(
                entryOrder.Symbol,
                new TradeType("TestTrade"),
                atomicOrders,
                new EntityId("StubOrderPacket"),
                StubDateTime.Now());
        }

        public static AtomicOrderPacket ThreeUnitsAndExpireTime(ZonedDateTime expireTime)
        {
            var entryOrder1 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId1").WithExpireTime(expireTime).BuildStopMarket();
            var stoplossOrder1 = new StubOrderBuilder().StoplossOrder("StoplossOrderId1").BuildStopMarket();
            var profitTargetOrder1 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId1").BuildStopMarket();

            var entryOrder2 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId2").WithExpireTime(expireTime).BuildStopMarket();
            var stoplossOrder2 = new StubOrderBuilder().StoplossOrder("StoplossOrderId2").BuildStopMarket();
            var profitTargetOrder2 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").BuildStopMarket();

            var entryOrder3 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId3").WithExpireTime(expireTime).BuildStopMarket();
            var stoplossOrder3 = new StubOrderBuilder().StoplossOrder("StoplossOrderId3").BuildStopMarket();
            var profitTargetOrder3 = Option<StopOrder>.None();

            var atomicOrder1 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder1,
                stoplossOrder1,
                profitTargetOrder1);

            var atomicOrder2 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder2,
                stoplossOrder2,
                profitTargetOrder2);

            var atomicOrder3 = new AtomicOrder(
                new TradeType("TestTrade"),
                entryOrder3,
                stoplossOrder3,
                profitTargetOrder3);

            var atomicOrders = new List<AtomicOrder> { atomicOrder1, atomicOrder2, atomicOrder3 };

            return new AtomicOrderPacket(
                entryOrder1.Symbol,
                new TradeType("TestTrade"),
                atomicOrders,
                new EntityId("StubOrderPacket"),
                StubDateTime.Now());
        }
    }
}