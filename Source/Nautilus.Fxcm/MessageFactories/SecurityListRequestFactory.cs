//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityListRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
