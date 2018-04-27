// -------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxServicesFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Setup
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="BlackBoxServicesFactory"/> class. A container to store and
    /// transport the factories which are used to instantiate the services of a 
    /// <see cref="BlackBox"/> instance.
    /// </summary>
    [Immutable]
    public sealed class BlackBoxServicesFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBoxServicesFactory"/> class.
        /// </summary>
        /// <param name="brokerageGatewayFactory">The brokerage service factory.</param>
        /// <param name="messagingServiceFactory">The messaging service factory.</param>
        /// <param name="switchboardFactory">The message switchboard factory.</param>
        /// <param name="alphaModelServiceFactory">The alpha model service factory.</param>
        /// <param name="dataServiceFactory">The data service factory.</param>
        /// <param name="executionServiceFactory">The execution service factory.</param>
        /// <param name="portfolioServiceFactory">The portfolio service factory.</param>
        /// <param name="riskServiceFactory">The risk service factory.</param>
        public BlackBoxServicesFactory(
            IBrokerageGatewayFactory brokerageGatewayFactory,
            IMessagingServiceFactory messagingServiceFactory,
            ISwitchboardFactory switchboardFactory,
            IServiceFactory alphaModelServiceFactory,
            IServiceFactory dataServiceFactory,
            IServiceFactory executionServiceFactory,
            IServiceFactory portfolioServiceFactory,
            IServiceFactory riskServiceFactory)
        {
            Validate.NotNull(brokerageGatewayFactory, nameof(brokerageGatewayFactory));
            Validate.NotNull(messagingServiceFactory, nameof(messagingServiceFactory));
            Validate.NotNull(switchboardFactory, nameof(switchboardFactory));
            Validate.NotNull(alphaModelServiceFactory, nameof(alphaModelServiceFactory));
            Validate.NotNull(dataServiceFactory, nameof(dataServiceFactory));
            Validate.NotNull(executionServiceFactory, nameof(executionServiceFactory));
            Validate.NotNull(portfolioServiceFactory, nameof(portfolioServiceFactory));
            Validate.NotNull(riskServiceFactory, nameof(riskServiceFactory));

            this.BrokerageGateway = brokerageGatewayFactory;
            this.MessagingService = messagingServiceFactory;
            this.Switchboard = switchboardFactory;
            this.AlphaModelService = alphaModelServiceFactory;
            this.DataService = dataServiceFactory;
            this.ExecutionService = executionServiceFactory;
            this.PortfolioService = portfolioServiceFactory;
            this.RiskService = riskServiceFactory;
        }

        /// <summary>
        /// Gets the black box brokerage service factory.
        /// </summary>
        public IBrokerageGatewayFactory BrokerageGateway { get; }

        /// <summary>
        /// Gets the black box messaging service factory.
        /// </summary>
        public IMessagingServiceFactory MessagingService { get; }

        /// <summary>
        /// Gets the black box message switchboard factory.
        /// </summary>
        public ISwitchboardFactory Switchboard { get; }

        /// <summary>
        /// Gets the black box alpha model service factory.
        /// </summary>
        public IServiceFactory AlphaModelService { get; }

        /// <summary>
        /// Gets the black box data service factory.
        /// </summary>
        public IServiceFactory DataService { get; }

        /// <summary>
        /// Gets the black box execution service factory.
        /// </summary>
        public IServiceFactory ExecutionService { get; }

        /// <summary>
        /// Gets the black box portfolio service factory.
        /// </summary>
        public IServiceFactory PortfolioService { get; }

        /// <summary>
        /// Gets the black box risk service factory.
        /// </summary>
        public IServiceFactory RiskService { get; }
    }
}