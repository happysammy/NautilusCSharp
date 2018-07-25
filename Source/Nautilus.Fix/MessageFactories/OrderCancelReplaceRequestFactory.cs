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
    using Nautilus.DomainModel.Interfaces;
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
        /// <returns>The <see cref="OrderCancelReplaceRequest"/> message.</returns>
        public static OrderCancelReplaceRequest Create(
            string brokerSymbol,
            IOrder order,
            Nautilus.DomainModel.ValueObjects.Price modifiedPrice,
            ZonedDateTime transactionTime)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(order, nameof(order));
            Debug.NotNull(modifiedPrice, nameof(modifiedPrice));
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var orderMessage = new OrderCancelReplaceRequest();

            orderMessage.SetField(new OrigClOrdID(order.Id.ToString()));
            orderMessage.SetField(new OrderID(order.IdBroker.ToString()));
            orderMessage.SetField(new ClOrdID(order.IdCurrent.ToString()));
            orderMessage.SetField(new Symbol(brokerSymbol));
            orderMessage.SetField(new Quantity(order.Quantity.Value));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderSide(order.Side));
            orderMessage.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderType(order.Type));
            orderMessage.SetField(new StopPx(modifiedPrice.Value));

            return orderMessage;
        }
    }
}
