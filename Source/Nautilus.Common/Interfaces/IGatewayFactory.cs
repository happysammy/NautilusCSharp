//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Provides <see cref="ITradeGateway"/>(s) for the system.
    /// </summary>
    public interface IGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ITradeGateway"/> from the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tradeClient">The trade client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <returns>A <see cref="ITradeGateway"/>.</returns>
        ITradeGateway Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITradeClient tradeClient,
            IInstrumentRepository instrumentRepository,
            CurrencyCode accountCurrency);
    }
}
