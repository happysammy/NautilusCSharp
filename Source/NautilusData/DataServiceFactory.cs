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
    using Nautilus.Common.Messages.Commands;
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
    using Nautilus.Serialization.Compression;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.Serialization.MessageSerializers;
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
            scheduler.Start();

            var messagingAdapter = MessageBusFactory.Create(container);
            messagingAdapter.Start();

            var dataBusAdapter = DataBusFactory.Create(container);
            dataBusAdapter.Start();

            var compressor = CompressorFactory.Create(config.MessagingConfig.CompressionCodec);

            var tickPublisher = new TickPublisher(
                container,
                dataBusAdapter,
                new TickDataSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.TickPublisherPort);

            var barPublisher = new BarPublisher(
                container,
                dataBusAdapter,
                new BarDataSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.BarPublisherPort);

            var instrumentPublisher = new InstrumentPublisher(
                container,
                dataBusAdapter,
                new InstrumentDataSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.InstrumentPublisherPort);

            var symbolConverter = new SymbolConverter(config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolConverter);

            var dataGateway = FixDataGatewayFactory.Create(
                container,
                dataBusAdapter,
                fixClient);

            var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");

            var tickRepository = new RedisTickRepository(
                container,
                dataBusAdapter,
                new TickDataSerializer(),
                connection);

            var barRepository = new RedisBarRepository(
                container,
                dataBusAdapter,
                new BarDataSerializer(),
                connection);

            var instrumentRepository = new RedisInstrumentRepository(
                container,
                dataBusAdapter,
                new InstrumentDataSerializer(),
                connection);
            instrumentRepository.CacheAll();

            var barAggregationController = new BarAggregationController(
                container,
                dataBusAdapter,
                scheduler);

            var tickProvider = new TickProvider(
                container,
                tickRepository,
                new TickDataSerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.TickRouterPort);

            var barProvider = new BarProvider(
                container,
                barRepository,
                new BarDataSerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.BarRouterPort);

            var instrumentProvider = new InstrumentProvider(
                container,
                instrumentRepository,
                new InstrumentDataSerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                config.MessagingConfig.EncryptionConfig,
                config.InstrumentRouterPort);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.BarAggregationController, barAggregationController.Endpoint },
                { ServiceAddress.TickRepository, tickRepository.Endpoint },
                { ServiceAddress.TickProvider, tickProvider.Endpoint },
                { ServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { ServiceAddress.BarRepository, barRepository.Endpoint },
                { ServiceAddress.BarProvider, barProvider.Endpoint },
                { ServiceAddress.BarPublisher, barPublisher.Endpoint },
                { ServiceAddress.InstrumentRepository, instrumentRepository.Endpoint },
                { ServiceAddress.InstrumentProvider, instrumentProvider.Endpoint },
                { ServiceAddress.InstrumentPublisher, instrumentPublisher.Endpoint },
                { ServiceAddress.DataGateway, dataGateway.Endpoint },
            };

            var dataService = new DataService(
                container,
                messagingAdapter,
                dataBusAdapter,
                scheduler,
                dataGateway,
                config);

            addresses.Add(ServiceAddress.DataService, dataService.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                dataService.NewGuid(),
                dataService.TimeNow()));

            return dataService;
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
