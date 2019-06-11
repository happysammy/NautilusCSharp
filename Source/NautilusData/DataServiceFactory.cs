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
    using Nautilus.Common.Data;
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

            var scheduler = new HashedWheelTimerScheduler(container);
            var messageBusAdapter = MessageBusFactory.Create(container);
            var dataBusAdapter = DataBusFactory.Create(container);

            var tickPublisher = new TickPublisher(
                container,
                dataBusAdapter,
                new TickSerializer(),
                config.ServerAddress,
                config.TickSubscribePort);

            var barPublisher = new BarPublisher(
                container,
                dataBusAdapter,
                new BarSerializer(),
                config.ServerAddress,
                config.BarSubscribePort);

            var instrumentPublisher = new InstrumentPublisher(
                container,
                dataBusAdapter,
                new MsgPackInstrumentSerializer(),
                config.ServerAddress,
                config.InstrumentSubscribePort);

            var venue = config.FixConfiguration.Broker.ToString().ToEnum<Venue>();
            var symbolConverter = new SymbolConverter(venue, config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messageBusAdapter,
                config.FixConfiguration,
                symbolConverter);

            var dataGateway = FixDataGatewayFactory.Create(
                container,
                messageBusAdapter,
                dataBusAdapter,
                fixClient);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var tickRepository = new InMemoryTickStore(container, dataBusAdapter);
            var barRepository = new RedisBarRepository(redisConnection);
            var instrumentRepository = new RedisInstrumentRepository(redisConnection);
            instrumentRepository.CacheAll();

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                dataBusAdapter,
                barRepository,
                instrumentRepository);

            var barAggregationController = new BarAggregationController(
                container,
                dataBusAdapter,
                scheduler);

            var tickProvider = new TickProvider(
                container,
                tickRepository,
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.TickRequestPort);

            var barProvider = new BarProvider(
                container,
                barRepository,
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.BarRequestPort);

            var instrumentProvider = new InstrumentProvider(
                container,
                instrumentRepository,
                new MsgPackInstrumentSerializer(),
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                config.ServerAddress,
                config.InstrumentRequestPort);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.DataGateway, dataGateway.Endpoint },
                { ServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { ServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { ServiceAddress.TickProvider, tickProvider.Endpoint },
                { ServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { ServiceAddress.BarProvider, barProvider.Endpoint },
                { ServiceAddress.BarPublisher, barPublisher.Endpoint },
                { ServiceAddress.InstrumentProvider, instrumentProvider.Endpoint },
                { ServiceAddress.InstrumentPublisher, instrumentPublisher.Endpoint },
            };

            return new DataService(
                container,
                messageBusAdapter,
                addresses,
                scheduler,
                dataGateway,
                config);
        }

        private static IFixClient CreateFixClient(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            FixConfiguration configuration,
            SymbolConverter symbolConverter)
        {
            switch (configuration.Broker)
            {
                case Brokerage.FXCM:
                    return FxcmFixClientFactory.Create(
                        container,
                        messageBusAdapter,
                        configuration,
                        symbolConverter);
                case Brokerage.DUKASCOPY:
                    return DukascopyFixClientFactory.Create(
                        container,
                        messageBusAdapter,
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
