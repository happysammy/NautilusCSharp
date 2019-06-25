//--------------------------------------------------------------------------------------------------
// <copyright file="FixTradingGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// Provides a factory for FIX gateways.
    /// </summary>
    public static class FixTradingGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new FIX gateway.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <returns>The created FIX gateway.</returns>
        public static FixTradingGateway Create(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IFixClient fixClient)
        {
            var gateway = new FixTradingGateway(
                container,
                messageBusAdapter,
                fixClient);

            fixClient.InitializeGateway(gateway);

            return gateway;
        }
    }
}
