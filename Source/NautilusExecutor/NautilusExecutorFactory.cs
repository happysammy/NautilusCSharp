//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusExecutorFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusExecutor
{
    using Akka.Actor;
    using global::NautilusExecutor.Build;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Build;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Validation;
    using Nautilus.Execution;
    using Nautilus.Fix;
    using Nautilus.Messaging.Network;
    using Nautilus.MsgPack;
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
            Validate.NotNull(fixConfig, nameof(fixConfig));
            Validate.NotNull(serviceAddress, nameof(serviceAddress));
            Validate.NotNull(commandsPort, nameof(commandsPort));
            Validate.NotNull(eventsPort, nameof(eventsPort));
            Validate.PositiveInt32(commandsPerSecond, nameof(commandsPerSecond));
            Validate.PositiveInt32(newOrdersPerSecond, nameof(newOrdersPerSecond));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusExecutor)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var actorSystem = ActorSystem.Create(nameof(NautilusExecutor));
            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                container,
                new FakeMessageStore());

            var fixClient = FxcmFixClientFactory.Create(
                container,
                messagingAdapter,
                fixConfig);

            var fixGateway = FixGatewayFactory.Create(
                container,
                messagingAdapter,
                fixClient,
                FxcmInstrumentDataProvider.GetTickDecimalsIndex());

            var switchboard = ExecutionServiceFactory.Create(
                actorSystem,
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

            var systemController = new SystemController(
                container,
                actorSystem,
                messagingAdapter,
                switchboard);

            return new NautilusExecutor(
                container,
                messagingAdapter,
                systemController,
                fixClient);
        }
    }
}
