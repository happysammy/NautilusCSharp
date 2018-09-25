//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
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
        /// <param name="configFilePath">The FIX config file path.</param>
        /// <param name="credentials">The FIX credentials.</param>
        /// <returns>The FXCM FIX client.</returns>
        public static IFixClient Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            string configFilePath,
            FixCredentials credentials)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(configFilePath, nameof(configFilePath));
            Validate.NotNull(credentials, nameof(credentials));

            return new FixClient(
                container,
                new FxcmFixMessageHandler(container),
                new FxcmFixMessageRouter(container, credentials.Account),
                configFilePath,
                credentials,
                Broker.FXCM,
                FxcmSymbolProvider.GetAllBrokerSymbols(),
                FxcmSymbolProvider.GetAllSymbols());
        }
    }
}
