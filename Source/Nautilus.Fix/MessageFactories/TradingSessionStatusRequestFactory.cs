//--------------------------------------------------------------------------------------------------
// <copyright file="TradingSessionStatusRequestFactory.cs" company="Nautech Systems Pty Ltd">
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
    /// Provides trading session status request FIX messages.
    /// </summary>
    public static class TradingSessionStatusRequestFactory
    {
        /// <summary>
        /// Creates and returns a new trading session status request message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static TradingSessionStatusRequest Create(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new TradingSessionStatusRequest();

            message.SetField(new TradSesReqID($"TSS_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID("FXCM"));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
