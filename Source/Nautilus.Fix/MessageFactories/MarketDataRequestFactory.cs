//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Correctness;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides market data request FIX messages.
    /// </summary>
    public static class MarketDataRequestFactory
    {
        /// <summary>
        /// Creates and returns a new market data request FIX message.
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
                new MDReqID($"MD_{timeNow.TickOfDay}"),
                new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES),
                new MarketDepth(marketDepth));
            message.Set(new MDUpdateType(0));
            message.AddGroup(marketDataEntryGroup1);
            message.AddGroup(marketDataEntryGroup2);
            message.Set(new NoRelatedSym(1));
            message.AddGroup(symbolGroup);

            return message;
        }
    }
}
