//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System.Collections.Generic;
    using Nautilus.Brokerage.Fxcm;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution;
    using Nautilus.Execution.Engine;
    using Nautilus.Execution.Network;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Redis.Execution;
    using Nautilus.Scheduler;
    using Nautilus.Serialization;
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
        public static ExecutionService Create(Configuration config)
        {
            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(config.LoggingAdapter));

            var messagingAdapter = MessageBusFactory.Create(container);
            var scheduler = new HashedWheelTimerScheduler(container);

            var venue = new Venue(config.FixConfiguration.Broker.Value);
            var symbolConverter = new SymbolConverter(config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolConverter);

            var tradingGateway = FixTradingGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var connection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var executionDatabase = new RedisExecutionDatabase(
                container,
                connection,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer());

            var eventPublisher = new EventPublisher(
                container,
                new MsgPackEventSerializer(),
                config.EventsPort);

            var executionEngine = new ExecutionEngine(
                container,
                messagingAdapter,
                executionDatabase,
                tradingGateway,
                eventPublisher.Endpoint);

            var commandServer = new CommandRouter(
                container,
                messagingAdapter,
                new MsgPackCommandSerializer(),
                new MsgPackResponseSerializer(),
                executionEngine.Endpoint,
                config);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.TradingGateway, tradingGateway.Endpoint },
                { ServiceAddress.ExecutionEngine, executionEngine.Endpoint },
                { ServiceAddress.CommandServer, commandServer.Endpoint },
                { ServiceAddress.EventPublisher, eventPublisher.Endpoint },
            };

            return new ExecutionService(
                container,
                messagingAdapter,
                addresses,
                scheduler,
                tradingGateway,
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
