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
    using Nautilus.Database.Publishers;
    using Nautilus.Scheduler;

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
                () => new Scheduler(setupContainer)));

            var tickPublisherRef = actorSystem.ActorOf(Props.Create(
                () => new TickPublisher(setupContainer)));

            var barPublisherRef = actorSystem.ActorOf(Props.Create(
                () => new BarPublisher(setupContainer)));

            var databaseTaskActorRef = actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    setupContainer,
                    barRepository)));

            var dataCollectionActorRef = actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    setupContainer,
                    messagingAdapter,
                    barPublisherRef)));

            var barAggregationControllerRef = actorSystem.ActorOf(Props.Create(
                () => new BarAggregationController(
                    setupContainer,
                    messagingAdapter)));

            var tickDataProcessor = new TickProcessor(
                setupContainer,
                tickPublisherRef,
                barAggregationControllerRef);

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { DatabaseService.Scheduler, schedulerRef },
                { DatabaseService.TaskManager, databaseTaskActorRef },
                { DatabaseService.CollectionManager, dataCollectionActorRef },
                { DatabaseService.BarAggregationController, barAggregationControllerRef},
                { DatabaseService.TickPublisher, tickPublisherRef},
                { DatabaseService.BarPublisher, barPublisherRef}
            };

            var fixClient = fixClientFactory.DataClient(
                setupContainer,
                messagingAdapter,
                tickDataProcessor);

            return new Database(
                setupContainer,
                actorSystem,
                messagingAdapter,
                addresses,
                fixClient);
        }
    }
}
