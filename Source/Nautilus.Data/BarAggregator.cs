//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="BarAggregator"/> class.
    /// </summary>
    public sealed class BarAggregator : ActorComponentBase
    {
        private readonly Symbol symbol;
        private readonly BarSpecification barSpecification;
        private readonly SpreadAnalyzer spreadAnalyzer;
        private readonly int decimals;

        private BarBuilder barBuilder;
        private ZonedDateTime barEndTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="symbolBarSpec">The symbol bar specification.</param>
        /// <param name="tickSize">The tick size.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BarAggregator(
            IComponentryContainer container,
            Enum serviceContext,
            SymbolBarSpec symbolBarSpec,
            decimal tickSize)
            : base(
            serviceContext,
            LabelFactory.Component(
                nameof(BarAggregator),
                symbolBarSpec),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(serviceContext, nameof(serviceContext));
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            this.symbol = symbolBarSpec.Symbol;
            this.barSpecification = symbolBarSpec.BarSpecification;
            this.spreadAnalyzer = new SpreadAnalyzer(tickSize);
            this.decimals = tickSize.GetDecimalPlaces();

            this.IsTimeBar = true;
            if (this.barSpecification.Resolution == BarResolution.Tick)
            {
                this.IsTimeBar = false;
            }

            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Gets a value indicating whether this is a time based bar aggregator.
        /// </summary>
        private bool IsTimeBar { get; }

        /// <summary>
        /// Gets a value indicating whether this is a tick based bar aggregator.
        /// </summary>
        private bool IsTickBar => !this.IsTimeBar;

        /// <summary>
        /// Gets a value indicating whether the bar aggregator has been initialized.
        /// </summary>
        private bool IsInitialized { get; set; }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnReceive(msg));
        }

        private void OnReceive(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            if (!this.IsInitialized)
            {
                this.OnFirstTick(tick);
            }

            this.spreadAnalyzer.OnQuote(tick);
            this.UpdateBarBuilder(tick);

            if (this.IsTimeBar && tick.Timestamp.IsGreaterThanOrEqualTo(this.barEndTime))
            {
                // The ticks timestamp is beyond when the bar expected to close.
                while (tick.Timestamp.IsGreaterThanOrEqualTo(this.barEndTime + this.barSpecification.Duration))
                {
                    this.CloseBar(tick, this.barEndTime);
                    this.UpdateBarBuilder(tick);
                }

                this.CloseBar(tick, tick.Timestamp);
            }
            if (this.IsTickBar && this.barBuilder.Volume >= this.barSpecification.Period)
            {
                this.CloseBar(tick, tick.Timestamp);
            }
        }

        private void OnFirstTick(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            this.CreateBarBuilder(tick.Timestamp);
            this.IsInitialized = true;

            this.Log.Debug($"Registered for {this.barSpecification} bars");
            this.Log.Debug($"Receiving quotes ({tick.Symbol.Code}) from {tick.Symbol.Exchange}...");
        }

        private void CreateBarBuilder(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            if (this.IsTimeBar)
            {
                var barStartTime = this.CalculateBarStartTime(timeNow);
                this.barEndTime = barStartTime + this.barSpecification.Duration;
            }

            this.barBuilder = new BarBuilder();
        }

        private ZonedDateTime CalculateBarStartTime(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var dotNetDateTime = timeNow.ToDateTimeUtc();

            if (this.barSpecification.Resolution == BarResolution.Second)
            {
                var secondsStart = Math.Floor(dotNetDateTime.Second / (double)this.barSpecification.Period) * this.barSpecification.Period;

                var dateTimeStart = new DateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    timeNow.Minute,
                    (int)secondsStart,
                    DateTimeKind.Utc);

                var instantStart = Instant.FromDateTimeUtc(dateTimeStart);

                return new ZonedDateTime(instantStart, DateTimeZone.Utc);
            }

            if (this.barSpecification.Resolution == BarResolution.Minute)
            {
                var minutesStart = Math.Floor(dotNetDateTime.Minute / (double)this.barSpecification.Period) * this.barSpecification.Period;

                var dateTimeStart = new DateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    timeNow.Hour,
                    (int)minutesStart,
                    0,
                    DateTimeKind.Utc);

                var instantStart = Instant.FromDateTimeUtc(dateTimeStart);

                return new ZonedDateTime(instantStart, DateTimeZone.Utc);
            }

            if (this.barSpecification.Resolution == BarResolution.Hour)
            {
                var hoursStart = Math.Floor(dotNetDateTime.Hour / (double)this.barSpecification.Period) * this.barSpecification.Period;

                var dateTimeStart = new DateTime(
                    timeNow.Year,
                    timeNow.Month,
                    timeNow.Day,
                    (int)hoursStart,
                    0,
                    0,
                    DateTimeKind.Utc);

                var instantStart = Instant.FromDateTimeUtc(dateTimeStart);

                return new ZonedDateTime(instantStart, DateTimeZone.Utc);
            }

            throw new InvalidOperationException($"BarSpecification {this.barSpecification} currently not supported.");
        }

        private void UpdateBarBuilder(Tick tick)
        {
            switch (this.barSpecification.QuoteType)
            {
                case BarQuoteType.Bid:
                    this.barBuilder.OnQuote(tick.Bid, tick.Timestamp);
                    break;

                case BarQuoteType.Ask:
                    this.barBuilder.OnQuote(tick.Ask, tick.Timestamp);
                    break;

                case BarQuoteType.Mid:
                    this.barBuilder.OnQuote(
                        Price.Create(Math.Round(tick.Bid + tick.Ask / 2, decimals), decimals), tick.Timestamp);
                    break;
                default:
                    throw new InvalidOperationException("The quote type is not recognized.");
            }
        }

        private BarDataEvent CreateBarDataEvent(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            var newBar = this.IsTimeBar
                ? this.barBuilder.Build(this.barEndTime)
                : this.barBuilder.Build(tick.Timestamp);

            return new BarDataEvent(
                this.symbol,
                this.barSpecification,
                newBar,
                tick,
                this.spreadAnalyzer.AverageSpread,
                false,
                this.NewGuid(),
                this.TimeNow());
        }

        private void CloseBar(Tick tick, ZonedDateTime timestamp)
        {
            var @event = this.CreateBarDataEvent(tick);
            this.CreateBarBuilder(timestamp);
            this.spreadAnalyzer.OnBarUpdate(timestamp);

            Context.Parent.Tell(@event);
        }
    }
}
