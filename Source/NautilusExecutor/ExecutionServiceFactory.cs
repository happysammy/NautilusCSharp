//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Configuration;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Execution;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Network;
    using Nautilus.Fix;
    using Nautilus.Fxcm;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Compression;
    using Nautilus.Redis.Execution;
    using Nautilus.Scheduling;
    using Nautilus.Serialization.MessageSerializers;
    using NodaTime;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a factory for creating the <see cref="ExecutionService"/>.
    /// </summary>
    public static class ExecutionServiceFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ExecutionService"/>.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The service.</returns>
        public static ExecutionService Create(ServiceConfiguration config)
        {
            VersionChecker.Run(
                config.LoggerFactory.CreateLogger(nameof(ExecutionService)),
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

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfig,
                config.SymbolMap);

            var tradingGateway = FixTradingGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var requestSerializer = new MsgPackRequestSerializer();
            var responseSerializer = new MsgPackResponseSerializer();
            var commandSerializer = new MsgPackCommandSerializer();
            var eventSerializer = new MsgPackEventSerializer();

            var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var executionDatabase = new RedisExecutionDatabase(
                container,
                connection,
                commandSerializer,
                eventSerializer);

            var compressor = CompressorFactory.Create(config.WireConfig.CompressionCodec);

            var eventPublisher = new EventPublisher(
                container,
                eventSerializer,
                compressor,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.EventsPort);

            var executionEngine = new ExecutionEngine(
                container,
                scheduler,
                messagingAdapter,
                executionDatabase,
                tradingGateway,
                eventPublisher.Endpoint);

            var commandRouter = new CommandRouter(
                container,
                messagingAdapter,
                executionEngine.Endpoint,
                config.NetworkConfig);

            var commandServer = new CommandServer(
                container,
                requestSerializer,
                responseSerializer,
                commandSerializer,
                compressor,
                commandRouter.Endpoint,
                config.WireConfig.EncryptionConfig,
                config.NetworkConfig.CommandsPort);

            // TODO: Refactor to auto generate
            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.ExecutionEngine, executionEngine.Endpoint },
                { ServiceAddress.CommandRouter, commandRouter.Endpoint },
                { ServiceAddress.CommandServer, commandServer.Endpoint },
                { ServiceAddress.EventPublisher, eventPublisher.Endpoint },
                { ServiceAddress.TradingGateway, tradingGateway.Endpoint },
            };

            var executionService = new ExecutionService(
                container,
                messagingAdapter,
                scheduler,
                tradingGateway,
                config);

            addresses.Add(ServiceAddress.ExecutionService, executionService.Endpoint);
            messagingAdapter.Send(new InitializeSwitchboard(
                Switchboard.Create(addresses),
                guidFactory.Generate(),
                clock.TimeNow()));

            return executionService;
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
