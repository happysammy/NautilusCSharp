//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelReplaceRequestFactory.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Fxcm.MessageFactories
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides <see cref="OrderCancelReplaceRequest"/> FIX messages.
    /// </summary>
    public static class OrderCancelReplaceRequestFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="OrderCancelReplaceRequest"/> FIX message.
        /// </summary>
        /// <param name="brokerSymbol">The brokers symbol.</param>
        /// <param name="order">The order.</param>
        /// <param name="modifiedQuantity">The quantity to modify the order to.</param>
        /// <param name="modifiedPrice">The price to modify the order to.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The FIX message.</returns>
        public static OrderCancelReplaceRequest Create(
            string brokerSymbol,
            Order order,
            decimal modifiedQuantity,
            decimal modifiedPrice,
            ZonedDateTime transactionTime)
        {
            Debug.NotEmptyOrWhiteSpace(brokerSymbol, nameof(brokerSymbol));
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var message = new OrderCancelReplaceRequest();

            message.SetField(new OrigClOrdID(order.Id.Value));
            message.SetField(new OrderID(order.IdBroker?.Value));
            message.SetField(new ClOrdID(order.Id.Value));
            message.SetField(new Symbol(brokerSymbol));
            message.SetField(new Quantity(modifiedQuantity));
            message.SetField(FxcmMessageHelper.GetFixOrderSide(order.OrderSide));
            message.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));
            message.SetField(FxcmMessageHelper.GetFixOrderType(order.OrderType));

            // Set the order price depending on order type.
            switch (order.OrderType)
            {
                case OrderType.Market:
                    break;
                case OrderType.Limit:
                    message.SetField(new Price(modifiedPrice));
                    break;
                case OrderType.StopLimit:
                    message.SetField(new Price(modifiedPrice));
                    break;
                case OrderType.Stop:
                    message.SetField(new StopPx(modifiedPrice));
                    break;
                case OrderType.MIT:
                    message.SetField(new StopPx(modifiedPrice));
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
