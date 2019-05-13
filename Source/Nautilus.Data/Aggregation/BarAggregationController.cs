//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationController.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregation
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Commands;
    using Nautilus.Data.Messages.Jobs;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Provides a bar aggregation controller to manage bar aggregators for many symbols.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregationController : ComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IEndpoint barPublisher;
        private readonly Dictionary<Symbol, IEndpoint> barAggregators;
        private readonly Dictionary<BarSpecification, KeyValuePair<JobKey, TriggerKey>> barJobs;
        private readonly Dictionary<Duration, List<BarSpecification>> barTriggers;
        private readonly Dictionary<Duration, ITrigger> triggers;
        private readonly Dictionary<Duration, int> triggerCounts;

        private bool isMarketOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="barPublisher">The bar publisher endpoint.</param>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IEndpoint barPublisher)
            : base(
            NautilusService.Data,
            container,
            messagingAdapter)
        {
            this.storedContainer = container;
            this.barPublisher = barPublisher;
            this.barAggregators = new Dictionary<Symbol, IEndpoint>();
            this.barJobs = new Dictionary<BarSpecification, KeyValuePair<JobKey, TriggerKey>>();
            this.barTriggers = new Dictionary<Duration, List<BarSpecification>>();
            this.triggers = new Dictionary<Duration, ITrigger>();
            this.triggerCounts = new Dictionary<Duration, int>();

            this.isMarketOpen = this.IsFxMarketOpen();

            this.RegisterHandler<Subscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<BarType>>(this.OnMessage);
            this.RegisterHandler<Tick>(this.OnMessage);
            this.RegisterHandler<BarJob>(this.OnMessage);
            this.RegisterHandler<MarketStatusJob>(this.OnMessage);
            this.RegisterHandler<(BarType, Bar)>(this.OnMessage);
        }

        /// <summary>
        /// Start method called when the <see cref="Common.Messages.Commands.Start"/> message is received.
        /// </summary>
        public override void Start()
        {
            this.Log.Information($"Started at {this.StartTime}.");
            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();
        }

        private static IScheduleBuilder CreateBarJobSchedule(BarSpecification barSpec)
        {
            var scheduleBuilder = SimpleScheduleBuilder
                .Create()
                .RepeatForever()
                .WithMisfireHandlingInstructionFireNow();

            switch (barSpec.Resolution)
            {
                case Resolution.SECOND:
                    scheduleBuilder.WithIntervalInSeconds(barSpec.Period);
                    break;
                case Resolution.MINUTE:
                    scheduleBuilder.WithIntervalInMinutes(barSpec.Period);
                    break;
                case Resolution.HOUR:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period);
                    break;
                case Resolution.DAY:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period * 24);
                    break;
                case Resolution.TICK:
                    throw new InvalidOperationException("Cannot schedule tick bars.");
                default: throw new InvalidOperationException("Bar resolution not recognised.");
            }

            return scheduleBuilder;
        }

        private ITrigger CreateBarJobTrigger(BarSpecification barSpec)
        {
            Debug.KeyNotIn(barSpec.Duration, this.triggers, nameof(barSpec.Duration), nameof(this.triggers));

            var duration = barSpec.Duration;

            return TriggerBuilder
                .Create()
                .StartAt(this.TimeNow().Ceiling(duration).ToDateTimeUtc())
                .WithIdentity($"{barSpec.Period}-{barSpec.Resolution.ToString().ToLower()}", "bar_aggregation")
                .WithSchedule(CreateBarJobSchedule(barSpec))
                .Build();
        }

        private void CreateMarketOpenedJob()
        {
            var schedule = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 21, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var jobKey = new JobKey("market_opened", "bar_aggregation");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                new MarketStatusJob(true),
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(ServiceAddress.Scheduling, createJob);
            this.Log.Information("Created MarketStatusJob for market open Sundays 21:00 (UTC).");
        }

        private void CreateMarketClosedJob()
        {
            var schedule = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Saturday, 20, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var jobKey = new JobKey("market_closed", "bar_aggregation");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .WithSchedule(schedule)
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                new MarketStatusJob(false),
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(ServiceAddress.Scheduling, createJob);
            this.Log.Information("Created MarketStatusJob for market close Saturdays 20:00 (UTC).");
        }

        private bool IsFxMarketOpen()
        {
            // Market open Sun 21:00 UTC (Start of Sydney session)
            // Market close Sat 20:00 UTC (End of New York session)
            return ZonedDateTimeExtensions.IsOutsideWeeklyInterval(
                this.TimeNow(),
                (IsoDayOfWeek.Saturday, 20, 00),
                (IsoDayOfWeek.Sunday, 21, 00));
        }

        private void OnMessage(Subscribe<BarType> message)
        {
            this.Log.Debug($"Received {message}");

            var symbol = message.DataType.Symbol;
            var barSpec = message.DataType.Specification;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                var barAggregator = new BarAggregator(
                        this.storedContainer,
                        this.Endpoint,
                        symbol,
                        this.isMarketOpen).Endpoint;

                this.barAggregators.Add(message.DataType.Symbol, barAggregator);
            }

            this.barAggregators[symbol].Send(message);

            var duration = barSpec.Duration;

            if (!this.triggers.ContainsKey(duration))
            {
                var trigger = this.CreateBarJobTrigger(barSpec);
                this.triggers.Add(duration, trigger);
            }

            if (!this.barTriggers.ContainsKey(duration))
            {
                this.barTriggers.Add(duration, new List<BarSpecification>());
            }

            if (!this.barTriggers[duration].Contains(barSpec))
            {
                this.barTriggers[duration].Add(barSpec);

                var barJob = new BarJob(barSpec);

                var createJob = new CreateJob(
                    this.Endpoint,
                    barJob,
                    barJob.Key,
                    this.triggers[duration],
                    this.NewGuid(),
                    this.TimeNow());

                this.barJobs.Add(
                    barSpec, new KeyValuePair<JobKey, TriggerKey>(createJob.JobKey, createJob.Trigger.Key));

                this.Send(ServiceAddress.Scheduling, createJob);
            }

            if (!this.triggerCounts.ContainsKey(duration))
            {
                this.triggerCounts.Add(duration, 0);
            }

            this.triggerCounts[duration]++;

            this.Log.Debug($"Added trigger count for {barSpec.Period}-{barSpec.Resolution} " +
                      $"duration (count={this.triggerCounts[duration]}).");
        }

        private void OnMessage(Unsubscribe<BarType> message)
        {
            var symbol = message.DataType.Symbol;
            var barSpec = message.DataType.Specification;
            var duration = barSpec.Duration;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                // Nothing to unsubscribe from.
                return;
            }

            this.barAggregators[symbol].Send(message);

            if (this.barTriggers.ContainsKey(duration))
            {
                if (this.barTriggers[duration].Contains(barSpec))
                {
                    this.barTriggers[duration].Remove(barSpec);
                }
            }

            if (!this.triggerCounts.ContainsKey(barSpec.Duration))
            {
                return;
            }

            this.triggerCounts[barSpec.Duration]--;

            this.Log.Debug($"Subtracting trigger count for {barSpec.Period}-{barSpec.Resolution} " +
                           $"duration (count={this.triggerCounts[barSpec.Duration]}).");

            if (this.triggerCounts[barSpec.Duration] <= 0)
            {
                var job = this.barJobs[barSpec];
                var removeJob = new RemoveJob(
                    job.Key,
                    job.Value,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(ServiceAddress.Scheduling, removeJob);
            }
        }

        private void OnMessage(Tick tick)
        {
            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Send(tick);

                return;
            }

            // Log for debug purposes.
            // this.Log.Warning($"No bar aggregator for {tick.Symbol} ticks.");
        }

        private void OnMessage(BarJob job)
        {
            var closeTime = this.TimeNow().Floor(job.BarSpec.Duration);
            foreach (var aggregator in this.barAggregators.Values)
            {
                // TODO: Change this logic.
                var closeBar1 = new CloseBar(
                    new BarSpecification(1, job.BarSpec.Resolution, QuoteType.BID),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                var closeBar2 = new CloseBar(
                    new BarSpecification(1, job.BarSpec.Resolution, QuoteType.ASK),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                var closeBar3 = new CloseBar(
                    new BarSpecification(1, job.BarSpec.Resolution, QuoteType.MID),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                aggregator.Send(closeBar1);
                aggregator.Send(closeBar2);
                aggregator.Send(closeBar3);
            }
        }

        private void OnMessage(MarketStatusJob job)
        {
            if (job.IsMarketOpen)
            {
                // The market is now open.
                this.isMarketOpen = true;

                // Resume all active bar jobs.
                foreach (var barJob in this.barJobs.Values)
                {
                    var resumeJob = new ResumeJob(
                        barJob.Key,
                        this.NewGuid(),
                        this.TimeNow());

                    this.Send(ServiceAddress.Scheduling, resumeJob);
                }

                // Tell all bar aggregators the market is now open.
                var marketOpened = new MarketOpened(this.NewGuid(), this.TimeNow());
                foreach (var aggregator in this.barAggregators.Values)
                {
                    aggregator.Send(marketOpened);
                }
            }

            if (!job.IsMarketOpen)
            {
                // The market is now closed.
                this.isMarketOpen = false;

                // Pause all active bar jobs.
                foreach (var barJob in this.barJobs.Values)
                {
                    var pause = new PauseJob(
                        barJob.Key,
                        this.NewGuid(),
                        this.TimeNow());

                    this.Send(ServiceAddress.Scheduling, pause);
                }

                // Tell all aggregators the market is now closed.
                var marketClosed = new MarketClosed(this.NewGuid(), this.TimeNow());
                foreach (var aggregator in this.barAggregators.Values)
                {
                    aggregator.Send(marketClosed);
                }
            }
        }

        private void OnMessage((BarType, Bar) data)
        {
            // Forward data to bar publisher.
            this.barPublisher.Send(data);

            var dataDelivery = new DataDelivery<(BarType, Bar)>(
                data,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DataServiceAddress.DatabaseTaskManager, dataDelivery);
        }
    }
}
