//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelReplaceRequestFactory.cs" company="Nautech Systems Pty Ltd">
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
    /// Provides order cancel replace request FIX messages.
    /// </summary>
    public static class OrderCancelReplaceRequestFactory
    {
        /// <summary>
        /// Creates and returns a new order cancel replace request message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="order">The order.</param>
        /// <param name="stopLossPrice">The stop-loss price.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The <see cref="OrderCancelReplaceRequest"/> message.</returns>
        public static OrderCancelReplaceRequest Create(
            string brokerSymbol,
            Order order,
            decimal stopLossPrice,
            ZonedDateTime transactionTime)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(order, nameof(order));
            Debug.DecimalNotOutOfRange(stopLossPrice, nameof(stopLossPrice), decimal.Zero, decimal.MaxValue);
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var orderMessage = new OrderCancelReplaceRequest();

            orderMessage.SetField(new OrigClOrdID(order.OrderId.ToString()));
            orderMessage.SetField(new OrderID(order.BrokerOrderId.ToString()));
            orderMessage.SetField(new ClOrdID(order.CurrentOrderId.ToString()));
            orderMessage.SetField(new Symbol(brokerSymbol));
            orderMessage.SetField(new Quantity(order.Quantity.Value));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderSide(order.OrderSide));
            orderMessage.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderType(order.OrderType));
            orderMessage.SetField(new StopPx(stopLossPrice));

            return orderMessage;
        }
    }
}
