//--------------------------------------------------------------------------------------------------
// <copyright file="DataCollectionManager.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Documents;
    using Nautilus.Data.Messages.Events;
    using Nautilus.Data.Messages.Jobs;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Quartz;

    /// <summary>
    /// The manager class which orchestrates data collection operations.
    /// </summary>
    public class DataCollectionManager : ComponentBusConnectedBase
    {
        private readonly IEndpoint barPublisher;
        private readonly List<Resolution> resolutionsPersisting;
        private readonly int barRollingWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionManager"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="barPublisher">The bar publisher.</param>
        /// <param name="resolutionsToPersist">The bar resolutions to persist (with a period of 1).</param>
        /// <param name="barRollingWindow">The rolling window of persisted bar data (days).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the barRollingWindow is not positive (> 0).</exception>
        public DataCollectionManager(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IEndpoint barPublisher,
            IEnumerable<Resolution> resolutionsToPersist,
            int barRollingWindow)
            : base(
                NautilusService.Data,
                container,
                messagingAdapter)
        {
            Precondition.PositiveInt32(barRollingWindow, nameof(barRollingWindow));

            this.barPublisher = barPublisher;
            this.resolutionsPersisting = new List<Resolution>(resolutionsToPersist);
            this.barRollingWindow = barRollingWindow;

            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<CollectData<BarType>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<BarClosed>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<BarDataFrame>>(this.OnMessage);
            this.RegisterHandler<DataPersisted<BarType>>(this.OnMessage);
            this.RegisterHandler<DataCollected<BarType>>(this.OnMessage);
            this.RegisterHandler<TrimBarDataJob>(this.OnMessage);
        }

        /// <summary>
        /// Start method called when the <see cref="SystemStart"/> message is received.
        /// </summary>
        public override void Start()
        {
            this.CreateTrimBarDataJob();
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            this.Send(DataServiceAddress.BarAggregationController, message);
        }

        private void OnMessage(CollectData<BarType> message)
        {
            // Do nothing.
        }

        private void OnMessage(DataDelivery<BarClosed> message)
        {
            this.barPublisher.Send(message.Data);
            this.Send(DataServiceAddress.DatabaseTaskManager, message);
        }

        private void OnMessage(DataDelivery<BarDataFrame> message)
        {
            // Not implemented.
        }

        private void OnMessage(DataPersisted<BarType> message)
        {
            // Not implemented.
        }

        private void OnMessage(DataCollected<BarType> message)
        {
            // Not implemented.
        }

        private void OnMessage(TrimBarDataJob message)
        {
            var trimCommand = new TrimBarData(
                this.resolutionsPersisting,
                this.barRollingWindow,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DataServiceAddress.DatabaseTaskManager, trimCommand);
            this.Log.Information($"Received {nameof(TrimBarDataJob)}.");
        }

        private void CreateTrimBarDataJob()
        {
            var schedule = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 00, 01)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var jobKey = new JobKey("trim_bar_data", "data_management");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                new TrimBarDataJob(),
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(ServiceAddress.Scheduler, createJob);
            this.Log.Information($"Created {nameof(TrimBarDataJob)} for Sundays 00:01 (UTC).");
        }
    }
}
