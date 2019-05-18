//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelReplaceRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using System.ComponentModel;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides order cancel replace request FIX messages.
    /// </summary>
    public static class OrderCancelReplaceRequestFactory
    {
        /// <summary>
        /// Creates and returns a new order cancel replace request message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="order">The order.</param>
        /// <param name="modifiedPrice">The price to modify the order to.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The FIX message.</returns>
        public static OrderCancelReplaceRequest Create(
            string brokerSymbol,
            Order order,
            decimal modifiedPrice,
            ZonedDateTime transactionTime)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var message = new OrderCancelReplaceRequest();

            message.SetField(new OrigClOrdID(order.Id.ToString()));
            message.SetField(new OrderID(order.IdBroker.ToString()));
            message.SetField(new ClOrdID(order.IdCurrent.ToString()));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(new Quantity(order.Quantity.Value));
            message.SetField(FixMessageHelper.GetFixOrderSide(order.Side));
            message.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));
            message.SetField(FixMessageHelper.GetFixOrderType(order.Type));

            // Set the order price depending on order type.
            switch (order.Type)
            {
                case OrderType.MARKET:
                    break;
                case OrderType.LIMIT:
                    message.SetField(new Price(modifiedPrice));
                    break;
                case OrderType.STOP_LIMIT:
                    message.SetField(new Price(modifiedPrice));
                    break;
                case OrderType.STOP_MARKET:
                    message.SetField(new StopPx(modifiedPrice));
                    break;
                case OrderType.MIT:
                    message.SetField(new StopPx(modifiedPrice));
                    break;
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgumentException(order.Type, nameof(order.Type));
            }

            return message;
        }
    }
}
