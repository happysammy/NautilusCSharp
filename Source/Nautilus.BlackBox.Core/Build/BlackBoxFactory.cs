//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Build
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Build;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Provides a factory for creating <see cref="BlackBox"/> instances.
    /// </summary>
    public static class BlackBoxFactory
    {
        /// <summary>
        /// Creates and returns and new black box instance.
        /// </summary>
        /// <param name="environment">The black box environment.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="loggingAdapter">The logging adapter.</param>
        /// <param name="fixClient">The fix client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="quoteProvider">The quote provider.</param>
        /// <param name="riskModel">The risk model.</param>
        /// <param name="account">The account.</param>
        /// <param name="servicesFactory">The services factory.</param>
        /// <returns>The black box instance.</returns>
        public static BlackBox Create(
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggingAdapter loggingAdapter,
            IFixClient fixClient,
            IInstrumentRepository instrumentRepository,
            IQuoteProvider quoteProvider,
            RiskModel riskModel,
            Account account,
            BlackBoxServicesFactory servicesFactory)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(loggingAdapter, nameof(loggingAdapter));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));
            Validate.NotNull(quoteProvider, nameof(quoteProvider));
            Validate.NotNull(riskModel, nameof(riskModel));
            Validate.NotNull(account, nameof(account));
            Validate.NotNull(servicesFactory, nameof(servicesFactory));

            BuildVersionChecker.Run(loggingAdapter, "NautilusBlackBox - Automated Algorithmic Trading Platform");

            var loggerFactory = new LoggerFactory(loggingAdapter);
            var guidFactory = new GuidFactory();

            var container = new BlackBoxContainer(
                environment,
                clock,
                guidFactory,
                loggerFactory,
                instrumentRepository,
                quoteProvider,
                riskModel,
                account);

            var actorSystem = ActorSystem.Create("NautilusActorSystem");

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                container,
                new InMemoryMessageStore());

            var alphaModelService = servicesFactory.AlphaModelService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var dataService = servicesFactory.DataService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var executionService = servicesFactory.ExecutionService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var portfolioService = servicesFactory.PortfolioService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var riskService = servicesFactory.RiskService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var addresses = new Dictionary<NautilusService, IEndpoint>
            {
                { NautilusService.AlphaModel, alphaModelService },
                { NautilusService.Data, dataService },
                { NautilusService.Execution, executionService },
                { NautilusService.Portfolio, portfolioService },
                { NautilusService.Risk, riskService },
            };

            return new BlackBox(
                actorSystem,
                container,
                messagingAdapter,
                new Switchboard(addresses),
                null,
                fixClient,
                account,
                riskModel);
        }
    }
}
