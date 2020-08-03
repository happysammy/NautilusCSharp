//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderListEntryFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using NodaTime;
using QuickFix.Fields;
using QuickFix.FIX44;
using Symbol = QuickFix.Fields.Symbol;

namespace Nautilus.Fxcm.MessageFactories
{
    /// <summary>
    /// Provides <see cref="NewOrderList"/> FIX messages.
    /// </summary>
    public static class NewOrderListEntryFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NewOrderList"/> FIX message with contingency orders.
        /// </summary>
        /// <param name="symbolCode">The brokers symbol.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="bracketOrder">The bracket order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderList CreateWithStopLoss(
            string symbolCode,
            AccountNumber accountNumber,
            BracketOrder bracketOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(2));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(2));
            message.SetField(new BidType(3));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));

            // Order 1
            var entry = bracketOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.Value));
            order1.SetField(new ListSeqNo(0));

            if (entry.Label.NotNone())
            {
                order1.SetField(new SecondaryClOrdID(entry.Label.Value));
            }

            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber.Value));
            order1.SetField(new Symbol(symbolCode));
            order1.SetField(FxcmMessageHelper.GetFixOrderSide(entry.OrderSide));
            order1.SetField(FxcmMessageHelper.GetFixOrderType(entry.OrderType));
            order1.SetField(FxcmMessageHelper.GetFixTimeInForce(entry.TimeInForce));
            order1.SetField(new OrderQty(entry.Quantity.Value));

            // Add price
            if (entry.Price?.Value != null)
            {
                switch (entry.OrderType)
                {
                    case OrderType.Limit:
                        order1.SetField(new Price(entry.Price.Value));
                        break;
                    case OrderType.Stop:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.StopLimit:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.MIT:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.Market:
                    case OrderType.Undefined:
                        goto default;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(entry.OrderType, nameof(entry.OrderType));
                }
            }

            // Optional tags
            if (entry.ExpireTime.HasValue)
            {
                var expireTime = FxcmMessageHelper.ToExpireTimeFormat(entry.ExpireTime.Value);
                order1.SetField(new StringField(126, expireTime));
            }

            // Order 2 -----------------------------------------------------------------------------
            var stopLoss = bracketOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.Value));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber.Value));
            order2.SetField(new Symbol(symbolCode));
            order2.SetField(FxcmMessageHelper.GetFixOrderSide(stopLoss.OrderSide));
            order2.SetField(new OrdType(OrdType.STOP));
            order2.SetField(FxcmMessageHelper.GetFixTimeInForce(stopLoss.TimeInForce));
            order2.SetField(new OrderQty(stopLoss.Quantity.Value));

            // Optional tags
            if (stopLoss.Label.NotNone())
            {
                order2.SetField(new SecondaryClOrdID(stopLoss.Label.Value));
            }

            // Stop-loss orders should always have a stop price
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
        /// <param name="symbolCode">The brokers symbol.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        /// <param name="bracketOrder">The bracket order.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderList CreateWithStopLossAndTakeProfit(
            string symbolCode,
            AccountNumber accountNumber,
            BracketOrder bracketOrder,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderList();
            message.SetField(new ListID(timeNow.TickOfDay.ToString()));
            message.SetField(new TotNoOrders(3));
            message.SetField(new ContingencyType(101));
            message.SetField(new NoOrders(3));
            message.SetField(new BidType(3));

            // Order 1 -----------------------------------------------------------------------------
            var entry = bracketOrder.Entry;
            var order1 = new NewOrderList.NoOrdersGroup();
            order1.SetField(new ClOrdID(entry.Id.Value));
            order1.SetField(new ListSeqNo(0));
            order1.SetField(new ClOrdLinkID("1"));
            order1.SetField(new Account(accountNumber.Value));
            order1.SetField(new Symbol(symbolCode));
            order1.SetField(FxcmMessageHelper.GetFixOrderSide(entry.OrderSide));
            order1.SetField(FxcmMessageHelper.GetFixOrderType(entry.OrderType));
            order1.SetField(FxcmMessageHelper.GetFixTimeInForce(entry.TimeInForce));
            order1.SetField(new OrderQty(entry.Quantity.Value));

            // Add price
            if (entry.Price?.Value != null)
            {
                switch (entry.OrderType)
                {
                    case OrderType.Limit:
                        order1.SetField(new Price(entry.Price.Value));
                        break;
                    case OrderType.Stop:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.StopLimit:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.MIT:
                        order1.SetField(new StopPx(entry.Price.Value));
                        break;
                    case OrderType.Market:
                    case OrderType.Undefined:
                        goto default;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(entry.OrderType, nameof(entry.OrderType));
                }
            }

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

            // Order 2 -----------------------------------------------------------------------------
            var stopLoss = bracketOrder.StopLoss;
            var order2 = new NewOrderList.NoOrdersGroup();
            order2.SetField(new ClOrdID(stopLoss.Id.Value));
            order2.SetField(new ListSeqNo(1));
            order2.SetField(new ClOrdLinkID("2"));
            order2.SetField(new Account(accountNumber.Value));
            order2.SetField(new Symbol(symbolCode));
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
            var takeProfit = bracketOrder.TakeProfit;
            if (takeProfit != null)
            {
                var order3 = new NewOrderList.NoOrdersGroup();
                order3.SetField(new ClOrdID(takeProfit.Id.Value));
                order3.SetField(new ListSeqNo(2));
                order3.SetField(new ClOrdLinkID("2"));
                order3.SetField(new Account(accountNumber.Value));
                order3.SetField(new Symbol(symbolCode));
                order3.SetField(FxcmMessageHelper.GetFixOrderSide(takeProfit.OrderSide));
                order3.SetField(FxcmMessageHelper.GetFixOrderType(takeProfit.OrderType));
                order3.SetField(FxcmMessageHelper.GetFixTimeInForce(takeProfit.TimeInForce));
                order3.SetField(new OrderQty(takeProfit.Quantity.Value));

                // Optional tags
                if (takeProfit.Label.NotNone())
                {
                    order3.SetField(new SecondaryClOrdID(entry.Label.Value));
                }

                // Take-profit orders should always have a limit price
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
