//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Brokerage.Dukascopy;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Common.Scheduling;
    using Nautilus.Core.Extensions;
    using Nautilus.Data;
    using Nautilus.Data.Aggregation;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Builds the database and returns an address book endpoints.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The endpoint addresses for the data service.</returns>
        public static DataService Create(Configuration config)
        {
            var loggingAdapter = new SerilogLogger(config.LogLevel);
            loggingAdapter.Debug(NautilusService.Core, $"Starting {nameof(NautilusData)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusData - Financial Market Data Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(container, new FakeMessageStore());
            var scheduler = new Scheduler(container);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var barRepository = new RedisBarRepository(redisConnection);
            var instrumentRepository = new RedisInstrumentRepository(redisConnection);
            instrumentRepository.CacheAll();

            var venue = config.FixConfiguration.Broker.ToString().ToEnum<Venue>();
            var instrumentData = new InstrumentDataProvider(venue, config.FixConfiguration.InstrumentDataFileName);

            var tickPublisher = new TickPublisher(container, NetworkAddress.LocalHost(), new NetworkPort(60000));
            var barPublisher = new BarPublisher(container, NetworkAddress.LocalHost(), new NetworkPort(60000));

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                barRepository,
                instrumentRepository);

            var dataCollectionManager = new DataCollectionManager(
                container,
                messagingAdapter,
                barPublisher.Endpoint,
                config.BarSpecifications,
                config.BarRollingWindowDays);

            var barAggregationController = new BarAggregationController(
                container,
                messagingAdapter,
                barPublisher.Endpoint);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                instrumentData);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduling, scheduler.Endpoint },
                { DataServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { DataServiceAddress.DataCollectionManager, dataCollectionManager.Endpoint },
                { DataServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { DataServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { DataServiceAddress.BarPublisher, barPublisher.Endpoint },
            }.ToImmutableDictionary();

            return new DataService(
                container,
                messagingAdapter,
                addresses,
                fixGateway,
                config.FixConfiguration.UpdateInstruments);
        }

        private static IFixClient CreateFixClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            FixConfiguration configuration,
            InstrumentDataProvider instrumentData)
        {
            switch (configuration.Broker)
            {
                case Brokerage.FXCM:
                    return FxcmFixClientFactory.Create(
                        container,
                        configuration,
                        instrumentData);
                case Brokerage.DUKASCOPY:
                    return DukascopyFixClientFactory.Create(
                        container,
                        configuration,
                        instrumentData);
                case Brokerage.Simulation:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                case Brokerage.IB:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                case Brokerage.LMAX:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                default:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client (broker {configuration.Broker} is not recognized).");
            }
        }
    }
}
