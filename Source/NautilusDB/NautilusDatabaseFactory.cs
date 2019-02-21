//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabaseFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
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
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NodaTime;
    using Serilog.Events;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusDatabase"/> system.
    /// </summary>
    public static class NautilusDatabaseFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NautilusDatabase"/> system.
        /// </summary>
        /// <param name="logLevel">The log level threshold.</param>
        /// <param name="isCompression">The is data compression on boolean flag.</param>
        /// <param name="compressionCodec">The data compression codec.</param>
        /// <param name="fixConfig">The FIX configuration.</param>
        /// <param name="symbols">The symbols to collect.</param>
        /// <param name="resolutions">The resolutions to persist.</param>
        /// <param name="barRollingWindow">The length of the rolling window for bar data.</param>
        /// <returns>The <see cref="Nautilus"/> system.</returns>
        public static NautilusDatabase Create(
            LogEventLevel logLevel,
            bool isCompression,
            string compressionCodec,
            FixConfiguration fixConfig,
            IReadOnlyList<string> symbols,
            IReadOnlyList<Resolution> resolutions,
            int barRollingWindow)
        {
            Validate.NotNull(compressionCodec, nameof(compressionCodec));
            Validate.NotNull(fixConfig, nameof(fixConfig));
            Validate.NotNullOrEmpty(symbols, nameof(symbols));
            Validate.NotNullOrEmpty(resolutions, nameof(resolutions));
            Validate.PositiveInt32(barRollingWindow, nameof(barRollingWindow));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Information(NautilusService.Data, $"Starting {nameof(NautilusDB)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusDB - Financial Market Data Service");

            var actorSystem = ActorSystem.Create(nameof(NautilusDB));
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

            var scheduler = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                    () => new Scheduler(container))));

            var clientManager = new BasicRedisClientManager(
                new[] { RedisConstants.LocalHost },
                new[] { RedisConstants.LocalHost });

            var barRepository = new RedisBarRepository(clientManager);

            var instrumentRepository = new RedisInstrumentRepository(clientManager);
            instrumentRepository.CacheAll();

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

            var dataServiceAddresses = DataServiceFactory.Create(
                actorSystem,
                container,
                messagingAdapter,
                fixClient,
                fixGateway,
                new RedisChannelPublisherFactory(clientManager),
                barRepository,
                instrumentRepository,
                symbols,
                resolutions,
                barRollingWindow,
                fixConfig.UpdateInstruments);

            dataServiceAddresses.Add(ServiceAddress.Scheduler, scheduler);
            var switchboard = new Switchboard(dataServiceAddresses);

            var systemController = new SystemController(
                container,
                actorSystem,
                messagingAdapter,
                switchboard);

            return new NautilusDatabase(
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
