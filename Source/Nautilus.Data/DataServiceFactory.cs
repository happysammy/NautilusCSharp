//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Aggregators;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Processors;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Scheduler;

    /// <summary>
    /// Provides a factory for building the database.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Builds the database and returns an <see cref="IActorRef"/> address to the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClientFactory">The FIX client factory.</param>
        /// <param name="gatewayFactory">The execution gateway factory.</param>
        /// <param name="publisherFactory">The channel publisher factory.</param>
        /// <param name="barRepository">The database market data repo.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="symbols">The symbols to initially subscribe to.</param>
        /// <param name="resolutions">The bar resolutions to persist (with a period of 1).</param>
        /// <param name="barRollingWindow">The rolling window size of bar data to be maintained.</param>
        /// <returns>The endpoint addresses for the data service.</returns>
        public static Dictionary<NautilusService, IEndpoint> Create(
            IComponentryContainer setupContainer,
            ActorSystem actorSystem,
            MessagingAdapter messagingAdapter,
            IFixClientFactory fixClientFactory,
            IExecutionGatewayFactory gatewayFactory,
            IChannelPublisherFactory publisherFactory,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository,
            IReadOnlyList<string> symbols,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(fixClientFactory, nameof(fixClientFactory));
            Validate.NotNull(publisherFactory, nameof(publisherFactory));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));
            Validate.NotNull(symbols, nameof(symbols));

            var scheduler = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new Scheduler(setupContainer))));

            var tickPublisher = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new TickPublisher(setupContainer, publisherFactory.Create()))));

            var barPublisher = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new BarPublisher(setupContainer, publisherFactory.Create()))));

            var databaseTaskActor = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    setupContainer,
                    barRepository))));

            var dataCollectionActor = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    setupContainer,
                    messagingAdapter,
                    barPublisher,
                    resolutions,
                    barRollingWindow))));

            var barAggregationController = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new BarAggregationController(
                    setupContainer,
                    messagingAdapter))));

            var tickDataProcessor = new TickProcessor(
                setupContainer,
                tickPublisher,
                barAggregationController);

            var fixClient = fixClientFactory.Create(
                setupContainer,
                messagingAdapter,
                tickDataProcessor);

            var gateway = gatewayFactory.Create(
                setupContainer,
                messagingAdapter,
                fixClient,
                instrumentRepository);

            fixClient.InitializeGateway(gateway);
            instrumentRepository.CacheAll();

            var dataService = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new DataService(
                        setupContainer,
                        messagingAdapter,
                        fixClient))));

            return new Dictionary<NautilusService, IEndpoint>
            {
                { NautilusService.Scheduler, scheduler },
                { NautilusService.DatabaseTaskManager, databaseTaskActor },
                { NautilusService.DataCollectionManager, dataCollectionActor },
                { NautilusService.BarAggregationController, barAggregationController },
                { NautilusService.TickPublisher, tickPublisher },
                { NautilusService.BarPublisher, barPublisher },
                { NautilusService.Data, dataService },
            };
        }
    }
}
