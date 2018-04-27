// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TradingSessionStatusRequestFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using NodaTime;

    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// The trading session status request.
    /// </summary>
    public static class TradingSessionStatusRequestFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="timeNow">
        /// The time now.
        /// </param>
        /// <returns>
        /// The <see cref="TradingSessionStatusRequest"/>.
        /// </returns>
        public static TradingSessionStatusRequest Create(ZonedDateTime timeNow)
        {
            var message = new TradingSessionStatusRequest();

            message.SetField(new TradSesReqID($"TSS_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID("FXCM"));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
