//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The <see cref="IBrokerageGatewayFactory"/> interface. Provides
    /// <see cref="IBrokerageGateway"/>(s) for the <see cref="BlackBox"/> system from the given inputs.
    /// </summary>
    public interface IBrokerageGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IBrokerageGateway"/> from the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tradeClient">The brokerage client.</param>
        /// <returns>A <see cref="IBrokerageGateway"/>.</returns>
        IBrokerageGateway Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            ITradeClient tradeClient);
    }
}
