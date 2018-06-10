//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataCollector.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Collectors
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Messages;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Events;
    using Nautilus.Database.Orchestration;
    using System;
    using Messages.Queries;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Database.Enums;
    using Nautilus.DomainModel.Factories;
    using NodaTime;

    /// <summary>
    /// Represents a market data collector.
    /// </summary>
    public class MarketDataCollector : ActorComponentBase
    {
        private readonly IBarDataReader dataReader;
        private readonly DataCollectionSchedule collectionSchedule;
        private Option<ZonedDateTime?> lastPersistedBarTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataCollector"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="dataReader">The bar data reader.</param>
        /// <param name="collectionSchedule">The collection schedule.</param>
        public MarketDataCollector(
            DatabaseSetupContainer container,
            IMessagingAdapter messagingAdapter,
            IBarDataReader dataReader,
            DataCollectionSchedule collectionSchedule)
            : base(
                DatabaseService.DataCollection,
                LabelFactory.Component($"{nameof(MarketDataCollector)}-{dataReader.SymbolBarSpec}"),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(dataReader, nameof(dataReader));
            Validate.NotNull(collectionSchedule, nameof(collectionSchedule));

            this.dataReader = dataReader;
            this.collectionSchedule = collectionSchedule;

            this.Receive<StartSystem>(msg => this.OnMessage(msg));
            this.Receive<CollectData>(msg => this.OnMessage(msg));
            this.Receive<DataStatusResponse>(msg => this.OnMessage(msg));
            this.Receive<MarketDataPersisted>(msg => this.OnMessage(msg));
        }

        private void OnMessage(StartSystem message)
        {
            Debug.NotNull(message, nameof(message));
        }

        private void OnMessage(CollectData message)
        {
            Debug.NotNull(message, nameof(message));

            if (this.dataReader.GetAllCsvFilesOrdered().IsFailure)
            {
                this.Log.Warning($"No csv files found for {this.dataReader.SymbolBarSpec}");

                Context.Parent.Tell(new AllDataCollected(this.dataReader.SymbolBarSpec, Guid.NewGuid(), this.TimeNow()), this.Self);

                return;
            }

            foreach (var csv in this.dataReader.GetAllCsvFilesOrdered().Value)
            {
                // TODO: Changed to just read all bars to get all data in.
                var csvQuery = this.dataReader.GetAllBars(csv);

                if (csvQuery.IsSuccess)
                {
//                    // TODO: Temporary work around of bottleneck
//                    this.Logger.Information($"{this.ComponentName} delaying 30s to allow repository to persist...");
//                    Thread.Sleep(TimeSpan.FromSeconds(30));

                    Context.Parent.Tell(
                        new MarketDataDelivery(
                            csvQuery.Value,
                            Guid.NewGuid(),
                            this.TimeNow()),
                        this.Self);

                    this.collectionSchedule.UpdateLastCollectedTime(this.TimeNow());

                    //this.Log(LogLevel.Debug, $"{this.Component} collected {csvQuery.Value.Bars.Length} {csvQuery.Value.BarSpecification} bars");
                    this.Log.Debug($"Updated last collected time to {this.collectionSchedule.LastCollectedTime.Value.ToIsoString()}");
                }

                if (csvQuery.IsFailure)
                {
                    this.Log.Warning(csvQuery.Message);
                }
            }

            Context.Parent.Tell(new AllDataCollected(this.dataReader.SymbolBarSpec, Guid.NewGuid(), this.TimeNow()), this.Self);
        }

        private void OnMessage(DataStatusResponse message)
        {
            Debug.NotNull(message, nameof(message));

            if (message.LastTimestampQueryResult.IsSuccess)
            {
                this.lastPersistedBarTime = message.LastTimestampQueryResult.Value;

                this.Log.Debug(
                    $"From {nameof(DataStatusResponse)} " +
                    $"updated last persisted bar timestamp to {this.lastPersistedBarTime.Value.ToIsoString()}");

                return;
            }

            this.Log.Debug(
                $"From {nameof(DataStatusResponse)} " +
                $"no persisted bar timestamp");
        }

        private void OnMessage(MarketDataPersisted message)
        {
            Debug.NotNull(message, nameof(message));

            this.lastPersistedBarTime = message.LastBarTime;
        }
    }
}
