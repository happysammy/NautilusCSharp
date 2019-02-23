//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderListEntryFactory.cs" company="Nautech Systems Pty Ltd">
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

    /// <summary>
    /// Provides order list entry stop FIX messages.
    /// </summary>
    public static class NewOrderListEntryFactory
    {
        /// <summary>
        /// Creates and returns a new order list entry FIX message with entry and stop orders.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderList CreateWithStopLoss(
            string brokerSymbol,
            string accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(2));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(2));
            message.SetField(new BidType(3));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));

            var entry = atomicOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.ToString()));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new SecondaryClOrdID(entry.Label.ToString()));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FixMessageHelper.GetFixOrderSide(entry.Side));
            order1.SetField(new OrdType(OrdType.STOP));
            order1.SetField(FixMessageHelper.GetFixTimeInForce(entry.TimeInForce));

            if (entry.ExpireTime.HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException (checked above)
                var expireTime = entry.ExpireTime.Value.Value.ToDateTimeUtc();
                order1.SetField(new ExpireTime(expireTime));
            }

            order1.SetField(new OrderQty(entry.Quantity.Value));
            order1.SetField(new StopPx(entry.Price.Value.Value));
            message.AddGroup(order1);

            var stopLoss = atomicOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.ToString()));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new SecondaryClOrdID(stopLoss.Label.ToString()));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FixMessageHelper.GetFixOrderSide(stopLoss.Side));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FixMessageHelper.GetFixTimeInForce(stopLoss.TimeInForce));
            order2.SetField(new OrderQty(stopLoss.Quantity.Value));
            order2.SetField(new StopPx(stopLoss.Price.Value.Value));
            message.AddGroup(order2);

            return message;
        }

        /// <summary>
        /// Creates and returns a new order list entry FIX message with entry, stop and limit orders.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderList CreateWithStopLossAndProfitTarget(
            string brokerSymbol,
            string accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(atomicOrder, nameof(atomicOrder));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(3));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(3));
            message.SetField(new BidType(3));

            var entry = atomicOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.ToString()));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new SecondaryClOrdID(entry.Label.ToString()));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FixMessageHelper.GetFixOrderSide(entry.Side));
            order1.SetField(new OrdType(OrdType.STOP));
            order1.SetField(FixMessageHelper.GetFixTimeInForce(entry.TimeInForce));

            if (entry.ExpireTime.HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException (already checked above).
                var expireTime = entry.ExpireTime.Value.Value.ToDateTimeUtc();
                order1.SetField(new ExpireTime(expireTime));
            }

            order1.SetField(new OrderQty(entry.Quantity.Value));
            order1.SetField(new StopPx(entry.Price.Value.Value));
            message.AddGroup(order1);

            var stopLoss = atomicOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.ToString()));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new SecondaryClOrdID(stopLoss.Label.ToString()));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FixMessageHelper.GetFixOrderSide(stopLoss.Side));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FixMessageHelper.GetFixTimeInForce(stopLoss.TimeInForce));
            order2.SetField(new OrderQty(stopLoss.Quantity.Value));
            order2.SetField(new StopPx(stopLoss.Price.Value.Value));
            message.AddGroup(order2);

            var profitTarget = atomicOrder.ProfitTarget;
            var order3 = new NewOrderList.NoOrdersGroup();
            order3.SetField(new ClOrdID(profitTarget.Value.Id.ToString()));
            order3.SetField(new ListSeqNo(2));
            order3.SetField(new SecondaryClOrdID(profitTarget.Value.Label.ToString()));
            order3.SetField(new ClOrdLinkID("2"));
            order3.SetField(new Account(accountNumber));
            order3.SetField(new Symbol(brokerSymbol));
            order3.SetField(FixMessageHelper.GetFixOrderSide(profitTarget.Value.Side));
            order3.SetField(new OrdType(OrdType.LIMIT));
            order3.SetField(FixMessageHelper.GetFixTimeInForce(profitTarget.Value.TimeInForce));
            order3.SetField(new OrderQty(profitTarget.Value.Quantity.Value));
            order3.SetField(new Price(profitTarget.Value.Price.Value.Value));
            message.AddGroup(order3);

            return message;
        }
    }
}
