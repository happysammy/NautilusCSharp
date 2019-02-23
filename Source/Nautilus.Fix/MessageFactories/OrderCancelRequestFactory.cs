//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides order cancel request FIX messages.
    /// </summary>
    public static class OrderCancelRequestFactory
    {
        /// <summary>
        /// Creates and returns a new order cancel request message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="order">The order.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The FIX message.</returns>
        public static OrderCancelRequest Create(
            string brokerSymbol,
            Order order,
            ZonedDateTime transactionTime)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(order, nameof(order));
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var message = new OrderCancelRequest();

            message.SetField(new OrigClOrdID(order.Id.ToString()));
            message.SetField(new OrderID(order.IdBroker.ToString()));
            message.SetField(new ClOrdID(order.IdCurrent.ToString()));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(new Quantity(order.Quantity.Value));
            message.SetField(FixMessageHelper.GetFixOrderSide(order.Side));
            message.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));

            return message;
        }
    }
}
