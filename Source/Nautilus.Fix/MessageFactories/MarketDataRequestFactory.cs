//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataRequestSubscriptionFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Validation;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides market data request FIX messages.
    /// </summary>
    public static class MarketDataRequestFactory
    {
        /// <summary>
        /// Creates and returns a new market data request message.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>A <see cref="MarketDataRequest"/> message.</returns>
        public static MarketDataRequest Create(string symbol, ZonedDateTime timeNow)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var subType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES);
            var marketDepth = new MarketDepth(0);

            var marketDataEntryGroup = new MarketDataRequest.NoMDEntryTypesGroup();
            marketDataEntryGroup.Set(new MDEntryType(MDEntryType.BID));

            var symbolGroup = new MarketDataRequest.NoRelatedSymGroup();
            symbolGroup.Set(new Symbol(symbol));

            var message = new MarketDataRequest(new MDReqID($"MD_{timeNow.TickOfDay}"), subType, marketDepth);
            message.AddGroup(marketDataEntryGroup);
            message.AddGroup(symbolGroup);

            return message;
        }
    }
}
