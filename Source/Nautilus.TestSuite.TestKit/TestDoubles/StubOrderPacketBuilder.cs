//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderPacketBuilder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
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
            var entryOrder = new StubOrderBuilder().EntryOrder("EntryOrderId").BuildStopMarketOrder();
            var stoplossOrder = new StubOrderBuilder().StopLossOrder("StoplossOrderId").BuildStopMarketOrder();
            var profitTargetOrder = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId").BuildStopMarketOrder();

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
                StubZonedDateTime.UnixEpoch());
        }

        public static AtomicOrderPacket ThreeUnitsAndExpireTime(ZonedDateTime expireTime)
        {
            var entryOrder1 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId1").WithExpireTime(expireTime).BuildStopMarketOrder();
            var stoplossOrder1 = new StubOrderBuilder().StopLossOrder("StoplossOrderId1").BuildStopMarketOrder();
            var profitTargetOrder1 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId1").BuildStopMarketOrder();

            var entryOrder2 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId2").WithExpireTime(expireTime).BuildStopMarketOrder();
            var stoplossOrder2 = new StubOrderBuilder().StopLossOrder("StoplossOrderId2").BuildStopMarketOrder();
            var profitTargetOrder2 = new StubOrderBuilder().ProfitTargetOrder("ProfitTargetOrderId2").BuildStopMarketOrder();

            var entryOrder3 = new StubOrderBuilder().WithTimeInForce(TimeInForce.GTD).WithOrderId("EntryOrderId3").WithExpireTime(expireTime).BuildStopMarketOrder();
            var stoplossOrder3 = new StubOrderBuilder().StopLossOrder("StoplossOrderId3").BuildStopMarketOrder();
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
                StubZonedDateTime.UnixEpoch());
        }
    }
}