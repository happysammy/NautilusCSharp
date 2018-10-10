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
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Aggregators;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Address = Nautilus.Common.Messaging.Address;

    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Builds the database and returns an <see cref="Akka.Actor.IActorRef"/> address to the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="client">The FIX client.</param>
        /// <param name="gateway">The FIX gateway.</param>
        /// <param name="publisherFactory">The channel publisher factory.</param>
        /// <param name="barRepository">The bar repository.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="symbols">The symbols to initially subscribe to.</param>
        /// <param name="resolutions">The bar resolutions to persist (with a period of 1).</param>
        /// <param name="barRollingWindow">The rolling window size of bar data to be maintained.</param>
        /// <param name="updateInstruments">The option flag to update instruments.</param>
        /// <returns>The endpoint addresses for the data service.</returns>
        public static Dictionary<Address, IEndpoint> Create(
            ActorSystem actorSystem,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient client,
            IFixGateway gateway,
            IChannelPublisherFactory publisherFactory,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository,
            IReadOnlyList<string> symbols,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow,
            bool updateInstruments)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(publisherFactory, nameof(publisherFactory));
            Validate.NotNull(barRepository, nameof(barRepository));
            Validate.NotNull(symbols, nameof(symbols));

            var tickPublisher = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new TickPublisher(container, publisherFactory.Create()))));

            var barPublisher = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new BarPublisher(container, publisherFactory.Create()))));

            var databaseTaskActor = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    container,
                    barRepository,
                    instrumentRepository))));

            var dataCollectionActor = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    container,
                    messagingAdapter,
                    barPublisher,
                    resolutions,
                    barRollingWindow))));

            var barAggregationController = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new BarAggregationController(
                    container,
                    messagingAdapter))));

            var dataService = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new DataService(
                        container,
                        messagingAdapter,
                        gateway,
                        updateInstruments))));

            gateway.RegisterTickReceiver(tickPublisher);
            gateway.RegisterTickReceiver(barAggregationController);
            gateway.RegisterInstrumentReceiver(DataServiceAddress.DatabaseTaskManager);
            client.RegisterConnectionEventReceiver(dataService);

            return new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Data, dataService },
                { DataServiceAddress.DatabaseTaskManager, databaseTaskActor },
                { DataServiceAddress.DataCollectionManager, dataCollectionActor },
                { DataServiceAddress.BarAggregationController, barAggregationController },
                { DataServiceAddress.TickPublisher, tickPublisher },
                { DataServiceAddress.BarPublisher, barPublisher },
            };
        }
    }
}
