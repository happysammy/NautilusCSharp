//--------------------------------------------------------------
// <copyright file="DataCollectionManager.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Core.Collectors;
    using Nautilus.Database.Core.Integrity.Checkers;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Messages;
    using Nautilus.Database.Core.Messages.Commands;
    using Nautilus.Database.Core.Messages.Events;
    using Nautilus.Database.Core.Orchestration;
    using Nautilus.DomainModel.ValueObjects;
    using NautilusDB.Messaging.Queries;

    /// <summary>
    /// The manager class which contains the separate data collector types and orchestrates their
    /// operations.
    /// </summary>
    public class DataCollectionManager : ActorComponentBase
    {
        private readonly IScheduler scheduler;
        private readonly IActorRef databaseTaskActorRef;
        private readonly IBarDataProvider barDataProvider;
        private readonly Dictionary<BarSpecification, IActorRef> marketDataCollectors;
        private readonly EconomicNewsEventCollector newsEventCollector;
        private readonly DataCollectionSchedule collectionSchedule;

        private Dictionary<BarSpecification, bool> collectionJobsRoster;

        // TODO: Temporary variable to handle Dukascopy CSV initial collection from date.
        private bool isInitialCollection = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="databaseTaskActorRef">The database task actor ref.</param>
        /// <param name="collectionSchedule">The collection schedule.</param>
        /// <param name="barDataProvider">The market data provider.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataCollectionManager(
            ComponentryContainer container,
            IScheduler scheduler,
            IActorRef databaseTaskActorRef,
            DataCollectionSchedule collectionSchedule,
            IBarDataProvider barDataProvider)
            : base(container, nameof(DataCollectionManager))
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(scheduler, nameof(scheduler));
            Validate.NotNull(databaseTaskActorRef, nameof(databaseTaskActorRef));
            Validate.NotNull(collectionSchedule, nameof(collectionSchedule));

            this.scheduler = scheduler;
            this.databaseTaskActorRef = databaseTaskActorRef;
            this.barDataProvider = barDataProvider;
            this.marketDataCollectors = new Dictionary<BarSpecification, IActorRef>();
            this.newsEventCollector = new EconomicNewsEventCollector();
            this.collectionSchedule = collectionSchedule;
            this.collectionJobsRoster = new Dictionary<BarSpecification, bool>();

            this.Receive<StartSystem>(msg => this.OnMessage(msg));
            this.Receive<CollectData>(msg => this.OnMessage(msg));
            this.Receive<MarketDataDelivery>(msg => this.OnMessage(msg));
            this.Receive<MarketDataPersisted>(msg => this.OnMessage(msg));
            this.Receive<AllDataCollected>(msg => this.OnMessage(msg));
        }

        private void OnMessage(StartSystem message)
        {
            Debug.NotNull(message, nameof(message));

            // Provides system time to instantiate other components.
            Thread.Sleep(300);

            this.InitializeMarketDataCollectors();
        }

        private void OnMessage(CollectData message)
        {
            Debug.NotNull(message, nameof(message));

            if (message.DataType == DataType.Bar)
            {
                this.collectionJobsRoster = new Dictionary<BarSpecification, bool>();

                foreach (var collector in this.marketDataCollectors.Keys)
                {
                    this.collectionJobsRoster.Add(collector, false);
                }

                var timeNow = this.Clock.TimeNow();
                this.collectionSchedule.UpdateLastCollectedTime(this.Clock.TimeNow());
                this.Log(
                    LogLevel.Information,
                    $"{this.Component} updated last collection time to {timeNow.ToIsoString()}.");

                this.CollectMarketData();
            }
        }

        // TODO: Refactor this.
        private void OnMessage(MarketDataDelivery message)
        {
            Debug.NotNull(message, nameof(message));

            if (this.barDataProvider.IsBarDataCheckOn)
            {
                var result = BarDataChecker.CheckBars(
                    message.MarketData.BarSpecification,
                    message.MarketData.Bars);

                if (result.IsSuccess)
                {
                    if (result.Value.Count == 0)
                    {
                        this.Log(LogLevel.Information, result.Message);
                    }

                    if (result.Value.Count > 0)
                    {
                        this.Log(LogLevel.Warning, result.Message);

                        foreach (var anomaly in result.Value)
                        {
                            this.Log(LogLevel.Warning, anomaly);
                        }
                    }
                }

                if (result.IsFailure)
                {
                    this.Log(LogLevel.Warning, result.FullMessage);
                }
            }

            this.databaseTaskActorRef.Forward(message);
        }

        private void OnMessage(MarketDataPersisted message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.DictionaryContainsKey(message.BarSpecification, nameof(message.BarSpecification), this.marketDataCollectors);

            this.marketDataCollectors[message.BarSpecification].Tell(message);
        }

        private void OnMessage(AllDataCollected message)
        {
            Debug.NotNull(message, nameof(message));

            this.collectionJobsRoster[message.BarSpecification] = true;

            if (this.collectionJobsRoster.All(c => c.Value == true))
            {
                this.Logger.Information(
                    $"{this.ComponentName} data collection completed for {this.marketDataCollectors.Count} collectors...");

                this.ScheduleNextCollection();

                return;
            }

            this.CollectMarketData();
        }

        private void InitializeMarketDataCollectors()
        {
            this.Logger.Information($"{this.ComponentName} initializing all market data collectors...");

            foreach (var barSpec in this.barDataProvider.BarSpecifications)
            {
                var dataReader = new CsvBarDataReader(
                    barSpec,
                    this.barDataProvider);

                var collectorRef = Context.ActorOf(Props.Create(
                    () => new MarketDataCollector(
                        this.Container,
                        dataReader,
                        this.collectionSchedule)));

                this.marketDataCollectors.Add(barSpec, collectorRef);

                var message = new DataStatusRequest(barSpec, Guid.NewGuid(), this.Clock.TimeNow());

                var dataStatusTask = Task.Run(() => this.databaseTaskActorRef.Ask<DataStatusResponse>(message, TimeSpan.FromSeconds(10), CancellationToken.None));

                this.Logger.Debug($"{this.Component} waiting for DataStatusResponse for {barSpec}...");
                dataStatusTask.Wait();

                if (dataStatusTask.IsCompletedSuccessfully)
                {
                    this.Logger.Debug($"{this.Component} received a DataStatusResponse for {barSpec} and sending to collector");

                    collectorRef.Tell(dataStatusTask.Result, this.Self);
                }
            }

            // Allow above actors to initialize.
            Task.Delay(2000);

            if (this.barDataProvider.InitialFromDateSpecified)
            {
                this.barDataProvider.InitialFromDateConfigCsv(
                    this.GetCurrencyPairsStringList(),
                    this.collectionSchedule.NextCollectionTime);
            }

            this.collectionJobsRoster = new Dictionary<BarSpecification, bool>();

            this.Self.Tell(new CollectData(
                DataType.Bar,
                Guid.NewGuid(),
                this.Clock.TimeNow()));
        }

        private void CollectMarketData()
        {
            var nextCollectorOffTheRank = this.collectionJobsRoster.FirstOrDefault(c => c.Value == false);

            this.marketDataCollectors[nextCollectorOffTheRank.Key].Tell(new CollectData(
                DataType.Bar,
                Guid.NewGuid(),
                this.Clock.TimeNow()));

            this.Logger.Information(
                $"{this.ComponentName} initiating market data collection for {nextCollectorOffTheRank.Key}...");
        }

        private void ScheduleNextCollection()
        {
            if (!this.isInitialCollection)
            {
                this.barDataProvider.UpdateConfigCsv(
                    this.GetCurrencyPairsStringList(),
                    this.collectionSchedule.NextCollectionTime - Duration.FromDays(6),
                    this.collectionSchedule.NextCollectionTime - Duration.FromDays(1));
            }

            this.isInitialCollection = false;

            var timeFromCollection = (this.collectionSchedule.NextCollectionTime - this.Clock.TimeNow())
                .ToTimeSpan();

            this.scheduler.ScheduleTellOnce(
                timeFromCollection,
                this.Self,
                new CollectData(
                    DataType.Bar,
                    Guid.NewGuid(),
                    this.Clock.TimeNow()),
                this.Self);

            this.Logger.Information(
                $"{this.ComponentName} scheduled next collection time for {this.collectionSchedule.NextCollectionTime.ToIsoString()}");
        }

        private IReadOnlyList<string> GetCurrencyPairsStringList()
        {
            return this.marketDataCollectors.Keys
                .ToList()
                .Select(k => k.Symbol)
                .Distinct()
                .ToList()
                .AsReadOnly();
        }
    }
}