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
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Creates a <see cref="ITradeGateway"/> for the system.
    /// </summary>
    [Immutable]
    public sealed class GatewayFactory : IGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ITradeGateway"/> based on the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tradeClient">The broker client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <returns>A <see cref="ITradeGateway"/>.</returns>
        public ITradeGateway Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITradeClient tradeClient,
            IInstrumentRepository instrumentRepository,
            CurrencyCode accountCurrency)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tradeClient, nameof(tradeClient));

            return new TradeGateway(
                container,
                messagingAdapter,
                tradeClient,
                instrumentRepository,
                accountCurrency);
        }
    }
}
