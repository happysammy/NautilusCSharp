//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Aggregators
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Enums;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Documents;
    using Nautilus.Database.Messages.Events;
    using Nautilus.Database.Messages.Jobs;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using NodaTime;
    using Quartz;

    using Resolution = Nautilus.DomainModel.Enums.Resolution;
    using Tick = Nautilus.DomainModel.ValueObjects.Tick;
    using Bar = Nautilus.DomainModel.ValueObjects.BarSpecification;
    using BarSpecification = Nautilus.DomainModel.ValueObjects.BarSpecification;
    using QuoteType = Nautilus.DomainModel.Enums.QuoteType;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;


    /// <summary>
    /// This class is responsible for coordinating the creation of closed <see cref="Bar"/> data
    /// events from ingested <see cref="Tick"/>s based on bar jobs created from subscriptions.
    /// </summary>
    [PerformanceOptimized]
    public sealed class BarAggregationController : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly Dictionary<Symbol, IActorRef> barAggregators;
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
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            ServiceContext.Database,
            LabelFactory.Component(nameof(BarAggregationController)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.storedContainer = container;
            this.barAggregators = new Dictionary<Symbol, IActorRef>();
            this.barJobs = new Dictionary<BarSpecification, KeyValuePair<JobKey, TriggerKey>>();
            this.barTriggers = new Dictionary<Duration, List<BarSpecification>>();
            this.triggers = new Dictionary<Duration, ITrigger>();
            this.triggerCounts = new Dictionary<Duration, int>();

            this.isMarketOpen = this.IsFxMarketOpen();

            // Command messages
            this.Receive<StartSystem>(msg => this.OnMessage(msg));
            this.Receive<Subscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<Unsubscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<JobCreated>(msg => this.OnMessage(msg));
            this.Receive<JobRemoved>(msg => this.OnMessage(msg));
            this.Receive<RemoveJobFail>(msg => this.OnMessage(msg));
            this.Receive<BarJob>(msg => this.OnMessage(msg));
            this.Receive<MarketStatusJob>(msg => this.OnMessage(msg));

            // Event messages
            this.Receive<Tick>(msg => this.OnMessage(msg));
            this.Receive<BarClosed>(msg => this.OnMessage(msg));
        }

        private void OnMessage(StartSystem message)
        {
            Debug.NotNull(message, nameof(message));

            this.CreateMarketOpenedJob();
            this.CreateMarketClosedJob();
        }

        /// <summary>
        /// Handles the message by creating a <see cref="BarAggregator"/> for the symbol if none
        /// exists, then forwarding the message there. Bar jobs and then registered with the
        /// <see cref="Akka.Actor.IScheduler"/>.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(Subscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            var symbol = message.DataType.Symbol;
            var barSpec = message.DataType.Specification;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new BarAggregator(
                    this.storedContainer,
                    symbol)));

                this.barAggregators.Add(message.DataType.Symbol, barAggregatorRef);
            }

            this.barAggregators[symbol].Tell(message);

            var duration = barSpec.Duration;

            if (!this.triggers.ContainsKey(duration))
            {
                var trigger = this.CreateTrigger(barSpec);
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
                    this.Self,
                    barJob,
                    this.triggers[duration],
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(DatabaseService.Scheduler, createJob);
            }

            if (!this.triggerCounts.ContainsKey(duration))
            {
                this.triggerCounts.Add(duration, 0);
            }

            this.triggerCounts[duration]++;

            Log.Debug($"Added trigger count for {barSpec.Period}-{barSpec.Resolution} " +
                      $"duration (count={this.triggerCounts[duration]}).");
        }

        /// <summary>
        /// Handles the message by cancelling the bar jobs with the <see cref="Akka.Actor.IScheduler"/>, then
        /// forwarding the message to the <see cref="BarAggregator"/> for the symbol.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(Unsubscribe<BarType> message)
        {
            Debug.NotNull(message, nameof(message));

            var symbol = message.DataType.Symbol;
            var barSpec = message.DataType.Specification;
            var duration = barSpec.Duration;

            if (!this.barAggregators.ContainsKey(symbol))
            {
                return;
            }

            this.barAggregators[symbol].Tell(message);

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
                    "null job",
                    this.Self,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(DatabaseService.Scheduler, removeJob);
            }
        }

        private void OnMessage(JobCreated message)
        {
            Debug.NotNull(message, nameof(message));

            var job = message.Job as BarJob;

            if (job != null)
            {
                var barSpec = job.BarSpec;

                if (!this.barJobs.ContainsKey(barSpec))
                {
                    this.barJobs.Add(barSpec, new KeyValuePair<JobKey, TriggerKey>(
                        message.JobKey,
                        message.TriggerKey));
                }
            }

            Log.Information($"Job created Key={message.JobKey}, TriggerKey={message.TriggerKey}");
        }

        private void OnMessage(JobRemoved message)
        {
            Debug.NotNull(message, nameof(message));

            Log.Information($"Job removed Key={message.JobKey}, TriggerKey={message.TriggerKey}");
        }

        private void OnMessage(RemoveJobFail message)
        {
            Debug.NotNull(message, nameof(message));

            Log.Warning($"Remove job failed Key={message.JobKey}, TriggerKey={message.TriggerKey}, Reason={message.Reason.Message}");
        }

        /// <summary>
        /// Handles the message by forwarding the given <see cref="Tick"/> to the relevant
        /// <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="tick">The received tick.</param>
        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Tell(tick);
            }
        }

        /// <summary>
        /// Handles the message by creating a <see cref="CloseBar"/> command message which is then
        /// forwarded to the relevant <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="job">The received job.</param>
        private void OnMessage(BarJob job)
        {
            Debug.NotNull(job, nameof(job));

            var closeTime = this.TimeNow().Floor(job.BarSpec.Duration);
            foreach (var aggregator in this.barAggregators)
            {
                // TODO: Change this logic.
                var closeBar1 = new CloseBar(
                    new BarSpecification(QuoteType.Bid, job.BarSpec.Resolution, 1),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                var closeBar2 = new CloseBar(
                    new BarSpecification(QuoteType.Ask, job.BarSpec.Resolution, 1),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                var closeBar3 = new CloseBar(
                    new BarSpecification(QuoteType.Mid, job.BarSpec.Resolution, 1),
                    closeTime,
                    this.NewGuid(),
                    this.TimeNow());

                aggregator.Value.Tell(closeBar1);
                aggregator.Value.Tell(closeBar2);
                aggregator.Value.Tell(closeBar3);

                // Log for unit testing only.
                // Log.Debug($"Received {job} at {this.TimeNow().ToIsoString()}.");
            }

            //Log.Warning($"Does not contain aggregator to close bar for {job}.");
        }

        /// <summary>
        /// Handles the message by creating a <see cref="CloseBar"/> command message which is then
        /// forwarded to the relevant <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="job">The received job.</param>
        private void OnMessage(MarketStatusJob job)
        {
            Debug.NotNull(job, nameof(job));

            if (job.IsMarketOpen)
            {
                // The market is now open.
                this.isMarketOpen = true;
                var statusChangeJob = new MarketStatusJob(true);

                foreach (var aggregator in this.barAggregators.Values)
                {
                    aggregator.Tell(statusChangeJob);
                }
            }

            if (!job.IsMarketOpen)
            {
                // The market is now closed.
                this.isMarketOpen = false;
                var statusChangeJob = new MarketStatusJob(false);

                foreach (var aggregator in this.barAggregators.Values)
                {
                    aggregator.Tell(statusChangeJob);
                }
            }
        }

        /// <summary>
        /// Handles the message by creating a new event message which is then forwarded to the
        /// list of held bar data event receivers.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(BarClosed message)
        {
            var document = new DataDelivery<BarClosed>(
                message,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DatabaseService.CollectionManager, document);
        }

        private ITrigger CreateTrigger(BarSpecification barSpec)
        {
            Debug.NotNull(barSpec, nameof(barSpec));
            Debug.DictionaryDoesNotContainKey(barSpec.Duration, nameof(barSpec.Duration), this.triggers.ToImmutableDictionary());

            var duration = barSpec.Duration;

            return TriggerBuilder
                .Create()
                .StartAt(this.TimeNow().Ceiling(duration).ToDateTimeUtc())
                .WithIdentity($"{barSpec.Period}-{barSpec.Resolution.ToString().ToLower()}", "bar_aggregation")
                .WithSchedule(this.CreateScheduleBuilder(barSpec))
                .Build();
        }

        private IScheduleBuilder CreateScheduleBuilder(BarSpecification barSpec)
        {
            var scheduleBuilder = SimpleScheduleBuilder
                .Create()
                .RepeatForever()
                .WithMisfireHandlingInstructionFireNow();

            switch (barSpec.Resolution)
            {
                case Resolution.Tick:
                    throw new InvalidOperationException("Cannot schedule tick bars.");

                case Resolution.Second:
                    scheduleBuilder.WithIntervalInSeconds(barSpec.Period);
                    break;

                case Resolution.Minute:
                    scheduleBuilder.WithIntervalInMinutes(barSpec.Period);
                    break;

                case Resolution.Hour:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period);
                    break;

                case Resolution.Day:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period * 24);
                    break;
                default: throw new InvalidOperationException("Bar resolution not recognised.");
            }

            return scheduleBuilder;
        }

        private void CreateMarketOpenedJob()
        {
            var scheduleBuilder = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Sunday, 21, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"market_opened", "bar_aggregation")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.Self,
                new MarketStatusJob(true),
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DatabaseService.Scheduler, createJob);
            this.Log.Information("Created MarketStatusJob for market open Sundays 21:00 (UTC).");
        }

        private void CreateMarketClosedJob()
        {
            var scheduleBuilder = CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Saturday, 20, 00)
                .InTimeZone(TimeZoneInfo.Utc)
                .WithMisfireHandlingInstructionFireAndProceed();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"market_closed", "bar_aggregation")
                .WithSchedule(scheduleBuilder)
                .Build();

            var createJob = new CreateJob(
                this.Self,
                new MarketStatusJob(false),
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.Send(DatabaseService.Scheduler, createJob);
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
    }
}
