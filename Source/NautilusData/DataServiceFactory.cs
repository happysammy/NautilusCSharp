//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System.Collections.Generic;
    using Nautilus.Brokerage.Dukascopy;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data;
    using Nautilus.Data.Aggregation;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Redis;
    using Nautilus.Scheduler;
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

            var messagingAdapter = MessagingServiceFactory.Create(container);
            var scheduler = new HashedWheelTimerScheduler(container);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var barRepository = new RedisBarRepository(redisConnection);
            var instrumentRepository = new RedisInstrumentRepository(redisConnection);
            instrumentRepository.CacheAll();

            var venue = config.FixConfiguration.Broker.ToString().ToEnum<Venue>();
            var symbolConverter = new SymbolConverter(venue, config.SymbolIndex);

            var tickPublisher = new TickPublisher(container, config.ServerAddress, config.TickPublisherPort);
            var barPublisher = new BarPublisher(container, config.ServerAddress, config.TickPublisherPort);

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                barRepository,
                instrumentRepository);

            var barAggregationController = new BarAggregationController(
                container,
                messagingAdapter,
                scheduler,
                barPublisher.Endpoint);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolConverter);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            // Wire up system
            fixGateway.RegisterConnectionEventReceiver(DataServiceAddress.Core);
            fixGateway.RegisterTickReceiver(tickPublisher.Endpoint);
            fixGateway.RegisterTickReceiver(barAggregationController.Endpoint);
            fixGateway.RegisterInstrumentReceiver(DataServiceAddress.DatabaseTaskManager);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { DataServiceAddress.Scheduler, scheduler.Endpoint },
                { DataServiceAddress.FixGateway, fixGateway.Endpoint },
                { DataServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { DataServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { DataServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { DataServiceAddress.BarPublisher, barPublisher.Endpoint },
            };

            return new DataService(
                container,
                messagingAdapter,
                addresses,
                scheduler,
                fixGateway,
                symbolConverter.GetAllSymbols(),
                config.BarSpecifications,
                config.BarRollingWindowDays);
        }

        private static IFixClient CreateFixClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            FixConfiguration configuration,
            SymbolConverter symbolConverter)
        {
            switch (configuration.Broker)
            {
                case Brokerage.FXCM:
                    return FxcmFixClientFactory.Create(
                        container,
                        messagingAdapter,
                        configuration,
                        symbolConverter);
                case Brokerage.DUKASCOPY:
                    return DukascopyFixClientFactory.Create(
                        container,
                        messagingAdapter,
                        configuration,
                        symbolConverter);
                case Brokerage.Simulation:
                    goto default;
                case Brokerage.IB:
                    goto default;
                case Brokerage.LMAX:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(
                        configuration.Broker,
                        nameof(configuration.Broker));
            }
        }
    }
}
