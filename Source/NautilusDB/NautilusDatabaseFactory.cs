//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabaseFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.Common;
    using Nautilus.Common.Build;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Logging;
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Compression;
    using Nautilus.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Execution;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NautilusDB.Build;
    using NodaTime;
    using Serilog.Events;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusDatabase"/> system.
    /// </summary>
    public static class NautilusDatabaseFactory
    {
        public static NautilusDatabase Create(
            LogEventLevel logLevel,
            bool isCompression,
            string compressionCodec,
            string username,
            string password,
            string accountNumber,
            IReadOnlyList<string> symbols,
            IReadOnlyList<string> barSpecs,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow)
        {
            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusDB)} builder...");
            BuildVersionChecker.Run(loggingAdapter, "NautilusExecutor - Financial Market Execution Service");

            var actorSystem = ActorSystem.Create(nameof(NautilusDB));
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

            var gatewayFactory = new ExecutionGatewayFactory();

            var fixClientFactory = new FxcmFixClientFactory(
                username,
                password,
                accountNumber);

            var publisherFactory = new RedisChannelPublisherFactory(clientManager);

            var barRepository = new RedisBarRepository(
                clientManager,
                CompressorFactory.Create(isCompression, compressionCodec));

            var instrumentRepository = new RedisInstrumentRepository(clientManager);

            var dataServiceAddresses = DataServiceFactory.Create(
                setupContainer,
                actorSystem,
                messagingAdapter,
                fixClientFactory,
                gatewayFactory,
                publisherFactory,
                barRepository,
                instrumentRepository,
                symbols,
                resolutions,
                barRollingWindow);

            var switchboard = new Switchboard(dataServiceAddresses);

            var systemController = new SystemController(
                setupContainer,
                actorSystem,
                messagingAdapter,
                switchboard);

            return new NautilusDatabase(
                setupContainer,
                messagingAdapter,
                systemController);
        }
    }
}
