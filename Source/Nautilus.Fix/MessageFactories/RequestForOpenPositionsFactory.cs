//---------------------------------------------------------------------------------
// <copyright file="RequestForOpenPositionsFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using NodaTime;

    using QuickFix.Fields;
    using QuickFix.FIX44;

    /// <summary>
    /// The request for open positions.
    /// </summary>
    public static class RequestForOpenPositionsFactory
    {
        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="timeNow">
        /// The transaction time.
        /// </param>
        /// <returns>
        /// The <see cref="RequestForPositions"/>.
        /// </returns>
        public static RequestForPositions Create(ZonedDateTime timeNow)
        {
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
