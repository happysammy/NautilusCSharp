//--------------------------------------------------------------------------------------------------
// <copyright file="RequestForPositionsFactory.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Identifiers;
using NodaTime;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Nautilus.Fxcm.MessageFactories
{
    /// <summary>
    /// Provides <see cref="RequestForPositions"/> FIX messages.
    /// </summary>
    public static class RequestForPositionsFactory
    {
        private const string Broker = "FXCM";

        /// <summary>
        /// Creates and returns a new <see cref="RequestForPositions"/> FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="account">The account for the request.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static RequestForPositions OpenAll(ZonedDateTime timeNow, AccountNumber account, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            return Create(timeNow, 0, account, subscribe); // 0 = Open positions
        }

        /// <summary>
        /// Creates and returns a new <see cref="RequestForPositions"/> FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="account">The account for the request.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static RequestForPositions ClosedAll(ZonedDateTime timeNow, AccountNumber account, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            return Create(timeNow, 1, account, subscribe); // 1 = Closed positions
        }

        private static RequestForPositions Create(ZonedDateTime timeNow, int reqType, AccountNumber account,  bool subscribe)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new RequestForPositions();
            message.SetField(new PosReqID($"RP_{timeNow.TickOfDay}"));
            message.SetField(new PosReqType(reqType));
            message.SetField(new Account(account.Value));
            message.SetField(new AccountType(AccountType.ACCOUNT_IS_CARRIED_ON_NON_CUSTOMER_SIDE_OF_BOOKS_AND_IS_CROSS_MARGINED));
            message.SetField(new TradingSessionID(Broker));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));
            message.SetField(subscribe is true
                ? new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES)
                : new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT));

            return message;
        }
    }
}
