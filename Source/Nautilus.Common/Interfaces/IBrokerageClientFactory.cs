//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// Provides <see cref="IBrokerageClient"/>(s) for the <see cref="BlackBox"/> system from the
    /// given inputs.
    /// </summary>
    public interface IBrokerageClientFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IBrokerageGateway"/> from the given inputs.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <returns>A <see cref="IBrokerageGateway"/>.</returns>
        IBrokerageClient Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickDataProcessor tickDataProcessor);
    }
}
