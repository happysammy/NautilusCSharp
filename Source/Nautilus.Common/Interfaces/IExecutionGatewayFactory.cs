//--------------------------------------------------------------------------------------------------
// <copyright file="IExecutionGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides an abstract factory for creating execution gateways.
    /// </summary>
    public interface IExecutionGatewayFactory
    {
        /// <summary>
        /// Creates a new <see cref="IExecutionGateway"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The fix client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <returns>The execution gateway.</returns>
        IExecutionGateway Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient fixClient,
            IInstrumentRepository instrumentRepository);
    }
}
