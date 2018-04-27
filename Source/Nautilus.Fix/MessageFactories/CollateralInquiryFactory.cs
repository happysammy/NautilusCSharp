// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiryFactory.cs" company="Nautech Systems Pty Ltd.">
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
    /// The collateral inquiry.
    /// </summary>
    public static class CollateralInquiryFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="timeNow">
        /// The time now.
        /// </param>
        /// <returns>
        /// The <see cref="CollateralInquiry"/>.
        /// </returns>
        public static CollateralInquiry Create(ZonedDateTime timeNow)
        {
            var message = new CollateralInquiry();

            message.SetField(new CollInquiryID($"CI_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID("FXCM"));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
