//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityListRequestFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// The security list request.
    /// </summary>
    public static class SecurityListRequestFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="timeNow">
        /// The time now.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityListRequestFactory"/>.
        /// </returns>
        public static SecurityListRequest Create(ZonedDateTime timeNow)
        {
            var message = new SecurityListRequest();

            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.ALL_SECURITIES));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <param name="timeNow">
        /// The time now.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityListRequestFactory"/>.
        /// </returns>
        public static SecurityListRequest Create(string symbol, ZonedDateTime timeNow)
        {
            var message = new SecurityListRequest();

            message.SetField(new SecurityReqID($"SLR_{timeNow.TickOfDay}"));
            message.SetField(new SecurityListRequestType(SecurityListRequestType.SYMBOL));
            message.SetField(new Symbol(symbol));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
