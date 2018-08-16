//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Interfaces;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

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
        /// <returns>The created new order single message.</returns>
        public static QuickFix.FIX44.NewOrderSingle Create(
            string brokerSymbol,
            string accountNumber,
            IOrder order,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(order, nameof(order));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderSingle();

            message.SetField(new ClOrdID(order.Id.ToString()));
            message.SetField(new SecondaryClOrdID(order.Label.ToString()));
            message.SetField(new Account(accountNumber));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(FixMessageHelper.GetFixOrderSide(order.Side));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));
            message.SetField(FixMessageHelper.GetFixOrderType(order.Type));
            message.SetField(FixMessageHelper.GetFixTimeInForce(order.TimeInForce));

            if (order.ExpireTime.HasValue)
            {
                // ReSharper disable once PossibleInvalidOperationException (checked above)
                var expireTime = order.ExpireTime.Value.Value.ToDateTimeUtc();
                message.SetField(new ExpireTime(expireTime));
            }

            message.SetField(new OrderQty(order.Quantity.Value));

            switch (order.Type)
            {
                // Set the order price depending on order type.
                case OrderType.LIMIT:
                case OrderType.STOP_LIMIT:
                    message.SetField(new Price(order.Price.Value.Value));
                    break;
                case OrderType.STOP_MARKET:
                case OrderType.MIT:
                    message.SetField(new StopPx(order.Price.Value.Value));
                    break;
            }

            return message;
        }
    }
}
