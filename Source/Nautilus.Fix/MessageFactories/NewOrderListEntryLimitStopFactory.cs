//---------------------------------------------------------------------------------
// <copyright file="NewOrderListEntryLimitStopFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Brokerage.FXCM;
    using Nautilus.DomainModel.Entities;
    using NodaTime;

    using QuickFix.Fields;
    using QuickFix.FIX44;

    using Account = QuickFix.Fields.Account;

    /// <summary>
    /// The new order list entry limit stop.
    /// </summary>
    public static class NewOrderListEntryLimitStopFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="atomicOrder">
        /// The order.
        /// </param>
        /// <param name="timeNow">
        /// The time now.
        /// </param>
        /// <returns>
        /// The <see cref="NewOrderList"/>.
        /// </returns>
        public static NewOrderList Create(
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            var symbol = FxcmSymbolMapper.GetFxcmSymbol(atomicOrder.Symbol.Code).Value;

            var orderList = new NewOrderList();
            orderList.SetField(new ListID(timeNow.TickOfDay.ToString()));
            orderList.SetField(new TotNoOrders(3));
            orderList.SetField(new ContingencyType(101));
            orderList.SetField(new NoOrders(3));
            orderList.SetField(new BidType(3));

           var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(atomicOrder.EntryOrder.OrderId.ToString()));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new SecondaryClOrdID(atomicOrder.EntryOrder.OrderLabel.ToString()));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account("02402856"));
            order1.SetField(new Symbol(symbol));
            order1.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.EntryOrder.OrderSide));
            order1.SetField(new OrdType(OrdType.STOP));
            order1.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.EntryOrder.TimeInForce));

            if (atomicOrder.EntryOrder.ExpireTime.HasValue)
            {
                order1.SetField(new ExpireTime(atomicOrder.EntryOrder.ExpireTime.Value.Value.ToDateTimeUtc()));
            }

            order1.SetField(new OrderQty(atomicOrder.EntryOrder.Quantity.Value));
            order1.SetField(new StopPx(atomicOrder.EntryOrder.Price.Value));
            orderList.AddGroup(order1);

            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(atomicOrder.StopLossOrder.OrderId.ToString()));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new SecondaryClOrdID(atomicOrder.StopLossOrder.OrderLabel.ToString()));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account("02402856"));
            order2.SetField(new Symbol(symbol));
            order2.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.StopLossOrder.OrderSide));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.StopLossOrder.TimeInForce));
            order2.SetField(new OrderQty(atomicOrder.StopLossOrder.Quantity.Value));
            order2.SetField(new StopPx(atomicOrder.StopLossOrder.Price.Value));
            orderList.AddGroup(order2);

            var order3 = new NewOrderList.NoOrdersGroup();
            order3.SetField(new ClOrdID(atomicOrder.ProfitTargetOrder.Value.OrderId.ToString()));
            order3.SetField(new ListSeqNo(2));
            order3.SetField(new SecondaryClOrdID(atomicOrder.ProfitTargetOrder.Value.OrderLabel.ToString()));
            order3.SetField(new ClOrdLinkID("2"));
            order3.SetField(new Account("02402856"));
            order3.SetField(new Symbol(symbol));
            order3.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.ProfitTargetOrder.Value.OrderSide));
            order3.SetField(new OrdType(OrdType.LIMIT));
            order3.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.ProfitTargetOrder.Value.TimeInForce));
            order3.SetField(new OrderQty(atomicOrder.ProfitTargetOrder.Value.Quantity.Value));
            order3.SetField(new Price(atomicOrder.ProfitTargetOrder.Value.Price.Value));
            orderList.AddGroup(order3);

            return orderList;
        }
    }
}
