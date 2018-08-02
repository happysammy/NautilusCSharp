//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiryFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using System;
    using Nautilus.Core.Validation;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides collateral enquiry messages from the given inputs.
    /// </summary>
    public static class CollateralInquiryFactory
    {
        /// <summary>
        /// Creates and returns a new collateral inquiry FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="tradingSessionId">The brokers name.</param>
        /// <returns>A <see cref="CollateralInquiry"/>.</returns>
        public static CollateralInquiry Create(ZonedDateTime timeNow, Enum tradingSessionId)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new CollateralInquiry();

            message.SetField(new CollInquiryID($"CI_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID(tradingSessionId.ToString()));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
