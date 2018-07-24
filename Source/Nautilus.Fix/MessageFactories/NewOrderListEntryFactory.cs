//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderListEntryStopFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Account = QuickFix.Fields.Account;

    /// <summary>
    /// Provides order list entry stop FIX messages.
    /// </summary>
    public static class NewOrderListEntryFactory
    {
        /// <summary>
        /// Creates and returns a new order list entry message for stop entries.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="NewOrderList"/> message.</returns>
        public static NewOrderList CreateWithStop(
            string brokerSymbol,
            string accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var orderList = new NewOrderList();
            orderList.SetField(new ListID(timeNow.TickOfDay.ToString()));
            orderList.SetField(new TotNoOrders(2));
            orderList.SetField(new ContingencyType(101));
            orderList.SetField(new NoOrders(2));
            orderList.SetField(new BidType(3));

           var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(atomicOrder.EntryOrder.OrderId.ToString()));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new SecondaryClOrdID(atomicOrder.EntryOrder.Label.ToString()));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.EntryOrder.Side));
            order1.SetField(new OrdType(OrdType.STOP));
            order1.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.EntryOrder.TimeInForce));

            if (atomicOrder.EntryOrder.ExpireTime.HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException (checked above)
                var expireTime = atomicOrder.EntryOrder.ExpireTime.Value.Value.ToDateTimeUtc();
                order1.SetField(new ExpireTime(expireTime));
            }

            order1.SetField(new OrderQty(atomicOrder.EntryOrder.Quantity.Value));
            order1.SetField(new StopPx(atomicOrder.EntryOrder.Price.Value));
            orderList.AddGroup(order1);

            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(atomicOrder.StopLossOrder.OrderId.ToString()));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new SecondaryClOrdID(atomicOrder.StopLossOrder.Label.ToString()));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account("02402856"));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.StopLossOrder.Side));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.StopLossOrder.TimeInForce));
            order2.SetField(new OrderQty(atomicOrder.StopLossOrder.Quantity.Value));
            order2.SetField(new StopPx(atomicOrder.StopLossOrder.Price.Value));
            orderList.AddGroup(order2);

            return orderList;
        }

        /// <summary>
        /// Creates and returns a new order list entry message for limit entries.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol</param>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="NewOrderList"/> message.</returns>
        public static NewOrderList CreateWithLimit(
            string brokerSymbol,
            string accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var orderList = new NewOrderList();
            orderList.SetField(new ListID(timeNow.TickOfDay.ToString()));
            orderList.SetField(new TotNoOrders(3));
            orderList.SetField(new ContingencyType(101));
            orderList.SetField(new NoOrders(3));
            orderList.SetField(new BidType(3));

           var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(atomicOrder.EntryOrder.OrderId.ToString()));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new SecondaryClOrdID(atomicOrder.EntryOrder.Label.ToString()));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.EntryOrder.Side));
            order1.SetField(new OrdType(OrdType.STOP));
            order1.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.EntryOrder.TimeInForce));

            if (atomicOrder.EntryOrder.ExpireTime.HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException (checked above)
                var expireTime = atomicOrder.EntryOrder.ExpireTime.Value.Value.ToDateTimeUtc();
                order1.SetField(new ExpireTime(expireTime));
            }

            order1.SetField(new OrderQty(atomicOrder.EntryOrder.Quantity.Value));
            order1.SetField(new StopPx(atomicOrder.EntryOrder.Price.Value));
            orderList.AddGroup(order1);

            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(atomicOrder.StopLossOrder.OrderId.ToString()));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new SecondaryClOrdID(atomicOrder.StopLossOrder.Label.ToString()));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.StopLossOrder.Side));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.StopLossOrder.TimeInForce));
            order2.SetField(new OrderQty(atomicOrder.StopLossOrder.Quantity.Value));
            order2.SetField(new StopPx(atomicOrder.StopLossOrder.Price.Value));
            orderList.AddGroup(order2);

            var order3 = new NewOrderList.NoOrdersGroup();
            order3.SetField(new ClOrdID(atomicOrder.ProfitTargetOrder.Value.OrderId.ToString()));
            order3.SetField(new ListSeqNo(2));
            order3.SetField(new SecondaryClOrdID(atomicOrder.ProfitTargetOrder.Value.Label.ToString()));
            order3.SetField(new ClOrdLinkID("2"));
            order3.SetField(new Account(accountNumber));
            order3.SetField(new Symbol(brokerSymbol));
            order3.SetField(FxcmFixMessageHelper.GetFixOrderSide(atomicOrder.ProfitTargetOrder.Value.Side));
            order3.SetField(new OrdType(OrdType.LIMIT));
            order3.SetField(FxcmFixMessageHelper.GetFixTimeInForce(atomicOrder.ProfitTargetOrder.Value.TimeInForce));
            order3.SetField(new OrderQty(atomicOrder.ProfitTargetOrder.Value.Quantity.Value));
            order3.SetField(new Price(atomicOrder.ProfitTargetOrder.Value.Price.Value));
            orderList.AddGroup(order3);

            return orderList;
        }
    }
}
