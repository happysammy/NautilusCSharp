//--------------------------------------------------------------------------------------------------
// <copyright file="RequestForPositionsFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm.MessageFactories
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;
    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides <see cref="RequestForPositions"/> FIX messages.
    /// </summary>
    public static class RequestForPositionsFactory
    {
        private const string Broker = "FXCM";

        /// <summary>
        /// Creates and returns a new <see cref="RequestForPositions"/> FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="account">The account for the request.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static RequestForPositions OpenAll(ZonedDateTime timeNow, AccountNumber account, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            return Create(timeNow, 0, account, subscribe); // 0 = Open positions
        }

        /// <summary>
        /// Creates and returns a new <see cref="RequestForPositions"/> FIX message.
        /// </summary>
        /// <param name="timeNow">The time now.</param>
        /// <param name="account">The account for the request.</param>
        /// <param name="subscribe">The flag indicating whether updates should be subscribed to.</param>
        /// <returns>The FIX message.</returns>
        public static RequestForPositions ClosedAll(ZonedDateTime timeNow, AccountNumber account, bool subscribe = true)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            return Create(timeNow, 1, account, subscribe); // 1 = Closed positions
        }

        private static RequestForPositions Create(ZonedDateTime timeNow, int reqType, AccountNumber account,  bool subscribe)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new RequestForPositions();
            message.SetField(new PosReqID($"RP_{timeNow.TickOfDay}"));
            message.SetField(new PosReqType(reqType));
            message.SetField(new Account(account.Value));
            message.SetField(new AccountType(AccountType.ACCOUNT_IS_CARRIED_ON_NON_CUSTOMER_SIDE_OF_BOOKS_AND_IS_CROSS_MARGINED));
            message.SetField(new TradingSessionID(Broker));
            message.SetField(new TransactTime(timeNow.ToDateTimeUtc()));
            message.SetField(subscribe is true
                ? new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES)
                : new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT));

            return message;
        }
    }
}
