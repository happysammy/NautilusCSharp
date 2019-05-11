//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDataFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
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
    using Nautilus.Common.MessageStore;
    using Nautilus.Common.Messaging;
    using Nautilus.Common.Scheduling;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix;
    using Nautilus.Redis;
    using Nautilus.Serilog;
    using NodaTime;
    using Serilog.Events;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a factory for creating a <see cref="NautilusData"/> system.
    /// </summary>
    public static class NautilusDataFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="NautilusData"/> system.
        /// </summary>
        /// <param name="logLevel">The log level threshold.</param>
        /// <param name="fixConfig">The FIX configuration.</param>
        /// <param name="symbols">The symbols to collect.</param>
        /// <param name="resolutions">The resolutions to persist.</param>
        /// <param name="barRollingWindow">The length of the rolling window for bar data.</param>
        /// <returns>The <see cref="Nautilus"/> system.</returns>
        public static NautilusData Create(
            LogEventLevel logLevel,
            FixConfiguration fixConfig,
            IReadOnlyCollection<string> symbols,
            IReadOnlyCollection<Resolution> resolutions,
            int barRollingWindow)
        {
            Precondition.NotEmpty(symbols, nameof(symbols));
            Precondition.NotEmpty(resolutions, nameof(resolutions));
            Precondition.PositiveInt32(barRollingWindow, nameof(barRollingWindow));

            var loggingAdapter = new SerilogLogger(logLevel);
            loggingAdapter.Debug(NautilusService.Core, $"Starting {nameof(NautilusData)} builder...");
            VersionChecker.Run(loggingAdapter, "NautilusData - Financial Market Data Service");

            var clock = new Clock(DateTimeZone.Utc);
            var guidFactory = new GuidFactory();
            var container = new ComponentryContainer(
                clock,
                guidFactory,
                new LoggerFactory(loggingAdapter));

            var messagingAdapter = MessagingServiceFactory.Create(container, new FakeMessageStore());
            var scheduler = new Scheduler(container);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true");
            var barRepository = new RedisBarRepository(redisConnection);
            var instrumentRepository = new RedisInstrumentRepository(redisConnection);
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
                container,
                messagingAdapter,
                fixClient,
                fixGateway,
                barRepository,
                instrumentRepository,
                symbols,
                resolutions,
                barRollingWindow,
                fixConfig.UpdateInstruments);

            dataServiceAddresses.Add(ServiceAddress.Scheduler, scheduler.Endpoint);
            var switchboard = Switchboard.Create(dataServiceAddresses);

            var systemController = new SystemController(
                container,
                messagingAdapter,
                switchboard);

            return new NautilusData(
                container,
                messagingAdapter,
                systemController,
                fixClient);
        }

        private static IFixClient GetFixClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
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
