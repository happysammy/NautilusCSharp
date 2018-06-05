//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxServicesFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Build
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// A container to store and transport the factories which are used to instantiate the services
    /// of a instance.
    /// </summary>
    [Immutable]
    public sealed class BlackBoxServicesFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBoxServicesFactory"/> class.
        /// </summary>
        /// <param name="brokerageGatewayFactory">The brokerage service factory.</param>
        /// <param name="brokerageClientFactory">The brokerage client factory.</param>
        /// <param name="alphaModelServiceFactory">The alpha model service factory.</param>
        /// <param name="dataServiceFactory">The data service factory.</param>
        /// <param name="executionServiceFactory">The execution service factory.</param>
        /// <param name="portfolioServiceFactory">The portfolio service factory.</param>
        /// <param name="riskServiceFactory">The risk service factory.</param>
        public BlackBoxServicesFactory(
            IBrokerageGatewayFactory brokerageGatewayFactory,
            IBrokerageClientFactory brokerageClientFactory,
            IServiceFactory alphaModelServiceFactory,
            IServiceFactory dataServiceFactory,
            IServiceFactory executionServiceFactory,
            IServiceFactory portfolioServiceFactory,
            IServiceFactory riskServiceFactory)
        {
            Validate.NotNull(brokerageGatewayFactory, nameof(brokerageGatewayFactory));
            Validate.NotNull(brokerageClientFactory, nameof(brokerageClientFactory));
            Validate.NotNull(alphaModelServiceFactory, nameof(alphaModelServiceFactory));
            Validate.NotNull(dataServiceFactory, nameof(dataServiceFactory));
            Validate.NotNull(executionServiceFactory, nameof(executionServiceFactory));
            Validate.NotNull(portfolioServiceFactory, nameof(portfolioServiceFactory));
            Validate.NotNull(riskServiceFactory, nameof(riskServiceFactory));

            this.BrokerageGateway = brokerageGatewayFactory;
            this.BrokerageClient = brokerageClientFactory;
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
        /// Gets the black box brokerage service factory.
        /// </summary>
        public IBrokerageClientFactory BrokerageClient { get; }

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
