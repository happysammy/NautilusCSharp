﻿//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataCollector.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Collectors
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Interfaces;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Documents;
    using Nautilus.Data.Orchestration;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a market data collector.
    /// </summary>
    public class BarDataCollector : ComponentBase
    {
        private readonly IBarDataReader dataReader;
        private readonly DataCollectionSchedule collectionSchedule;
        private OptionVal<ZonedDateTime> lastPersistedBarTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataCollector"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="dataReader">The bar data reader.</param>
        /// <param name="collectionSchedule">The collection schedule.</param>
        public BarDataCollector(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IBarDataReader dataReader,
            DataCollectionSchedule collectionSchedule)
            : base(
                NautilusService.Data,
                LabelFactory.Create($"{nameof(BarDataCollector)}-{dataReader.BarType}"),
                container)
        {
            this.dataReader = dataReader;
            this.collectionSchedule = collectionSchedule;
        }

        private void OnMessage(CollectData<BarType> message)
        {
            if (this.dataReader.GetAllCsvFilesOrdered().IsFailure)
            {
                this.Log.Warning($"No csv files found for {this.dataReader.BarType}");

                // Context.Parent.Tell(new DataCollected<BarType>(this.dataReader.BarType, Guid.NewGuid(), this.TimeNow()), this.Self);
                return;
            }

            foreach (var csv in this.dataReader.GetAllCsvFilesOrdered().Value)
            {
                // Changed to just read all bars to get all data in.
                var csvQuery = this.dataReader.GetAllBars(csv);

                if (csvQuery.IsSuccess)
                {
// Context.Parent.Tell(
//                        new DataDelivery<BarDataFrame>(
//                            csvQuery.Value,
//                            Guid.NewGuid(),
//                            this.TimeNow()),
//                        this.Self);
                    this.collectionSchedule.UpdateLastCollectedTime(this.TimeNow());

                    this.Log.Debug($"Updated last collected time to {this.collectionSchedule.LastCollectedTime.Value.ToIsoString()}");
                }

                if (csvQuery.IsFailure)
                {
                    this.Log.Warning(csvQuery.Message);
                }
            }

            // Context.Parent.Tell(new DataCollected<BarType>(this.dataReader.BarType, Guid.NewGuid(), this.TimeNow()), this.Self);
        }

        private void OnMessage(DataStatusResponse<ZonedDateTime> message)
        {
            if (message.LastTimestampQuery.IsSuccess)
            {
                this.lastPersistedBarTime = message.LastTimestampQuery.Value;

                this.Log.Debug(
                    $"From {nameof(DataStatusResponse<ZonedDateTime>)} " +
                    $"updated last persisted bar timestamp to {this.lastPersistedBarTime.Value.ToIsoString()}");

                return;
            }

            this.Log.Debug(
                $"From {nameof(DataStatusResponse<ZonedDateTime>)} " +
                $"no persisted bar timestamp");
        }

        private void OnMessage(DataPersisted<BarType> message)
        {
            this.lastPersistedBarTime = message.LastDataTime;
        }
    }
}
