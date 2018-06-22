//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiryFactory.cs" company="Nautech Systems Pty Ltd">
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
