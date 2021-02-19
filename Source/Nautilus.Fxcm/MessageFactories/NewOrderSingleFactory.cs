﻿//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using NodaTime;
using QuickFix.Fields;
using QuickFix.FIX44;
using Account = QuickFix.Fields.Account;
using Symbol = QuickFix.Fields.Symbol;

namespace Nautilus.Fxcm.MessageFactories
{
    /// <summary>
    /// Provides <see cref="NewOrderSingle"/> FIX messages.
    /// </summary>
    public static class NewOrderSingleFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NewOrderSingle"/> FIX message.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="order">The order to submit.</param>
        /// <param name="positionIdBroker">The optional broker position identifier.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static NewOrderSingle Create(
            AccountNumber accountNumber,
            Order order,
            PositionIdBroker? positionIdBroker,
            ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new NewOrderSingle();

            message.SetField(new ClOrdID(order.Id.Value));
            message.SetField(new Account(accountNumber.Value));
            message.SetField(new Symbol(order.Symbol.Code));
            message.SetField(FxcmMessageHelper.GetFixOrderSide(order.OrderSide));
            message.SetField(FxcmMessageHelper.GetFixOrderType(order.OrderType));
            message.SetField(FxcmMessageHelper.GetFixTimeInForce(order.TimeInForce));
            message.SetField(new OrderQty(order.Quantity.Value));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));

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
            if (order.Price?.Value != null)
            {
                switch (order.OrderType)
                {
                    case OrderType.Limit:
                        message.SetField(new Price(order.Price.Value));
                        break;
                    case OrderType.Stop:
                        message.SetField(new StopPx(order.Price.Value));
                        break;
                    case OrderType.StopLimit:
                        message.SetField(new StopPx(order.Price.Value));
                        break;
                    case OrderType.Market:
                    case OrderType.Undefined:
                        goto default;
                    default:
                        throw ExceptionFactory.InvalidSwitchArgument(order.OrderType, nameof(order.OrderType));
                }
            }

            return message;
        }
    }
}
