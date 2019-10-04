//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderListEntryFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm.MessageFactories
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Symbol = QuickFix.Fields.Symbol;

    /// <summary>
    /// Provides <see cref="NewOrderList"/> FIX messages.
    /// </summary>
    public static class NewOrderListEntryFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NewOrderList"/> FIX message with contingency orders.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderList CreateWithStopLoss(
            string brokerSymbol,
            AccountNumber accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(2));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(2));
            message.SetField(new BidType(3));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));

            // Order 1
            var entry = atomicOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.Value));
            order1.SetField(new ListSeqNo(0));

            if (entry.Label.NotNone())
            {
                order1.SetField(new SecondaryClOrdID(entry.Label.Value));
            }

            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber.Value));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FxcmMessageHelper.GetFixOrderSide(entry.OrderSide));
            order1.SetField(FxcmMessageHelper.GetFixOrderType(entry.OrderType));
            order1.SetField(FxcmMessageHelper.GetFixTimeInForce(entry.TimeInForce));
            order1.SetField(new OrderQty(entry.Quantity.Value));

            // Optional tags
            if (entry.ExpireTime.HasValue)
            {
                var expireTime = FxcmMessageHelper.ToExpireTimeFormat(entry.ExpireTime.Value);
                order1.SetField(new StringField(126, expireTime));
            }

            if (entry.Price?.Value != null)
            {
                order1.SetField(new StopPx(entry.Price.Value));
            }

            // Order 2
            var stopLoss = atomicOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.Value));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber.Value));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FxcmMessageHelper.GetFixOrderSide(stopLoss.OrderSide));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FxcmMessageHelper.GetFixTimeInForce(stopLoss.TimeInForce));
            order2.SetField(new OrderQty(stopLoss.Quantity.Value));

            // Optional tags
            if (stopLoss.Label.NotNone())
            {
                order2.SetField(new SecondaryClOrdID(stopLoss.Label.Value));
            }

            // Stop-loss orders should always have a price
            if (stopLoss.Price?.Value != null)
            {
                order2.SetField(new StopPx(stopLoss.Price.Value));
            }

            message.AddGroup(order1);
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
        public static NewOrderList CreateWithStopLossAndTakeProfit(
            string brokerSymbol,
            AccountNumber accountNumber,
            AtomicOrder atomicOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(3));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(3));
            message.SetField(new BidType(3));

            // Order 1
            var entry = atomicOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.Value));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber.Value));
            order1.SetField(new Symbol(brokerSymbol));
            order1.SetField(FxcmMessageHelper.GetFixOrderSide(entry.OrderSide));
            order1.SetField(FxcmMessageHelper.GetFixOrderType(entry.OrderType));
            order1.SetField(FxcmMessageHelper.GetFixTimeInForce(entry.TimeInForce));
            order1.SetField(new OrderQty(entry.Quantity.Value));

            // Optional tags
            if (entry.Label.NotNone())
            {
                order1.SetField(new SecondaryClOrdID(entry.Label.Value));
            }

            if (entry.ExpireTime.HasValue)
            {
                var expireTime = entry.ExpireTime.Value.ToDateTimeUtc();
                order1.SetField(new ExpireTime(expireTime));
            }

            if (entry.Price?.Value != null)
            {
                order1.SetField(new StopPx(entry.Price.Value));
            }

            // Order 2
            var stopLoss = atomicOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.Value));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber.Value));
            order2.SetField(new Symbol(brokerSymbol));
            order2.SetField(FxcmMessageHelper.GetFixOrderSide(stopLoss.OrderSide));
            order2.SetField(FxcmMessageHelper.GetFixOrderType(stopLoss.OrderType));
            order2.SetField(FxcmMessageHelper.GetFixTimeInForce(stopLoss.TimeInForce));
            order2.SetField(new OrderQty(stopLoss.Quantity.Value));

            // Optional tags
            if (stopLoss.Label.NotNone())
            {
                order2.SetField(new SecondaryClOrdID(entry.Label.Value));
            }

            // Stop-loss orders should always have a price
            if (stopLoss.Price?.Value != null)
            {
                order2.SetField(new StopPx(stopLoss.Price.Value));
            }

            message.AddGroup(order1);
            message.AddGroup(order2);

            // Order 3 (optional)
            var takeProfit = atomicOrder.TakeProfit;
            if (takeProfit != null)
            {
                var order3 = new NewOrderList.NoOrdersGroup();
                order3.SetField(new ClOrdID(takeProfit.Id.Value));
                order3.SetField(new ListSeqNo(2));
                order3.SetField(new ClOrdLinkID("2"));
                order3.SetField(new Account(accountNumber.Value));
                order3.SetField(new Symbol(brokerSymbol));
                order3.SetField(FxcmMessageHelper.GetFixOrderSide(takeProfit.OrderSide));
                order3.SetField(FxcmMessageHelper.GetFixOrderType(takeProfit.OrderType));
                order3.SetField(FxcmMessageHelper.GetFixTimeInForce(takeProfit.TimeInForce));
                order3.SetField(new OrderQty(takeProfit.Quantity.Value));

                // Optional tags
                if (takeProfit.Label.NotNone())
                {
                    order3.SetField(new SecondaryClOrdID(entry.Label.Value));
                }

                // Take-profit orders should always have a price
                if (takeProfit.Price?.Value != null)
                {
                    order3.SetField(new Price(takeProfit.Price.Value));
                }

                message.AddGroup(order3);
            }

            return message;
        }
    }
}
