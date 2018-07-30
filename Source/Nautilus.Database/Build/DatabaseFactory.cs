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
    using Nautilus.Common;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Aggregators;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Processors;
    using Nautilus.Database.Publishers;
    using Nautilus.DomainModel.Enums;
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
        /// <param name="publisherFactory">The channel publisher factory.</param>
        /// <param name="barRepository">The database market data repo.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="symbols">The symbols to initially subscribe to.</param>
        /// <param name="barSpecs">The bar specifications to initially create.</param>
        /// <param name="resolutions">The bar resolutions to persist (with a period of 1).</param>
        /// <param name="barRollingWindow">The rolling window size of bar data to be maintained.</param>
        /// <returns>A built Nautilus database.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static Database Create(
            ILoggingAdapter logger,
            IFixClientFactory fixClientFactory,
            IChannelPublisherFactory publisherFactory,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository,
            IReadOnlyList<string> symbols,
            IReadOnlyList<string> barSpecs,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(fixClientFactory, nameof(fixClientFactory));
            Validate.NotNull(publisherFactory, nameof(publisherFactory));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));
            Validate.NotNull(symbols, nameof(symbols));
            Validate.NotNull(barSpecs, nameof(barSpecs));

            logger.Information(NautilusService.Data, $"Starting {nameof(Database)} builder...");
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
                setupContainer,
                new FakeMessageStore());

            var schedulerRef = actorSystem.ActorOf(Props.Create(
                () => new Scheduler(setupContainer)));

            var tickPublisherRef = actorSystem.ActorOf(Props.Create(
                () => new TickPublisher(setupContainer, publisherFactory.Create())));

            var barPublisherRef = actorSystem.ActorOf(Props.Create(
                () => new BarPublisher(setupContainer, publisherFactory.Create())));

            var databaseTaskActorRef = actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    setupContainer,
                    barRepository)));

            var dataCollectionActorRef = actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    setupContainer,
                    messagingAdapter,
                    barPublisherRef,
                    resolutions,
                    barRollingWindow)));

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
                { NautilusService.Scheduler, schedulerRef },
                { NautilusService.DatabaseTaskManager, databaseTaskActorRef },
                { NautilusService.DataCollectionManager, dataCollectionActorRef },
                { NautilusService.BarAggregationController, barAggregationControllerRef },
                { NautilusService.TickPublisher, tickPublisherRef },
                { NautilusService.BarPublisher, barPublisherRef }
            };

            var fixClient = fixClientFactory.Create(
                setupContainer,
                messagingAdapter,
                tickDataProcessor);

            var gateway = new ExecutionGateway(
                setupContainer,
                messagingAdapter,
                fixClient,
                instrumentRepository,
                CurrencyCode.GBP);

            fixClient.InitializeGateway(gateway);
            instrumentRepository.CacheAll();

            return new Database(
                setupContainer,
                actorSystem,
                messagingAdapter,
                addresses,
                fixClient);
        }
    }
}
