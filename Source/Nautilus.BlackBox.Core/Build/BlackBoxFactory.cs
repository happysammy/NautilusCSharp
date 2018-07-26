//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Build
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Interfaces;

    public static class BlackBoxFactory
    {
        public static BlackBox Create(
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggingAdapter loggingAdapter,
            IDatabaseAdapter databaseAdapter,
            IInstrumentRepository instrumentRepository,
            IQuoteProvider quoteProvider,
            IRiskModel riskModel,
            IBrokerageAccount account,
            BlackBoxServicesFactory servicesFactory)
        {
            BuildVersionChecker.Run(loggingAdapter);

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

            var alphaModelServiceRef = servicesFactory.AlphaModelService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var dataServiceRef = servicesFactory.DataService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var executionServiceRef = servicesFactory.ExecutionService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var portfolioServiceRef = servicesFactory.PortfolioService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var riskServiceRef = servicesFactory.RiskService.Create(
                actorSystem,
                container,
                messagingAdapter);

            var brokerageClient =
                servicesFactory.FixClient.Create(container, messagingAdapter, null);

            var tradeGateway = new FixGateway(
                container,
                messagingAdapter,
                brokerageClient,
                instrumentRepository,
                account.Currency);

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { NautilusService.AlphaModel, alphaModelServiceRef },
                { NautilusService.Data, dataServiceRef },
                { NautilusService.Execution, executionServiceRef },
                { NautilusService.Portfolio , portfolioServiceRef },
                { NautilusService.Risk, riskServiceRef }
            };

            return new BlackBox(
                actorSystem,
                container,
                messagingAdapter,
                new Switchboard(addresses),
                tradeGateway,
                brokerageClient,
                account,
                riskModel);
        }
    }
}
