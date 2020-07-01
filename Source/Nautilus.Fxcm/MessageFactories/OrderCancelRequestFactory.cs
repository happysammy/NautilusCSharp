//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCancelRequestFactory.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Aggregates;
using NodaTime;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Nautilus.Fxcm.MessageFactories
{
    /// <summary>
    /// Provides <see cref="OrderCancelRequest"/> FIX messages.
    /// </summary>
    public static class OrderCancelRequestFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="OrderCancelRequest"/> FIX message.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="transactionTime">The transaction time.</param>
        /// <returns>The FIX message.</returns>
        public static OrderCancelRequest Create(
            Order order,
            ZonedDateTime transactionTime)
        {
            Debug.NotDefault(transactionTime, nameof(transactionTime));

            var message = new OrderCancelRequest();

            message.SetField(new OrigClOrdID(order.Id.Value));
            message.SetField(new OrderID(order.IdBroker?.Value));
            message.SetField(new ClOrdID(order.Id.Value));
            message.SetField(new Symbol(order.Symbol.Code));
            message.SetField(new Quantity(order.Quantity.Value));
            message.SetField(FxcmMessageHelper.GetFixOrderSide(order.OrderSide));
            message.SetField(new TransactTime(transactionTime.ToDateTimeUtc()));

            return message;
        }
    }
}
