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
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Scheduler;

    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Builds the database and returns an <see cref="IActorRef"/> address to the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="gateway">The execution gateway.</param>
        /// <param name="publisherFactory">The channel publisher factory.</param>
        /// <param name="barRepository">The database market data repo.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="symbols">The symbols to initially subscribe to.</param>
        /// <param name="resolutions">The bar resolutions to persist (with a period of 1).</param>
        /// <param name="barRollingWindow">The rolling window size of bar data to be maintained.</param>
        /// <returns>The endpoint addresses for the data service.</returns>
        public static Dictionary<NautilusService, IEndpoint> Create(
            ActorSystem actorSystem,
            IComponentryContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IExecutionGateway gateway,
            IChannelPublisherFactory publisherFactory,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository,
            IReadOnlyList<string> symbols,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
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

            gateway.RegisterTickReceiver(tickPublisher);
            gateway.RegisterTickReceiver(barAggregationController);
            instrumentRepository.CacheAll();

            var dataService = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new DataService(
                        setupContainer,
                        messagingAdapter))));

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
