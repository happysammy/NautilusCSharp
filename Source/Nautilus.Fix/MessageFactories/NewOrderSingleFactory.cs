//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Account = QuickFix.Fields.Account;
    using Symbol = QuickFix.Fields.Symbol;

    /// <summary>
    /// Provides new order single FIX messages.
    /// </summary>
    public static class NewOrderSingleFactory
    {
        /// <summary>
        /// Creates and returns a new new order single message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="order">The order to submit.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static QuickFix.FIX44.NewOrderSingle Create(
            string brokerSymbol,
            AccountNumber accountNumber,
            Order order,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderSingle();

            message.SetField(new ClOrdID(order.Id.Value));
            message.SetField(new SecondaryClOrdID(order.Label.Value));
            message.SetField(new Account(accountNumber.Value));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(FixMessageHelper.GetFixOrderSide(order.OrderSide));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));
            message.SetField(FixMessageHelper.GetFixOrderType(order.OrderType));
            message.SetField(FixMessageHelper.GetFixTimeInForce(order.TimeInForce));

            if (order.ExpireTime.HasValue)
            {
                var expireTime = order.ExpireTime.Value.ToDateTimeUtc();
                message.SetField(new ExpireTime(expireTime));
            }

            message.SetField(new OrderQty(order.Quantity.Value));

            switch (order.OrderType)
            {
                case OrderType.MARKET:
                    // Do nothing.
                    break;
                case OrderType.LIMIT:
                case OrderType.STOP_LIMIT:
                    if (order.Price?.Value != null)
                    {
                        message.SetField(new Price(order.Price.Value));
                    }

                    break;
                case OrderType.STOP_MARKET:
                case OrderType.MIT:
                    if (order.Price?.Value != null)
                    {
                        message.SetField(new StopPx(order.Price.Value));
                    }

                    break;
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(order.OrderType, nameof(order.OrderType));
            }

            return message;
        }
    }
}
