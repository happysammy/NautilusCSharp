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
    using System.Collections.Immutable;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Data;
    using Nautilus.Common.Interfaces;
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
    using Nautilus.Network.Compression;
    using Nautilus.Redis.Data;
    using Nautilus.Scheduling;
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
        public static DataService Create(ServiceConfiguration config)
        {
            VersionChecker.Run(
                config.LoggerFactory.CreateLogger(nameof(DataService)),
                "NAUTILUS DATA - Algorithmic Trading Data Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                config.LoggerFactory);

            var scheduler = new HashedWheelTimerScheduler(container);
            scheduler.Start();

            var messagingAdapter = MessageBusFactory.Create(container);
            messagingAdapter.Start();

            var dataBusAdapter = DataBusFactory.Create(container);
            dataBusAdapter.Start();

            var compressor = CompressorFactory.Create(config.WireConfig.CompressionCodec);

            var tickPublisher = new TickPublisher(
                container,
                dataBusAdapter,
                new TickDataSerializer(),
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.TickPublisherPort);

            var barPublisher = new BarPublisher(
                container,
                dataBusAdapter,
                new BarDataSerializer(),
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.BarPublisherPort);

            var instrumentPublisher = new InstrumentPublisher(
                container,
                dataBusAdapter,
                new InstrumentDataSerializer(),
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.InstrumentPublisherPort);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfig,
                config.SymbolMap);

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
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.TickRouterPort);

            var barProvider = new BarProvider(
                container,
                barRepository,
                new BarDataSerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.BarRouterPort);

            var instrumentProvider = new InstrumentProvider(
                container,
                instrumentRepository,
                new InstrumentDataSerializer(),
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.InstrumentRouterPort);

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
            ImmutableDictionary<string, string> symbolMap)
        {
            switch (configuration.Broker.Value)
            {
                case "FXCM":
                    return FxcmFixClientFactory.Create(
                        container,
                        messageBusAdapter,
                        configuration,
                        symbolMap);
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
