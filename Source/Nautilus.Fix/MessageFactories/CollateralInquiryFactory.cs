//--------------------------------------------------------------------------------------------------
// <copyright file="CollateralInquiryFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
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
        /// <param name="broker">The brokers name.</param>
        /// <returns>The FIX message.</returns>
        public static CollateralInquiry Create(ZonedDateTime timeNow, Brokerage broker)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new CollateralInquiry();

            message.SetField(new CollInquiryID($"CI_{timeNow.TickOfDay}"));
            message.SetField(new TradingSessionID(broker.ToString()));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

            return message;
        }
    }
}
