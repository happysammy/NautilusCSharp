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

            var tickDataSerializer = new TickDataSerializer();
            var barDataSerializer = new BarDataSerializer();
            var instrumentDataSerializer = new InstrumentDataSerializer();
            var headerSerializer = new MsgPackDictionarySerializer();
            var requestSerializer = new MsgPackRequestSerializer();
            var responseSerializer = new MsgPackResponseSerializer();
            var compressor = CompressorFactory.Create(config.WireConfig.CompressionCodec);

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
                tickDataSerializer,
                connection);

            var barRepository = new RedisBarRepository(
                container,
                dataBusAdapter,
                barDataSerializer,
                connection);

            var instrumentRepository = new RedisInstrumentRepository(
                container,
                dataBusAdapter,
                instrumentDataSerializer,
                connection);
            instrumentRepository.CacheAll();

            var barAggregationController = new BarAggregationController(
                container,
                dataBusAdapter,
                scheduler);

            var tickProvider = new TickProvider(
                container,
                messagingAdapter,
                tickRepository,
                tickDataSerializer);

            var barProvider = new BarProvider(
                container,
                messagingAdapter,
                barRepository,
                barDataSerializer);

            var instrumentProvider = new InstrumentProvider(
                container,
                messagingAdapter,
                instrumentRepository,
                instrumentDataSerializer);

            var dataServer = new DataServer(
                container,
                messagingAdapter,
                headerSerializer,
                requestSerializer,
                responseSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.DataReqPort,
                config.NetworkConfig.DataResPort);

            var dataPublisher = new DataPublisher(
                container,
                dataBusAdapter,
                barDataSerializer,
                instrumentDataSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.DataPubPort);

            var tickPublisher = new TickPublisher(
                container,
                dataBusAdapter,
                tickDataSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.TickPubPort);

            // TODO: Refactor to auto generate
            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.DataGateway, dataGateway.Endpoint },
                { ServiceAddress.DataServer, dataServer.Endpoint },
                { ServiceAddress.DataPublisher, dataPublisher.Endpoint },
                { ServiceAddress.TickRepository, tickRepository.Endpoint },
                { ServiceAddress.TickProvider, tickProvider.Endpoint },
                { ServiceAddress.TickPublisher, tickPublisher.Endpoint },
                { ServiceAddress.BarRepository, barRepository.Endpoint },
                { ServiceAddress.BarProvider, barProvider.Endpoint },
                { ServiceAddress.InstrumentRepository, instrumentRepository.Endpoint },
                { ServiceAddress.InstrumentProvider, instrumentProvider.Endpoint },
                { ServiceAddress.BarAggregationController, barAggregationController.Endpoint },
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
                guidFactory.Generate(),
                clock.TimeNow()));

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
