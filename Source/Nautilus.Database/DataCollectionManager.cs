//--------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Database.Collectors;
    using Nautilus.Database.Enums;
    using Nautilus.Database.Integrity.Checkers;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Documents;
    using Nautilus.Database.Messages.Events;
    using Nautilus.Database.Orchestration;
    using Nautilus.Database.Readers;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The manager class which contains the separate data collector types and orchestrates their
    /// operations.
    /// </summary>
    public class DataCollectionManager : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IBarDataProvider barDataProvider;
        private readonly Dictionary<BarType, IActorRef> marketDataCollectors;
        private readonly EconomicEventCollector eventCollector;
        private readonly DataCollectionSchedule collectionSchedule;

        private Dictionary<BarType, bool> collectionJobsRoster;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionManager"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="collectionSchedule">The collection schedule.</param>
        /// <param name="barDataProvider">The market data provider.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public DataCollectionManager(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            DataCollectionSchedule collectionSchedule,
            IBarDataProvider barDataProvider)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(DataCollectionManager)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(collectionSchedule, nameof(collectionSchedule));
            Validate.NotNull(barDataProvider, nameof(barDataProvider));

            this.barDataProvider = barDataProvider;
            this.marketDataCollectors = new Dictionary<BarType, IActorRef>();
            this.eventCollector = new EconomicEventCollector();
            this.collectionSchedule = collectionSchedule;
            this.collectionJobsRoster = new Dictionary<BarType, bool>();
            this.storedContainer = container;

            // Command messages
            this.Receive<StartSystem>(msg => this.OnMessage(msg));
            this.Receive<CollectData<BarType>>(msg => this.OnMessage(msg));

            // Document messages
            this.Receive<DataDelivery<BarClosed>>(msg => this.OnMessage(msg));
            this.Receive<DataDelivery<BarDataFrame>>(msg => this.OnMessage(msg));
            this.Receive<DataPersisted<BarType>>(msg => this.OnMessage(msg));
            this.Receive<DataCollected<BarType>>(msg => this.OnMessage(msg));
        }

        private void OnMessage(StartSystem message)
        {
            Debug.NotNull(message, nameof(message));

            // Provides system time to instantiate other components.
            Thread.Sleep(300);

            this.InitializeMarketDataCollectors();
        }

        private void OnMessage(CollectData<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            this.collectionJobsRoster = new Dictionary<BarType, bool>();

            foreach (var collector in this.marketDataCollectors.Keys)
            {
                this.collectionJobsRoster.Add(collector, false);
            }

            var timeNow = this.TimeNow();
            this.collectionSchedule.UpdateLastCollectedTime(this.TimeNow());
            this.Log.Information(
                $"Updated last collection time to {timeNow.ToIsoString()}.");

            this.CollectMarketData();
        }

        private void OnMessage(DataDelivery<BarClosed> message)
        {
            Debug.NotNull(message, nameof(message));

            this.Send(DatabaseService.TaskManager, message);
        }

        // TODO: Refactor this.
        private void OnMessage(DataDelivery<BarDataFrame> message)
        {
            Debug.NotNull(message, nameof(message));

            if (this.barDataProvider.IsBarDataCheckOn)
            {
                var result = BarDataChecker.CheckBars(
                    message.Data.BarType,
                    message.Data.Bars);

                if (result.IsSuccess)
                {
                    if (result.Value.Count == 0)
                    {
                        this.Log.Information(result.Message);
                    }

                    if (result.Value.Count > 0)
                    {
                        this.Log.Warning(result.Message);

                        foreach (var anomaly in result.Value)
                        {
                            this.Log.Warning(anomaly);
                        }
                    }
                }

                if (result.IsFailure)
                {
                    this.Log.Warning(result.FullMessage);
                }
            }

            this.Send(DatabaseService.TaskManager, message);
        }

        private void OnMessage(DataPersisted<BarType> message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.DictionaryContainsKey(message.DataType, nameof(message.DataType.Specification), this.marketDataCollectors);

            this.marketDataCollectors[message.DataType].Tell(message);
        }

        private void OnMessage(DataCollected<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            this.collectionJobsRoster[message.DataType] = true;

            if (this.collectionJobsRoster.All(c => c.Value))
            {
                this.Log.Information(
                    $"Data collection completed for {this.marketDataCollectors.Count} collectors...");

                this.ScheduleNextCollection();

                return;
            }

            this.CollectMarketData();
        }

        private void InitializeMarketDataCollectors()
        {
            this.Log.Information($"Initializing all market data collectors...");

            foreach (var barType in this.barDataProvider.SymbolBarDatas)
            {
                var dataReader = new CsvBarDataReader(
                    barType,
                    this.barDataProvider,
                    5);

                var collectorRef = Context.ActorOf(Props.Create(
                    () => new BarDataCollector(
                        this.storedContainer,
                        this.GetMessagingAdapter(),
                        dataReader,
                        this.collectionSchedule)));

                this.marketDataCollectors.Add(barType, collectorRef);
            }

            // Allow above actors to initialize.
            Task.Delay(2000);

            this.collectionJobsRoster = new Dictionary<BarType, bool>();

            this.Self.Tell(new CollectData<BarType>(
                this.NewGuid(),
                this.TimeNow()));
        }

        private void CollectMarketData()
        {
            var nextCollectorOffTheRank = this.collectionJobsRoster.FirstOrDefault(c => c.Value == false);

            this.marketDataCollectors[nextCollectorOffTheRank.Key].Tell(new CollectData<BarType>(
                this.NewGuid(),
                this.TimeNow()));

            this.Log.Information(
                $"Initiating bar data collection for {nextCollectorOffTheRank.Key}...");
        }

        private void ScheduleNextCollection()
        {
            var timeFromCollection = (this.collectionSchedule.NextCollectionTime - this.TimeNow())
                .ToTimeSpan();

//            this.scheduler.ScheduleTellOnce(
//                timeFromCollection,
//                this.Self,
//                new CollectData<BarType>(
//                    this.NewGuid(),
//                    this.TimeNow()),
//                this.Self);

            this.Log.Information(
                $"Scheduled next collection time for {this.collectionSchedule.NextCollectionTime.ToIsoString()}");
        }

        private IReadOnlyList<string> GetCurrencyPairsStringList()
        {
            return this.marketDataCollectors.Keys
                .ToList()
                .Select(k => k.Symbol.Value)
                .Distinct()
                .ToList()
                .AsReadOnly();
        }
    }
}
