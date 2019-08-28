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
    using Nautilus.Brokerage.Dukascopy;
    using Nautilus.Brokerage.Fxcm;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution;
    using Nautilus.Execution.Network;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using Nautilus.Serialization;
    using NodaTime;

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

            var venue = new Venue(config.FixConfiguration.Broker.ToString());
            var symbolConverter = new SymbolConverter(venue, config.SymbolIndex);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolConverter);

            var eventPublisher = new EventPublisher(
                container,
                messagingAdapter,
                new MsgPackEventSerializer(),
                config.ServerAddress,
                config.EventsPort);

            var fixGateway = FixTradingGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var orderManager = new OrderManager(
                container,
                messagingAdapter,
                fixGateway);

            var commandServer = new CommandRouter(
                container,
                messagingAdapter,
                new MsgPackCommandSerializer(),
                new MsgPackResponseSerializer(),
                orderManager.Endpoint,
                config);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ServiceAddress.Scheduler, scheduler.Endpoint },
                { ServiceAddress.TradingGateway, fixGateway.Endpoint },
                { ServiceAddress.OrderManager, orderManager.Endpoint },
                { ServiceAddress.CommandServer, commandServer.Endpoint },
                { ServiceAddress.EventPublisher, eventPublisher.Endpoint },
            };

            return new ExecutionService(
                container,
                messagingAdapter,
                addresses,
                scheduler,
                fixGateway,
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
