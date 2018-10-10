//--------------------------------------------------------------------------------------------------
// <copyright file="RequestForOpenPositionsFactory.cs" company="Nautech Systems Pty Ltd">
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
    /// Provides request for open position FIX messages.
    /// </summary>
    public static class RequestForOpenPositionsFactory
    {
        /// <summary>
        /// Creates and returns a new request for open positions message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <returns>The FIX message.</returns>
        public static RequestForPositions Create(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new RequestForPositions();
            var transactTime = timeNow.ToDateTimeUtc();

            message.SetField(new PosReqID($"PR_{timeNow.TickOfDay}"));
            message.SetField(new PosReqType(PosReqType.POSITIONS));
            message.SetField(new AccountType(AccountType.ACCOUNT_IS_CARRIED_ON_CUSTOMER_SIDE_OF_BOOKS));
            message.SetField(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));
            message.SetField(new TransactTime(transactTime));

            return message;
        }
    }
}
