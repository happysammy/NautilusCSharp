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
    using Newtonsoft.Json.Linq;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Enums;
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Entities;
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
        /// <param name="collectionConfig">The collection configuration.</param>
        /// <param name="barRepository">The database market data repo.</param>
        /// <param name="economicEventRepository">The database economic news event repo.</param>
        /// <param name="barDataProvider">The market data provider.</param>
        /// <returns>A built Nautilus database.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static Database Create(
            ILoggingAdapter logger,
            JObject collectionConfig,
            IBarRepository barRepository,
            IEconomicEventRepository<EconomicEvent> economicEventRepository,
            IBarDataProvider barDataProvider)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(collectionConfig, nameof(collectionConfig));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(economicEventRepository, nameof(economicEventRepository));
            Validate.NotNull(barDataProvider, nameof(barDataProvider));

            logger.Information(ServiceContext.Database, $"Starting {nameof(Database)} builder...");
            StartupVersionChecker.Run(logger);

            var clock = new Clock(DateTimeZone.Utc);
            var collectionSchedule = DataCollectionScheduleFactory.Create(clock.TimeNow(), collectionConfig);

            var setupContainer = new DatabaseSetupContainer(
                clock,
                new GuidFactory(),
                new LoggerFactory(logger));

            var actorSystem = ActorSystem.Create(nameof(Database));

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                setupContainer);

            var schedulerRef = actorSystem.ActorOf(Props.Create(
                () => new Scheduler()));

            var databaseTaskActorRef = actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    setupContainer,
                    barRepository,
                    economicEventRepository)));

            var dataCollectionActorRef = actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    setupContainer,
                    messagingAdapter,
                    databaseTaskActorRef,
                    collectionSchedule,
                    barDataProvider)));

            var barAggregationController = actorSystem.ActorOf(Props.Create(
                () => new BarAggregationController(
                    setupContainer,
                    messagingAdapter,
                    dataCollectionActorRef,
                    schedulerRef,
                    ServiceContext.Database)));

            var addresses = new Dictionary<Enum, IActorRef>
            {
                { DatabaseService.DatabaseTaskManager, databaseTaskActorRef },
                { DatabaseService.DatabaseCollectionManager, dataCollectionActorRef },
            };

            return new Database(
                setupContainer,
                actorSystem,
                messagingAdapter,
                addresses);
        }
    }
}
