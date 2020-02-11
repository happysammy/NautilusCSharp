//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiryFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    /// Provides <see cref="CollateralInquiry"/> FIX messages.
    /// </summary>
    public static class CollateralInquiryFactory
    {
        private const string Broker = "FXCM";

        /// <summary>
        /// Creates and returns a new <see cref="CollateralInquiry"/> FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static CollateralInquiry Create(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new CollateralInquiry();

            message.SetField(new CollInquiryID($"CI_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID(Broker));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
