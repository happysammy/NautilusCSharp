//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityListRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    /// Provides security list request FIX messages.
    /// </summary>
    public static class SecurityListRequestFactory
    {
        /// <summary>
        /// Creates and returns a new security list request message for all symbols.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static SecurityListRequest Create(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new SecurityListRequest();

            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.ALL_SECURITIES));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }

        /// <summary>
        /// Creates and returns a new security list request message for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static SecurityListRequest Create(string symbol, ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new SecurityListRequest();

            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.SYMBOL));
            message.SetField(new Symbol(symbol));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
