//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataRequestFactory.cs" company="Nautech Systems Pty Ltd">
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
using NodaTime;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Nautilus.Fxcm.MessageFactories
{
    /// <summary>
    /// Provides <see cref="MarketDataRequest"/> FIX messages.
    /// </summary>
    public static class MarketDataRequestFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="MarketDataRequest"/> FIX message.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="marketDepth">The market depth.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static MarketDataRequest Create(string symbol, int marketDepth, ZonedDateTime timeNow)
        {
            Debug.NotEmptyOrWhiteSpace(symbol, nameof(symbol));
            Debug.NotNegativeInt32(marketDepth, nameof(marketDepth));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var marketDataEntryGroup1 = new MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup1.Set(new MDEntryType(MDEntryType.BID));

            var marketDataEntryGroup2 = new MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup2.Set(new MDEntryType(MDEntryType.OFFER));

            var symbolGroup = new MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.SetField(new Symbol(symbol));

            var message = new MarketDataRequest(
                new MDReqID($"MD_{timeNow.TickOfDay.ToString()}"),
                new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES),
                new MarketDepth(marketDepth));
            message.Set(new NoMDEntryTypes(2));
            message.Set(new MDUpdateType(1));  // val:1 to receive shortened message
            message.AddGroup(marketDataEntryGroup1);
            message.AddGroup(marketDataEntryGroup2);
            message.Set(new NoRelatedSym(1));
            message.AddGroup(symbolGroup);

            return message;
        }
    }
}
