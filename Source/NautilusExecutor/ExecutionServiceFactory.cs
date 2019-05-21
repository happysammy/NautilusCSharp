//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
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
    using Nautilus.DomainModel.Enums;
    using Nautilus.Execution;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.MsgPack;
    using Nautilus.Scheduler;
    using Nautilus.Serilog;
    using NodaTime;

    /// <summary>
    /// Provides a factory for creating the <see cref="ExecutionService"/>.
    /// </summary>
    public static class ExecutionServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="ExecutionService"/> and returns an address book of endpoints.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The services switchboard.</returns>
        public static ExecutionService Create(Configuration config)
        {
            var loggingAdapter = new SerilogLogger(config.LogLevel);
            loggingAdapter.Debug(NautilusService.Core, $"Starting {nameof(NautilusExecutor)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(container);
            var scheduler = new HashedWheelTimerScheduler(container);

            var venue = config.FixConfiguration.Broker.ToString().ToEnum<Venue>();
            var symbolProvider = new SymbolConverter(venue, config.SymbolIndex);

            var messageServer = new MessageServer(
                container,
                messagingAdapter,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer(),
                config.ServerAddress,
                config.CommandsPort,
                config.EventsPort);

            var orderManager = new OrderManager(container, messagingAdapter);

            var fixClient = CreateFixClient(
                container,
                messagingAdapter,
                config.FixConfiguration,
                symbolProvider);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            // Wire up system
            fixGateway.RegisterConnectionEventReceiver(ExecutionServiceAddress.Core);
            fixGateway.RegisterEventReceiver(orderManager.Endpoint);

            var addresses = new Dictionary<Address, IEndpoint>
            {
                { ExecutionServiceAddress.Scheduler, scheduler.Endpoint },
                { ExecutionServiceAddress.FixGateway, fixGateway.Endpoint },
                { ExecutionServiceAddress.MessageServer, messageServer.Endpoint },
                { ExecutionServiceAddress.OrderManager, orderManager.Endpoint },
            };

            return new ExecutionService(
                container,
                messagingAdapter,
                addresses,
                fixGateway,
                config.CommandsPerSecond,
                config.NewOrdersPerSecond);
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
