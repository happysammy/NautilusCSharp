//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm.MessageFactories
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
    /// Provides <see cref="NewOrderSingle"/> FIX messages.
    /// </summary>
    public static class NewOrderSingleFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NewOrderSingle"/> FIX message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="order">The order to submit.</param>
        /// <param name="positionIdBroker">The optional broker position identifier.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderSingle Create(
            string brokerSymbol,
            AccountNumber accountNumber,
            Order order,
            PositionIdBroker? positionIdBroker,
            ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderSingle();

            message.SetField(new ClOrdID(order.Id.Value));
            message.SetField(new Account(accountNumber.Value));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(FxcmMessageHelper.GetFixOrderSide(order.OrderSide));
            message.SetField(FxcmMessageHelper.GetFixOrderType(order.OrderType));
            message.SetField(FxcmMessageHelper.GetFixTimeInForce(order.TimeInForce));
            message.SetField(new OrderQty(order.Quantity.Value));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));

            // Optional tags
            if (order.Label.NotNone())
            {
                message.SetField(new SecondaryClOrdID(order.Label.Value));
            }

            if (!(positionIdBroker is null))
            {
                message.SetField(new StringField(FxcmTags.PosID, positionIdBroker.Value));
            }

            if (order.ExpireTime.HasValue)
            {
                var expireTime = FxcmMessageHelper.ToExpireTimeFormat(order.ExpireTime.Value);
                message.SetField(new StringField(126, expireTime));
            }

            // Add price
            switch (order.OrderType)
            {
                case OrderType.Market:
                    // Do nothing
                    break;
                case OrderType.Limit:
                case OrderType.StopLimit:
                    if (order.Price?.Value != null)
                    {
                        message.SetField(new Price(order.Price.Value));
                    }

                    break;
                case OrderType.Stop:
                case OrderType.MIT:
                    if (order.Price?.Value != null)
                    {
                        message.SetField(new StopPx(order.Price.Value));
                    }

                    break;
                case OrderType.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(order.OrderType, nameof(order.OrderType));
            }

            return message;
        }
    }
}
