//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="BrokerageGatewayFactory"/> class. Creates an
    /// <see cref="IBrokerageGateway"/> for the <see cref="BlackBox"/> system.
    /// </summary>
    [Immutable]
    public sealed class BrokerageGatewayFactory : IBrokerageGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IBrokerageGateway"/> based on the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tradeClient">The broker client.</param>
        /// <returns>A <see cref="IBrokerageGateway"/>.</returns>
        public IBrokerageGateway Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            ITradeClient tradeClient)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tradeClient, nameof(tradeClient));

            return new BrokerageGateway(
                container,
                messagingAdapter,
                tradeClient);
        }
    }
}
