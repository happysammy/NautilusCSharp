//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityListRequestFactory.cs" company="Nautech Systems Pty Ltd">
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
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides <see cref="SecurityListRequest"/> FIX messages.
    /// </summary>
    public static class SecurityListRequestFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="SecurityListRequest"/> FIX message for all symbols.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static SecurityListRequest Create(ZonedDateTime timeNow, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new SecurityListRequest();
            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.ALL_SECURITIES));
            message.SetField(subscribe is true
                ? new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES)
                : new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT));

            return message;
        }

        /// <summary>
        /// Creates and returns a new <see cref="SecurityListRequest"/> FIX message for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="timeNow">The time now.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static SecurityListRequest Create(string symbol, ZonedDateTime timeNow, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new SecurityListRequest();
            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.SYMBOL));
            message.SetField(new Symbol(symbol));
            message.SetField(subscribe is true
                ? new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES)
                : new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT));

            return message;
        }
    }
}
