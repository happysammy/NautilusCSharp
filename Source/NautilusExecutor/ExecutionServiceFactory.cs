//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Brokerage.Dukascopy;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
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
            var scheduler = new HashedWheelTimerScheduler(container.LoggerFactory.Create(NautilusService.Scheduling, new Label("Scheduler")));

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
                config.FixConfiguration,
                symbolProvider);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var addresses = new Dictionary<Address, IEndpoint>
            {
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
            FixConfiguration configuration,
            SymbolConverter symbolConverter)
        {
            switch (configuration.Broker)
            {
                case Brokerage.FXCM:
                    return FxcmFixClientFactory.Create(
                        container,
                        configuration,
                        symbolConverter);
                case Brokerage.DUKASCOPY:
                    return DukascopyFixClientFactory.Create(
                        container,
                        configuration,
                        symbolConverter);
                case Brokerage.Simulation:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                case Brokerage.IB:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                case Brokerage.LMAX:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client for broker {configuration.Broker}.");
                default:
                    throw new InvalidOperationException(
                        $"Cannot create FIX client (broker {configuration.Broker} is not recognized).");
            }
        }
    }
}
