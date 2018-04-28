//--------------------------------------------------------------
// <copyright file="TimeBarAggregator.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.DomainModel.Extensions;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="TimeBarAggregator"/> class.
    /// </summary>
    public sealed class TimeBarAggregator : ActorComponentBusConnectedBase
    {
        private readonly Symbol symbol;
        private readonly BarSpecification BarSpecification;
        private readonly TradeType tradeType;
        private readonly SpreadAnalyzer spreadAnalyzer;

        private BarBuilder barBuilder;
        private ZonedDateTime barEndTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBarAggregator"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="message">The subsciption message.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public TimeBarAggregator(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            SubscribeSymbolDataType message)
            : base(
            BlackBoxService.Data,
            LabelFactory.ComponentByTradeType(
                nameof(TimeBarAggregator),
                message.Symbol,
                message.TradeType),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(message, nameof(message));

            this.symbol = message.Symbol;
            this.BarSpecification = message.BarSpecification;
            this.tradeType = message.TradeType;
            this.spreadAnalyzer = new SpreadAnalyzer(message.TickSize);

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

            if (ZonedDateTime.Comparer.Instant.Compare(quote.Timestamp, this.barEndTime) >= 0)
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

            this.Log(
                LogLevel.Debug,
                  $"Registered for {this.BarSpecification} bars "
                + $"quoteBarStart={this.barBuilder.StartTime.ToStringFormattedIsoUtc()}, "
                + $"quoteBarEnd={this.barEndTime.ToStringFormattedIsoUtc()}");

            this.Log(LogLevel.Debug, $"Receiving quotes ({quote.Symbol.Code}) from {quote.Symbol.Exchange}...");
        }

        private void CreateBarBuilder(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            var barStartTime = this.CalculateBarStartTime(quote.Timestamp);
            this.barEndTime = barStartTime + this.BarSpecification.Duration;

            this.barBuilder = new BarBuilder(quote.Bid, quote.Timestamp);
        }

        private ZonedDateTime CalculateBarStartTime(ZonedDateTime timeNow)
        {
            Debug.NotDefault(timeNow, nameof(timeNow));

            var dotNetDateTime = timeNow.ToDateTimeUtc();

            if (this.BarSpecification.Resolution == BarResolution.Second)
            {
                var secondsStart = Math.Floor(dotNetDateTime.Second / (double)this.BarSpecification.Period) * this.BarSpecification.Period;

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

            if (this.BarSpecification.Resolution == BarResolution.Minute)
            {
                var minutesStart = Math.Floor(dotNetDateTime.Minute / (double)this.BarSpecification.Period) * this.BarSpecification.Period;

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

            if (this.BarSpecification.Resolution == BarResolution.Hour)
            {
                var hoursStart = Math.Floor(dotNetDateTime.Hour / (double)this.BarSpecification.Period) * this.BarSpecification.Period;

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

            throw new InvalidOperationException($"BarSpecification {this.BarSpecification} currently not supported.");
        }

        private void CreateNewMarketDataEvent(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            var newBar = this.barBuilder.Build(this.barEndTime);

            var marketData = new MarketDataEvent(
                this.symbol,
                this.tradeType,
                this.BarSpecification,
                newBar,
                quote,
                this.spreadAnalyzer.AverageSpread,
                false,
                this.NewGuid(),
                this.TimeNow());

            var recipients =
                new List<Enum>
                    {
                        BlackBoxService.AlphaModel,
                        BlackBoxService.Portfolio,
                        BlackBoxService.Risk
                    };

            var eventMessage = new EventMessage(
                marketData,
                this.NewGuid(),
                this.TimeNow());

            this.MessagingAdapter.Send(
                recipients,
                eventMessage,
                BlackBoxService.Data);
        }
    }
}
