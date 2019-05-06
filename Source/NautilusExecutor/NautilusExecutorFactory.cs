//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutorFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using System;
    using Nautilus.Brokerage.Dukascopy;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Common.Scheduling;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Execution;
    using Nautilus.Fix;
    using Nautilus.MsgPack;
    using Nautilus.Network;
    using Nautilus.Serilog;
    using NodaTime;
    using Serilog.Events;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusExecutor"/> system.
    /// </summary>
    public static class NautilusExecutorFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NautilusExecutor"/> class.
        /// </summary>
        /// <param name="logLevel">The logger log level threshold.</param>
        /// <param name="fixConfig">The FIX configuration.</param>
        /// <param name="serviceAddress">The services address.</param>
        /// <param name="commandsPort">The commands port.</param>
        /// <param name="eventsPort">The events port.</param>
        /// <param name="commandsPerSecond">The commands per second throttle limit.</param>
        /// <param name="newOrdersPerSecond">The new orders per second throttle limit.</param>
        /// <returns>The <see cref="NautilusExecutor"/> system.</returns>
        public static NautilusExecutor Create(
            LogEventLevel logLevel,
            FixConfiguration fixConfig,
            NetworkAddress serviceAddress,
            Port commandsPort,
            Port eventsPort,
            int commandsPerSecond,
            int newOrdersPerSecond)
        {
            Precondition.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Precondition.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusExecutor)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(container, new FakeMessageStore());

            var scheduler = new Scheduler(container);

            var venue = fixConfig.Broker.ToString().ToEnum<Venue>();
            var instrumentData = new InstrumentDataProvider(venue, fixConfig.InstrumentDataFileName);

            var fixClient = GetFixClient(
                container,
                messagingAdapter,
                fixConfig,
                instrumentData);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient);

            var executionServiceAddresses = ExecutionServiceFactory.Create(
                container,
                messagingAdapter,
                fixClient,
                fixGateway,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer(),
                serviceAddress,
                commandsPort,
                eventsPort,
                commandsPerSecond,
                newOrdersPerSecond);

            executionServiceAddresses.Add(ServiceAddress.Scheduler, scheduler.Endpoint);
            var switchboard = Switchboard.Create(executionServiceAddresses);

            var systemController = new SystemController(
                container,
                messagingAdapter,
                switchboard);

            return new NautilusExecutor(
                container,
                messagingAdapter,
                systemController,
                fixClient);
        }

        private static IFixClient GetFixClient(
            ComponentryContainer container,
            MessagingAdapter messagingAdapter,
            FixConfiguration configuration,
            InstrumentDataProvider instrumentData)
        {
            switch (configuration.Broker)
            {
                case Brokerage.FXCM:
                    return FxcmFixClientFactory.Create(
                        container,
                        messagingAdapter,
                        configuration,
                        instrumentData);

                case Brokerage.DUKASCOPY:
                    return DukascopyFixClientFactory.Create(
                        container,
                        messagingAdapter,
                        configuration,
                        instrumentData);

                default:
                    throw new InvalidOperationException($"Cannot create FIX client (broker {configuration.Broker} is not recognized).");
            }
        }
    }
}
