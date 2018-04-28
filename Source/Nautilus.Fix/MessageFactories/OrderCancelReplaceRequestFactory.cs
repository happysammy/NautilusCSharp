//---------------------------------------------------------------------------------
// <copyright file="OrderCancelReplaceRequestFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Brokerage.FXCM;
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// The order cancel replace request.
    /// </summary>
    public static class OrderCancelReplaceRequestFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="stopLossPrice">
        /// The stop-loss price.
        /// </param>
        /// <param name="transactionTime">
        /// The transaction time.
        /// </param>
        /// <returns>
        /// The <see cref="OrderCancelReplaceRequest"/>.
        /// </returns>
        public static OrderCancelReplaceRequest Create(
            Order order,
            DomainModel.ValueObjects.Price stopLossPrice,
            ZonedDateTime transactionTime)
        {
            var orderMessage = new OrderCancelReplaceRequest();

            orderMessage.SetField(new OrigClOrdID(order.OrderId.ToString()));
            orderMessage.SetField(new OrderID(order.BrokerOrderId.ToString()));
            orderMessage.SetField(new ClOrdID(order.CurrentOrderId.ToString()));
            orderMessage.SetField(new Symbol(FxcmSymbolMapper.GetFxcmSymbol(order.Symbol.Code).Value));
            orderMessage.SetField(new Quantity(order.Quantity.Value));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderSide(order.OrderSide));
            orderMessage.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));
            orderMessage.SetField(FxcmFixMessageHelper.GetFixOrderType(order.OrderType));
            orderMessage.SetField(new StopPx(stopLossPrice.Value));

            return orderMessage;
        }
    }
}
