//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Messages;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="BarAggregationController"/> class.
    /// </summary>
    public sealed class BarAggregationController : ActorComponentBusConnectedBase
    {
        private readonly IComponentryContainer storedContainer;
        private readonly IScheduler scheduler;
        private readonly IImmutableList<Enum> barDataReceivers;
        private readonly IDictionary<Symbol, IActorRef> barAggregators;
        private readonly IDictionary<(Symbol, BarSpecification), ICancelable> barJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregationController"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The actor system.</param>
        /// <param name="barReceivers">The bar data receivers.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregationController(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IScheduler scheduler,
            IImmutableList<Enum> barReceivers,
            Enum serviceContext)
            : base(
            serviceContext,
            LabelFactory.Component(nameof(BarAggregationController)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(scheduler, nameof(scheduler));
            Validate.NotNull(barReceivers, nameof(barReceivers));
            Validate.NotNull(serviceContext, nameof(serviceContext));

            this.storedContainer = container;
            this.scheduler = scheduler;
            this.barDataReceivers = barReceivers.ToImmutableList();
            this.barAggregators = new Dictionary<Symbol, IActorRef>();
            this.barJobs = new Dictionary<(Symbol, BarSpecification), ICancelable>();

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<SubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<UnsubscribeBarData>(msg => this.OnMessage(msg));
            this.Receive<(Symbol, BarSpecification)>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnMessage(msg));
            this.Receive<BarClosed>(msg => this.OnMessage(msg));
        }

        private void OnMessage(SubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));

            if (!this.barAggregators.ContainsKey(message.Symbol))
            {
                var barAggregatorRef = Context.ActorOf(Props.Create(() => new BarAggregator(
                    this.storedContainer,
                    this.Service,
                    message.Symbol)));

                this.barAggregators.Add(message.Symbol, barAggregatorRef);
            }

            this.barAggregators[message.Symbol].Tell(message);

            foreach (var barSpec in message.BarSpecifications)
            {
                var job = (message.Symbol, barSpec);
                this.AddJob(job);
            }
        }

        private void AddJob((Symbol Symbol, BarSpecification BarSpec) job)
        {
            if (!this.barJobs.ContainsKey(job))
            {
                var cancellationToken = this.scheduler.ScheduleTellRepeatedlyCancelable(
                    this.CalculateStartTimeDelay(job.BarSpec),
                    job.BarSpec.Duration.Milliseconds,
                    this.barAggregators[job.Item1],
                    job,
                    this.Self);

                this.barJobs.Add(job, cancellationToken);

                Log.Debug($"Bar job added {job}");
            }
        }

        private void RemoveJob((Symbol Symbol, BarSpecification BarSpec) job)
        {
            if (this.barJobs.ContainsKey(job))
            {
                this.barJobs[job].Cancel();
                this.barJobs.Remove(job);

                Log.Debug($"Bar job removed {job}");
            }
        }

        private void OnMessage(UnsubscribeBarData message)
        {
            Debug.NotNull(message, nameof(message));
            Validate.DictionaryContainsKey(message.Symbol, nameof(message.Symbol), this.barAggregators);

            foreach (var barSpec in message.BarSpecifications)
            {
                this.RemoveJob((message.Symbol, barSpec));
            }

            this.barAggregators[message.Symbol].Tell(message);
        }

        private void OnMessage(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            if (this.barAggregators.ContainsKey(tick.Symbol))
            {
                this.barAggregators[tick.Symbol].Tell(tick);
            }
        }

        private void OnMessage(BarClosed message)
        {
            var @event = new EventMessage(
                message,
                this.NewGuid(),
                this.TimeNow());

            this.Send(this.barDataReceivers, @event);
        }

        private void OnMessage((Symbol Symbol, BarSpecification BarSpec) message)
        {
            var closeBar = new CloseBar(
                message.BarSpec,
                this.CalculateCloseTime(message.BarSpec),
                this.NewGuid(),
                this.TimeNow());

            this.barAggregators[message.Symbol].Tell(closeBar);
        }

        private int CalculateStartTimeDelay(BarSpecification barSpec)
        {
            Debug.NotNull(barSpec, nameof(barSpec));

            var timeNow = this.TimeNow();

            if (barSpec.Resolution == BarResolution.Second)
            {
                var secondsStart = Math.Ceiling(timeNow.Second / (double) barSpec.Period) *
                                   barSpec.Period;

                var dateTimeStart = new ZonedDateTime(new LocalDateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    timeNow.Minute,
                    (int)secondsStart),
                    DateTimeZone.Utc,
                    Offset.Zero);

                return dateTimeStart.Millisecond - this.TimeNow().Millisecond;
            }

            if (barSpec.Resolution == BarResolution.Minute)
            {
                var minutesStart =
                    Math.Ceiling(timeNow.Minute / (double) barSpec.Period) *
                    barSpec.Period;

                var dateTimeStart = new ZonedDateTime(new LocalDateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    (int)minutesStart),
                    DateTimeZone.Utc,
                    Offset.Zero);

                return dateTimeStart.Millisecond - this.TimeNow().Millisecond;
            }

            if (barSpec.Resolution == BarResolution.Hour)
            {
                var hoursStart =
                    Math.Ceiling(timeNow.Hour / (double) barSpec.Period) *
                    barSpec.Period;

                var dateTimeStart = new ZonedDateTime(new LocalDateTime(
                        timeNow.Year,
                        timeNow.Month,
                        timeNow.Day,
                        (int)hoursStart,
                        0),
                    DateTimeZone.Utc,
                    Offset.Zero);

                return dateTimeStart.Millisecond - this.TimeNow().Millisecond;
            }

            if (barSpec.Resolution == BarResolution.Day)
            {
                var daysStart =
                    Math.Ceiling(timeNow.Day / (double) barSpec.Period) *
                    barSpec.Period;

                var dateTimeStart = new ZonedDateTime(new LocalDateTime(
                        timeNow.Year,
                        timeNow.Month,
                        (int)daysStart,
                        0,
                        0),
                    DateTimeZone.Utc,
                    Offset.Zero);

                return dateTimeStart.Millisecond - this.TimeNow().Millisecond;
            }

            throw new InvalidOperationException(
                $"BarSpecification {barSpec} currently not supported.");
        }

        private ZonedDateTime CalculateCloseTime(BarSpecification barSpec)
        {
            Debug.NotNull(barSpec, nameof(barSpec));

            var timeNow = this.TimeNow();

            if (barSpec.Resolution == BarResolution.Second)
            {
                var secondsStart = Math.Floor(timeNow.Second / (double) barSpec.Period) *
                                   barSpec.Period;

                return new ZonedDateTime(new LocalDateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    timeNow.Minute,
                    (int)secondsStart),
                    DateTimeZone.Utc,
                    Offset.Zero);
            }

            if (barSpec.Resolution == BarResolution.Minute)
            {
                var minutesStart =
                    Math.Floor(timeNow.Minute / (double) barSpec.Period) *
                    barSpec.Period;

                return new ZonedDateTime(new LocalDateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    (int)minutesStart),
                    DateTimeZone.Utc,
                    Offset.Zero);
            }

            if (barSpec.Resolution == BarResolution.Hour)
            {
                var hoursStart =
                    Math.Floor(timeNow.Hour / (double) barSpec.Period) *
                    barSpec.Period;

                return new ZonedDateTime(new LocalDateTime(
                        timeNow.Year,
                        timeNow.Month,
                        timeNow.Day,
                        (int)hoursStart,
                        0),
                    DateTimeZone.Utc,
                    Offset.Zero);
            }

            if (barSpec.Resolution == BarResolution.Day)
            {
                var daysStart =
                    Math.Ceiling(timeNow.Day / (double) barSpec.Period) *
                    barSpec.Period;

                return new ZonedDateTime(new LocalDateTime(
                        timeNow.Year,
                        timeNow.Month,
                        (int)daysStart,
                        0,
                        0),
                    DateTimeZone.Utc,
                    Offset.Zero);
            }

            throw new InvalidOperationException(
                $"BarSpecification {barSpec} currently not supported.");
        }
    }
}
