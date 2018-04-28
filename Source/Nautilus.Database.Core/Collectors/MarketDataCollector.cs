//--------------------------------------------------------------
// <copyright file="MarketDataCollector.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using NautechSystems.CSharp;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Extensions;
using NodaTime;

namespace Nautilus.Database.Core.Collectors
{
    using Nautilus.Common.Componentry;
    using Nautilus.Database.Core.Interfaces;
    using Nautilus.Database.Core.Orchestration;

    public class MarketDataCollector : ActorComponentBase
    {
        private readonly IBarDataReader dataReader;
        private readonly DataCollectionSchedule collectionSchedule;
        private Option<ZonedDateTime?> lastPersistedBarTime;

        public MarketDataCollector(
            ComponentryContainer container,
            IBarDataReader dataReader,
            DataCollectionSchedule collectionSchedule)
            : base(container, $"{nameof(MarketDataCollector)}-{dataReader.BarSpecification}")
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

            this.LogMsgReceipt(message);
        }

        private void OnMessage(CollectData message)
        {
            Debug.NotNull(message, nameof(message));

            this.LogMsgReceipt(message);

            if (this.dataReader.GetAllCsvFilesOrdered().IsFailure)
            {
                this.Logger.Warning($"{this.ComponentName} no csv files found for {this.dataReader.BarSpecification}");

                Context.Parent.Tell(new AllDataCollected(this.dataReader.BarSpecification, Guid.NewGuid(), this.Clock.TimeNow()), this.Self);

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
                            this.Clock.TimeNow()),
                        this.Self);

                    this.collectionSchedule.UpdateLastCollectedTime(this.Clock.TimeNow());

                    this.Logger.Debug($"{this.ComponentName} collected {csvQuery.Value.Bars.Length} {csvQuery.Value.BarSpecification} bars");
                    this.Logger.Debug($"{this.ComponentName} updated last collected time to {this.collectionSchedule.LastCollectedTime.Value.ToIsoString()}");
                }

                if (csvQuery.IsFailure)
                {
                    this.Logger.Warning(csvQuery.Message);
                }
            }

            Context.Parent.Tell(new AllDataCollected(this.dataReader.BarSpecification, Guid.NewGuid(), this.Clock.TimeNow()), this.Self);
        }

        private void OnMessage(DataStatusResponse message)
        {
            Debug.NotNull(message, nameof(message));

            this.LogMsgReceipt(message);

            if (message.LastTimestampQueryResult.IsSuccess)
            {
                this.lastPersistedBarTime = message.LastTimestampQueryResult.Value;

                this.Logger.Debug(
                    $"{this.ComponentName} from {nameof(DataStatusResponse)} " +
                    $"updated last persisted bar timestamp to {this.lastPersistedBarTime.Value.ToIsoString()}");

                return;
            }

            this.Logger.Debug(
                $"{this.ComponentName} from {nameof(DataStatusResponse)} " +
                $"no persisted bar timestamp");
        }

        private void OnMessage(MarketDataPersisted message)
        {
            Debug.NotNull(message, nameof(message));

            this.LogMsgReceipt(message);

            this.lastPersistedBarTime = message.LastBarTime;
        }
    }
}