//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabaseFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Build
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Grpc.Core;
    using Nautilus.BlackBox.Brokerage;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Enums;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Processors;
    using Nautilus.Database.Protobuf;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Scheduler;
    using ServiceStack.Validation;

    /// <summary>
    /// The builder for the NautilusDB database infrastructure.
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// Builds the NautilusDB database infrastructure and returns an <see cref="IActorRef"/>
        /// address to the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        /// <param name="logger">The database logger.</param>
        /// <param name="fixClientFactory">The FIX client factory.</param>
        /// <param name="barRepository">The database market data repo.</param>
        /// <returns>A built Nautilus database.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static Database Create(
            ILoggingAdapter logger,
            IFixClientFactory fixClientFactory,
            IBarRepository barRepository)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(barRepository, nameof(barRepository));

            logger.Information(ServiceContext.Database, $"Starting {nameof(Database)} builder...");
            StartupVersionChecker.Run(logger);

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();

            var setupContainer = new DatabaseSetupContainer(
                clock,
                guidFactory,
                new LoggerFactory(logger));

            var actorSystem = ActorSystem.Create(nameof(Nautilus.Database));

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                setupContainer);

            var schedulerRef = actorSystem.ActorOf(Props.Create(
                () => new Scheduler()));

            var databaseTaskActorRef = actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    setupContainer,
                    barRepository)));

            var dataCollectionActorRef = actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    setupContainer,
                    messagingAdapter)));

            var barAggregationControllerRef = actorSystem.ActorOf(Props.Create(
                () => new BarAggregationController(
                    setupContainer,
                    messagingAdapter)));

            var quoteProvider = new QuoteProvider(Exchange.FXCM);  // TODO: Hardcoded quote provider.

            var tickDataProcessor = new TickDataProcessor(
                setupContainer,
                quoteProvider,
                barAggregationControllerRef);

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { DatabaseService.Scheduler, schedulerRef },
                { DatabaseService.TaskManager, databaseTaskActorRef },
                { DatabaseService.CollectionManager, dataCollectionActorRef },
            };

            var fixClient = fixClientFactory.DataClient(
                setupContainer,
                messagingAdapter,
                tickDataProcessor);

            var subscriptionImpl = new DataSubscriptionServer(
                clock,
                guidFactory,
                dataCollectionActorRef);

            var subscriptionServer = new Server
            {
                Services = {DataServer.BindService(subscriptionImpl)},
                Ports = {new ServerPort("localhost", 500051, ServerCredentials.Insecure)}
            };

            return new Database(
                setupContainer,
                actorSystem,
                messagingAdapter,
                addresses,
                fixClient,
                subscriptionServer);
        }
    }
}
