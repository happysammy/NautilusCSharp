//--------------------------------------------------------------------------------------------------
// <copyright file="TimeBarAggregator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Data.Builders;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="TimeBarAggregator"/> class.
    /// </summary>
    public sealed class TimeBarAggregator : ActorComponentBase
    {
        private readonly Symbol symbol;
        private readonly BarSpecification barSpecification;
        private readonly SpreadAnalyzer spreadAnalyzer;

        private BarBuilder barBuilder;
        private ZonedDateTime barEndTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBarAggregator"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="symbolBarSpec">The symbol bar specification.</param>
        /// <param name="tickSize">The tick size.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public TimeBarAggregator(
            IComponentryContainer container,
            Enum serviceContext,
            SymbolBarSpec symbolBarSpec,
            decimal tickSize)
            : base(
                serviceContext,
                LabelFactory.Component(
                    nameof(TimeBarAggregator),
                    symbolBarSpec),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(serviceContext, nameof(serviceContext));
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            this.symbol = symbolBarSpec.Symbol;
            this.barSpecification = symbolBarSpec.BarSpecification;
            this.spreadAnalyzer = new SpreadAnalyzer(tickSize);

            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnReceive(msg));
        }

        private void OnReceive(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            if (this.barBuilder == null)
            {
                this.OnFirstTick(quote);

                return;
            }

            this.barBuilder.OnQuote(quote.Bid, quote.Timestamp);
            this.spreadAnalyzer.OnQuote(quote);

            if (quote.Timestamp.Compare(this.barEndTime) >= 0)
            {
                this.CreateNewMarketDataEvent(quote);
                this.CreateBarBuilder(quote);
                this.spreadAnalyzer.OnBarUpdate(this.barEndTime);
            }
        }

        private void OnFirstTick(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            this.CreateBarBuilder(quote);

            this.Log.Debug(
                  $"Registered for {this.barSpecification} bars "
                + $"quoteBarStart={this.barBuilder.StartTime.ToIsoString()}, "
                + $"quoteBarEnd={this.barEndTime.ToIsoString()}");

            this.Log.Debug($"Receiving quotes ({quote.Symbol.Code}) from {quote.Symbol.Exchange}...");
        }

        private void CreateBarBuilder(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            var barStartTime = this.CalculateBarStartTime(quote.Timestamp);
            this.barEndTime = barStartTime + this.barSpecification.Duration;

            this.barBuilder = new BarBuilder(quote.Bid, quote.Timestamp);
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

        private void CreateNewMarketDataEvent(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            var newBar = this.barBuilder.Build(this.barBuilder.Timestamp);

            Context.Parent.Tell(new BarDataEvent(
                this.symbol,
                this.barSpecification,
                newBar,
                quote,
                this.spreadAnalyzer.AverageSpread,
                false,
                this.NewGuid(),
                this.TimeNow()),
                this.Self);
        }
    }
}
