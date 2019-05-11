//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Aggregators;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Builds the database and returns an address book endpoints.
        /// </summary>
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
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient client,
            IFixGateway gateway,
            IChannelPublisherFactory publisherFactory,
            IBarRepository barRepository,
            IInstrumentRepository instrumentRepository,
            IReadOnlyCollection<string> symbols,
            IReadOnlyCollection<Resolution> resolutions,
            int barRollingWindow,
            bool updateInstruments)
        {
            var tickPublisher = new TickPublisher(container, publisherFactory.Create());
            var barPublisher = new BarPublisher(container, publisherFactory.Create());

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                barRepository,
                instrumentRepository);

            var dataCollectionManager = new DataCollectionManager(
                container,
                messagingAdapter,
                barPublisher.Endpoint,
                resolutions,
                barRollingWindow);

            var barAggregationController = new BarAggregationController(container, messagingAdapter);

            var dataService = new DataService(
                container,
                messagingAdapter,
                gateway,
                updateInstruments);

            gateway.RegisterTickReceiver(tickPublisher.Endpoint);
            gateway.RegisterTickReceiver(barAggregationController.Endpoint);
            gateway.RegisterInstrumentReceiver(DataServiceAddress.DatabaseTaskManager);
            client.RegisterConnectionEventReceiver(dataService.Endpoint);

            return new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Data, dataService.Endpoint },
                { DataServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { DataServiceAddress.DataCollectionManager, dataCollectionManager.Endpoint },
                { DataServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { DataServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { DataServiceAddress.BarPublisher, barPublisher.Endpoint },
            };
        }
    }
}
