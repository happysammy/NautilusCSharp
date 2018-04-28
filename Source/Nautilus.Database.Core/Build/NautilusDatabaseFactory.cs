//--------------------------------------------------------------
// <copyright file="NautilusDatabaseFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using Akka.Actor;
using NautechSystems.CSharp.Validation;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace Nautilus.Database.Core.Build
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Configuration;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The builder for the NautilusDB database infrastructure.
    /// </summary>
    public static class NautilusDatabaseFactory
    {
        /// <summary>
        /// Builds the NautilusDB database infrastructure and returns an <see cref="IActorRef"/>
        /// address to the <see cref="DatabaseTaskManager"/>.
        /// </summary>
        /// <param name="logger">The database logger.</param>
        /// <param name="collectionConfig">The collection configuration.</param>
        /// <param name="marketDataRepository">The database market data repo.</param>
        /// <param name="economicNewsEventRepository">The database economic news event repo.</param>
        /// <param name="barDataProvider">The market data provider.</param>
        /// <returns>A built Nautilus database.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public static NautilusDatabase Create(
            ILoggingAdapter logger,
            JObject collectionConfig,
            IMarketDataRepository marketDataRepository,
            IEconomicNewsEventRepository<EconomicNewsEvent> economicNewsEventRepository,
            IBarDataProvider barDataProvider)
        {
            Validate.NotNull(logger, nameof(logger));
            Validate.NotNull(collectionConfig, nameof(collectionConfig));
            Validate.NotNull(marketDataRepository, nameof(marketDataRepository));
            Validate.NotNull(economicNewsEventRepository, nameof(economicNewsEventRepository));
            Validate.NotNull(barDataProvider, nameof(barDataProvider));

            logger.Information($"Starting {nameof(NautilusDB)} builder...");
            StartupVersionChecker.Run(logger);

            var clock = new Clock(DateTimeZone.Utc);
            var container = new ComponentryContainer(clock, logger);
            var collectionSchedule = DataCollectionScheduleFactory.Create(clock.TimeNow(), collectionConfig);
            var actorSystem = ActorSystem.Create(nameof(NautilusDB));

            var databaseTaskActorRef = actorSystem.ActorOf(Props.Create(
                () => new DatabaseTaskManager(
                    container,
                    marketDataRepository,
                    economicNewsEventRepository)));

            var dataCollectionActorRef = actorSystem.ActorOf(Props.Create(
                () => new DataCollectionManager(
                    container,
                    actorSystem.Scheduler,
                    databaseTaskActorRef,
                    collectionSchedule,
                    barDataProvider)));

            var actorReferences = new ActorReferences(
                databaseTaskActorRef,
                dataCollectionActorRef);

            return new NautilusDatabase(
                container,
                actorSystem,
                actorReferences);
        }
    }
}
