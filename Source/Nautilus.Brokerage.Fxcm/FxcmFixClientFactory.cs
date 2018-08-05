//--------------------------------------------------------------------------------------------------
// <copyright file="FixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;

    /// <summary>
    /// Provides a factory for creating FIX clients.
    /// </summary>
    public static class FxcmFixClientFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixClientFactory"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="username">The FIX account username.</param>
        /// <param name="password">The FIX account password.</param>
        /// <param name="accountNumber">The FIX account number.</param>

        public static IFixClient Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            string username,
            string password,
            string accountNumber)
        {
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(password, nameof(password));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            var credentials = new FixCredentials(
                username,
                password,
                accountNumber);

            return new FixClient(
                container,
                new FxcmFixMessageHandler(container),
                new FxcmFixMessageRouter(container, credentials.AccountNumber),
                credentials,
                Broker.FXCM,
                FxcmSymbolProvider.GetAllBrokerSymbols(),
                FxcmSymbolProvider.GetAllSymbols());
        }
    }
}
