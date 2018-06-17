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
    using DomainModel.Enums;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Database.Messages.Commands;
    using Nautilus.Database.Messages.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Scheduler.Commands;
    using Nautilus.Scheduler.Events;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// This class is responsible for coordinating the creation of closed <see cref="Bar"/> data
    /// events from ingested <see cref="Tick"/>s based on bar jobs created from subscriptions.
    /// </summary>
    public sealed class BarAggregationController : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IActorRef dataCollectionManagerRef;
        private readonly IActorRef schedulerRef;
        private readonly IDictionary<Symbol, IActorRef> barAggregators;
        private readonly IDictionary<Duration, KeyValuePair<JobKey, TriggerKey>> barJobs;
        private readonly IDictionary<Duration, ITrigger> triggers;
        private readonly IDictionary<Duration, int> triggerCounts;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="dataCollectionManagerRef">The bar data receivers.</param>
        /// <param name="schedulerRef">The scheduler actor address.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IActorRef dataCollectionManagerRef,
            IActorRef schedulerRef,
            Enum serviceContext)
            : base(
            serviceContext,
            LabelFactory.Component(nameof(BarAggregationController)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(dataCollectionManagerRef, nameof(dataCollectionManagerRef));
            Validate.NotNull(schedulerRef, nameof(schedulerRef));
            Validate.NotNull(serviceContext, nameof(serviceContext));

            this.storedContainer = container;
            this.dataCollectionManagerRef = dataCollectionManagerRef;
            this.schedulerRef = schedulerRef;
            this.barAggregators = new Dictionary<Symbol, IActorRef>();
            this.barJobs = new Dictionary<Duration, KeyValuePair<JobKey, TriggerKey>>();
            this.triggers = new Dictionary<Duration, ITrigger>();
            this.triggerCounts = new Dictionary<Duration, int>();

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<Subscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<Unsubscribe<BarType>>(msg => this.OnMessage(msg));
            this.Receive<JobCreated>(msg => this.OnMessage(msg));
            this.Receive<JobRemoved>(msg => this.OnMessage(msg));
            this.Receive<RemoveJobFail>(msg => this.OnMessage(msg));
            this.Receive<BarJob>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
            this.Receive<BarClosed>(msg => this.OnMessage(msg));
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
                    this.Service,
                    symbol)));

                this.barAggregators.Add(message.DataType.Symbol, barAggregatorRef);
            }

            this.barAggregators[symbol].Tell(message);

            var duration = barSpec.Duration;

            if (!this.triggers.ContainsKey(duration))
            {
                var trigger = this.CreateTrigger(barSpec);
                this.triggers.Add(duration, trigger);

                if (!this.barJobs.ContainsKey(duration))
                {
                    var barJob = new BarJob(message.DataType);

                    var createJob = new CreateJob(
                        this.Self,
                        barJob,
                        this.triggers[duration]);

                    this.schedulerRef.Tell(createJob);
                }
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

            if (!this.barAggregators.ContainsKey(symbol))
            {
                return;
            }

            this.barAggregators[symbol].Tell(message);

            if (!this.triggerCounts.ContainsKey(barSpec.Duration))
            {
                return;
            }

            this.triggerCounts[barSpec.Duration]--;

            this.Log.Debug($"Subtracting trigger count for {barSpec.Period}-{barSpec.Resolution} " +
                           $"duration (count={this.triggerCounts[barSpec.Duration]}).");

            if (this.triggerCounts[barSpec.Duration] <= 0)
            {
                var job = this.barJobs[barSpec.Duration];
                var removeJob = new RemoveJob(job.Key, job.Value, "null job");

                this.schedulerRef.Tell(removeJob);
            }

        }

        private void OnMessage(JobCreated message)
        {
            Debug.NotNull(message, nameof(message));

            var job = message.Job as BarJob;

            if (job != null)
            {
                var barSpec = job.BarType.Specification;
                var duration = barSpec.Duration;

                if (!this.barJobs.ContainsKey(duration))
                {
                    this.barJobs.Add(duration, new KeyValuePair<JobKey, TriggerKey>(
                        message.JobKey,
                        message.TriggerKey));
                }
            }

            Log.Debug($"Job created Key={message.JobKey}, TriggerKey={message.TriggerKey}");
        }

        private void OnMessage(JobRemoved message)
        {
            Debug.NotNull(message, nameof(message));

            Log.Debug($"Job removed Key={message.JobKey}, TriggerKey={message.TriggerKey}");
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
            Debug.DictionaryContainsKey(tick.Symbol, nameof(this.barAggregators), this.barAggregators.ToImmutableDictionary());

            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Tell(tick);

                return;
            }

            Log.Warning($"Does not contain aggregator for {tick.Symbol} ticks.");
        }

        /// <summary>
        /// Handles the message by creating a <see cref="CloseBar"/> command message which is then
        /// forwarded to the relevant <see cref="BarAggregator"/>.
        /// </summary>
        /// <param name="job">The received job.</param>
        private void OnMessage(BarJob job)
        {
            Debug.NotNull(job, nameof(job));
            Debug.DictionaryContainsKey(job.BarType.Symbol, nameof(this.barAggregators), this.barAggregators.ToImmutableDictionary());

            if (this.barAggregators.ContainsKey(job.BarType.Symbol))
            {
                var closeBar = new CloseBar(
                    job.BarType.Specification,
                    this.TimeNow().Floor(job.BarType.Specification.Duration),
                    this.NewGuid(),
                    this.TimeNow());

                this.barAggregators[job.BarType.Symbol].Tell(closeBar);

                // Log for unit testing only.
                Log.Debug($"Received {job} at {this.TimeNow().ToIsoString()}.");
                return;
            }

            Log.Warning($"Does not contain aggregator to close bar for {job}.");
        }

        /// <summary>
        /// Handles the message by creating a new event message which is then forwarded to the
        /// list of held bar data event receivers.
        /// </summary>
        /// <param name="message">The received message.</param>
        private void OnMessage(BarClosed message)
        {
            var dataDelivery = new DataDelivery<BarClosed>(
                message,
                this.NewGuid(),
                this.TimeNow());

            this.dataCollectionManagerRef.Tell(dataDelivery, this.Self);
        }

        private ITrigger CreateTrigger(BarSpecification barSpec)
        {
            Debug.NotNull(barSpec, nameof(barSpec));
            Debug.DictionaryDoesNotContainKey(barSpec.Duration, nameof(barSpec.Duration), this.triggers.ToImmutableDictionary());

            var duration = barSpec.Duration;

            return TriggerBuilder
                .Create()
                .StartAt(this.TimeNow().Ceiling(duration).ToDateTimeUtc())
                .WithIdentity($"{barSpec.Period}-{barSpec.Resolution}")
                .WithSchedule(this.CreateScheduleBuilder(barSpec))
                .Build();
        }

        private IScheduleBuilder CreateScheduleBuilder(BarSpecification barSpec)
        {
            var scheduleBuilder = SimpleScheduleBuilder
                .Create()
                .RepeatForever();

            switch (barSpec.Resolution)
            {
                case BarResolution.Tick:
                    throw new InvalidOperationException("Cannot schedule tick bars.");

                case BarResolution.Second:
                    scheduleBuilder.WithIntervalInSeconds(barSpec.Period);
                    break;

                case BarResolution.Minute:
                    scheduleBuilder.WithIntervalInMinutes(barSpec.Period);
                    break;

                case BarResolution.Hour:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period);
                    break;

                case BarResolution.Day:
                    scheduleBuilder.WithIntervalInHours(barSpec.Period * 24);
                    break;
                default: throw new InvalidOperationException("Bar resolution not recognised.");
            }

            return scheduleBuilder;
        }
    }
}
