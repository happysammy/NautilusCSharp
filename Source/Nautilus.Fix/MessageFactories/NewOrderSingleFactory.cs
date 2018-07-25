//--------------------------------------------------------------------------------------------------
// <copyright file="NewOrderSingleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix.MessageFactories
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Interfaces;
    using NodaTime;
    using QuickFix.FIX44;

    /// <summary>
    /// Provides new order single FIX messages.
    /// </summary>
    public static class NewOrderSingleFactory
    {
        /// <summary>
        /// Creates and returns a new new order single message.
        /// </summary>
        /// <returns>A <see cref="NewOrderSingle"/> message.</returns>
        public static QuickFix.FIX44.NewOrderSingle Create(
            string brokerSymbol,
            string accountNumber,
            IOrder order,
            ZonedDateTime timeNow)
        {
            Debug.NotNull(brokerSymbol, nameof(brokerSymbol));
            Debug.NotNull(accountNumber, nameof(accountNumber));
            Debug.NotNull(order, nameof(order));
            Debug.NotDefault(timeNow, nameof(timeNow));

            var message = new QuickFix.FIX44.NewOrderSingle();

            // TODO

            return message;
        }
    }
}
