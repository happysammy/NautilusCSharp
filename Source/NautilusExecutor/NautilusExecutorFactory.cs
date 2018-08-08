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
    using Nautilus.MsgPack;
    using Nautilus.RabbitMQ;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NodaTime;
    using Serilog.Events;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusExecutor"/> system.
    /// </summary>
    public static class NautilusExecutorFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NautilusExecutor"/> class.
        /// </summary>
        /// <param name="logLevel">The logger log level threshold.</param>
        /// <param name="fixCredentials">The FIX credentials.</param>
        /// <returns>The <see cref="NautilusExecutor"/> system.</returns>
        public static NautilusExecutor Create(
            LogEventLevel logLevel,
            FixCredentials fixCredentials)
        {
            Validate.NotNull(fixCredentials, nameof(fixCredentials));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusExecutor)} builder...");
            BuildVersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var actorSystem = ActorSystem.Create(nameof(NautilusExecutor));
            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var setupContainer = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(
                actorSystem,
                setupContainer,
                new FakeMessageStore());

            var clientManager = new BasicRedisClientManager(
                new[] { RedisConstants.LocalHost },
                new[] { RedisConstants.LocalHost });

            var instrumentRepository = new RedisInstrumentRepository(clientManager);

            var fixClient = FxcmFixClientFactory.Create(
                setupContainer,
                messagingAdapter,
                fixCredentials);

            var gateway = ExecutionGatewayFactory.Create(
                setupContainer,
                instrumentRepository,
                fixClient);

            fixClient.InitializeGateway(gateway);

            var messageBroker = RabbitMQServerFactory.Create(
                actorSystem,
                setupContainer,
                messagingAdapter,
                new MsgPackCommandSerializer(),
                new MsgPackEventSerializer());

            var executionServiceAddresses = ExecutionServiceFactory.Create(
                actorSystem,
                setupContainer,
                messagingAdapter);

            var switchboard = new Switchboard(executionServiceAddresses);

            var systemController = new SystemController(
                setupContainer,
                actorSystem,
                messagingAdapter,
                switchboard);

            return new NautilusExecutor(
                setupContainer,
                messagingAdapter,
                systemController,
                fixClient,
                messageBroker);
        }
    }
}
