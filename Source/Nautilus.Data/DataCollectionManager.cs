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
    using Nautilus.Data.Messages.Jobs;
    using Nautilus.Data.Types;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;
    using Quartz;

    /// <summary>
    /// The manager class which orchestrates data collection operations.
    /// </summary>
    public class DataCollectionManager : ComponentBusConnectedBase
    {
        private readonly IEnumerable<BarSpecification> barSpecifications;
        private readonly int barRollingWindowDays;

        private bool isTrimJobActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCollectionManager"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="barPublisher">The bar publisher.</param>
        /// <param name="barSpecifications">The bar specifications to collect and persist.</param>
        /// <param name="barRollingWindowDays">The rolling window of persisted bar data (days).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the barRollingWindow is not positive (> 0).</exception>
        public DataCollectionManager(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IEndpoint barPublisher,
            IEnumerable<BarSpecification> barSpecifications,
            int barRollingWindowDays)
            : base(
                NautilusService.Data,
                container,
                messagingAdapter)
        {
            Precondition.PositiveInt32(barRollingWindowDays, nameof(barRollingWindowDays));

            this.barSpecifications = barSpecifications;
            this.barRollingWindowDays = barRollingWindowDays;
            this.isTrimJobActive = false;

            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<CollectData<BarType>>(this.OnMessage);
            this.RegisterHandler<DataDelivery<BarDataFrame>>(this.OnMessage);
            this.RegisterHandler<DataPersisted<BarType>>(this.OnMessage);
            this.RegisterHandler<DataCollected<BarType>>(this.OnMessage);
            this.RegisterHandler<TrimBarDataJob>(this.OnMessage);
        }

        /// <summary>
        /// Actions to be performed when the component is started.
        /// </summary>
        public override void Start()
        {
            if (this.isTrimJobActive)
            {
                return; // No actions to perform.
            }

            this.CreateTrimBarDataJob();
            this.isTrimJobActive = true;
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            this.Send(DataServiceAddress.BarAggregationController, message);
        }

        private void OnMessage(CollectData<BarType> message)
        {
            // Do nothing.
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
                this.barSpecifications,
                this.barRollingWindowDays,
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
