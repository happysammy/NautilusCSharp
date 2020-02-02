//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Data;
    using Nautilus.Data.Aggregation;
    using Nautilus.Data.Providers;
    using Nautilus.Data.Publishers;
    using Nautilus.Fix;
    using Nautilus.Fxcm;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Redis.Data;
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
                new Utf8TickSerializer(),
                config.TickSubscribePort);

            var barPublisher = new BarPublisher(
                container,
                dataBusAdapter,
                new Utf8BarSerializer(),
                config.BarSubscribePort);

            var instrumentPublisher = new InstrumentPublisher(
                container,
                dataBusAdapter,
                new BsonInstrumentSerializer(),
                config.InstrumentSubscribePort);

            var symbolConverter = new SymbolConverter(config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messageBusAdapter,
                config.FixConfiguration,
                symbolConverter);

            var dataGateway = FixDataGatewayFactory.Create(
                container,
                dataBusAdapter,
                fixClient);

            var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var tickRepository = new InMemoryTickStore(container, dataBusAdapter);
            var barRepository = new RedisBarRepository(container, connection);
            var instrumentRepository = new RedisInstrumentRepository(
                container,
                new BsonInstrumentSerializer(),
                connection);
            instrumentRepository.CacheAll();

            var databaseTaskManager = new DatabaseTaskManager(
                container,
                dataBusAdapter,
                tickRepository,
                barRepository,
                instrumentRepository);

            var barAggregationController = new BarAggregationController(
                container,
                dataBusAdapter,
                scheduler);

            var tickProvider = new TickProvider(
                container,
                tickRepository,
                new BsonByteArrayArraySerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                config.TickRequestPort);

            var barProvider = new BarProvider(
                container,
                barRepository,
                new BsonByteArrayArraySerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                config.BarRequestPort);

            var instrumentProvider = new InstrumentProvider(
                container,
                instrumentRepository,
                new BsonByteArrayArraySerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                config.InstrumentRequestPort);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.DataGateway, dataGateway.Endpoint },
                { ServiceAddress.DatabaseTaskManager, databaseTaskManager.Endpoint },
                { ServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { ServiceAddress.TickStore, tickRepository.Endpoint },
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
            switch (configuration.Broker.Value)
            {
                case "FXCM":
                    return FxcmFixClientFactory.Create(
                        container,
                        messageBusAdapter,
                        configuration,
                        symbolConverter);
                case "SIMULATION":
                    goto default;
                case "IB":
                    goto default;
                case "LMAX":
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(
                        configuration.Broker,
                        nameof(configuration.Broker));
            }
        }
    }
}
