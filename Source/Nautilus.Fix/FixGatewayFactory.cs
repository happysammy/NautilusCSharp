//--------------------------------------------------------------------------------------------------
// <copyright file="FixGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides a factory for FIX gateways.
    /// </summary>
    [Stateless]
    public static class FixGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new execution gateway.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <returns>The FIX gateway.</returns>
        public static IFixGateway Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IInstrumentRepository instrumentRepository,
            IFixClient fixClient)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(fixClient, nameof(fixClient));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));

            var gateway = new FixGateway(
                container,
                messagingAdapter,
                instrumentRepository,
                fixClient);

            fixClient.InitializeGateway(gateway);

            return gateway;
        }
    }
}
