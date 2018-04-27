// -------------------------------------------------------------------------------------------------
// <copyright file="BrokerageGatewayFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;

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
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="brokerageClient">The broker client.</param>
        /// <returns>A <see cref="IBrokerageGateway"/>.</returns>
        public IBrokerageGateway Create(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IBrokerageClient brokerageClient)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(brokerageClient, nameof(brokerageClient));

            return new BrokerageGateway(
                setupContainer,
                messagingAdapter,
                brokerageClient);
        }
    }
}
