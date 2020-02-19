//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm
{
    using System.Collections.Immutable;
    using Nautilus.Common.Interfaces;
    using Nautilus.Fix;

    /// <summary>
    /// Provides a factory for creating FXCM FIX clients.
    /// </summary>
    public static class FxcmFixClientFactory
    {
        /// <summary>
        /// Creates and returns a new FXCM FIX client.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="config">The FIX configuration.</param>
        /// <param name="symbolMap">The symbol provider.</param>
        /// <returns>The FXCM FIX client.</returns>
        public static IFixClient Create(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            FixConfiguration config,
            ImmutableDictionary<string, string> symbolMap)
        {
            return new FixClient(
                container,
                messageBusAdapter,
                config,
                new FxcmFixMessageHandler(
                    container,
                    config.AccountId,
                    config.AccountCurrency,
                    symbolMap),
                new FxcmFixMessageRouter(
                    container,
                    config.AccountId,
                    symbolMap));
        }
    }
}
