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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data;
    using Nautilus.Data.Aggregation;
    using Nautilus.Data.Network;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Redis;
    using Nautilus.Scheduler;
    using Nautilus.Serialization;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a factory for creating the <see cref="DataService"/>.
    /// </summary>
    public static class DataServiceFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="DataService"/>.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The service.</returns>
        public static DataService Create(Configuration config)
        {
            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(config.LoggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(container);
            var scheduler = new HashedWheelTimerScheduler(container);

            var venue = config.FixConfiguration.Broker.ToString().ToEnum<Venue>();
            var symbolConverter = new SymbolConverter(venue, config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolConverter);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var tickRepository = new InMemoryTickStore(container);
            var barRepository = new RedisBarRepository(redisConnection);
            var instrumentRepository = new RedisInstrumentRepository(redisConnection);
            instrumentRepository.CacheAll();

            var tickProvider = new TickProvider(
                container,
                tickRepository,
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.TickRequestPort);

            var tickPublisher = new TickPublisher(
                container,
                config.ServerAddress,
                config.TickSubscribePort);

            var barProvider = new BarProvider(
                container,
                barRepository,
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.BarRequestPort);

            var barPublisher = new BarPublisher(
                container,
                config.ServerAddress,
                config.BarSubscribePort);

            var instrumentProvider = new InstrumentProvider(
                container,
                instrumentRepository,
                new MsgPackInstrumentSerializer(),
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.InstrumentRequestPort);

            var instrumentPublisher = new InstrumentPublisher(
                container,
                new MsgPackInstrumentSerializer(),
                config.ServerAddress,
                config.InstrumentSubscribePort);

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                barRepository,
                instrumentRepository);

            var barAggregationController = new BarAggregationController(
                container,
                messagingAdapter,
                scheduler,
                barPublisher.Endpoint);

            // Wire up service.
            fixGateway.RegisterConnectionEventReceiver(DataServiceAddress.Core);
            fixGateway.RegisterTickReceiver(tickPublisher.Endpoint);
            fixGateway.RegisterTickReceiver(tickRepository.Endpoint);
            fixGateway.RegisterTickReceiver(barAggregationController.Endpoint);
            fixGateway.RegisterInstrumentReceiver(DataServiceAddress.DatabaseTaskManager);
            fixGateway.RegisterInstrumentReceiver(DataServiceAddress.InstrumentPublisher);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { DataServiceAddress.Scheduler, scheduler.Endpoint },
                { DataServiceAddress.FixGateway, fixGateway.Endpoint },
                { DataServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { DataServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { DataServiceAddress.TickStore, tickRepository.Endpoint },
                { DataServiceAddress.TickProvider, tickProvider.Endpoint },
                { DataServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { DataServiceAddress.BarProvider, barProvider.Endpoint },
                { DataServiceAddress.BarPublisher, barPublisher.Endpoint },
                { DataServiceAddress.InstrumentProvider, instrumentProvider.Endpoint },
                { DataServiceAddress.InstrumentPublisher, instrumentPublisher.Endpoint },
            };

            return new DataService(
                container,
                messagingAdapter,
                addresses,
                scheduler,
                fixGateway,
                config);
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
