//--------------------------------------------------------------
// <copyright file="IBrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.BlackBox.Core.Setup;
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
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="brokerageClient">The brokerage client.</param>
        /// <returns>A <see cref="IBrokerageGateway"/>.</returns>
        IBrokerageGateway Create(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IBrokerageClient brokerageClient);
    }
}
